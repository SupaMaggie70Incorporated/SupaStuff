using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

using System.Reflection;

using System.Net.Sockets;
using SupaStuff.Net.Packets;
using SupaStuff.Net.Packets.BuiltIn;

using SupaStuff.Util;
using SupaStuff.Math;
using SupaStuff.Net.ServerSide;

namespace SupaStuff.Net
{
    /// <summary>
    /// The shared code between client and server that handles the sending/receiving of packets
    /// </summary>
    internal class PacketStream : IDisposable
    {
        /// <summary>
        /// The client or clientconnection which controls this
        /// </summary>
        private IDisposable Controller;
        /// <summary>
        /// Whether or not the packetstream should treat packets as being received by the server
        /// </summary>
        private bool IsServer;
        /// <summary>
        /// Whether or not the packetstream is still active and listening
        /// </summary>
        internal bool Running = true;
        /// <summary>
        /// The clientconnection which corresponds to the client if on a server
        /// </summary>
        private ServerSide.IClientConnection clientConnection = null;
        /// <summary>
        /// The stream data is sent over
        /// </summary>
        private NetworkStream stream;
        /// <summary>
        /// The logger to be used by messages
        /// </summary>
        private Logger logger = NetMain.NetLogger;

        /// <summary>
        /// Serveronly - When the client last sent a packet
        /// </summary>
        private DateTime lastCheckedIn = DateTime.UtcNow;

        /// <summary>
        /// How long after a client's last packet they should be kicked, in seconds
        /// </summary>
        private const int MaxUncheckedTime = 10;

        #region Packet buffer

        bool packetHeaderComplete = false;
        byte[] packetHeader = new byte[8];
        int packetID = -1;
        int packetSize = -1;
        byte[] packetBody = null;
        int packetBodyIndex = 0;
        #endregion
        /// <summary>
        /// Tries to recieve a packet, if it can't recieve the whole thing saves what it did get to variables to be continued later
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private bool TryGetPacket(out Packet packet)
        {
            packet = null;
            try
            {
                if (!stream.DataAvailable || !stream.CanRead)
                {
                    return false;
                }
                if (!packetHeaderComplete)
                {
                    int headerReqLength = 8 - packetBodyIndex;
                    int headerAmountRead = stream.Read(packetHeader, packetBodyIndex, headerReqLength);
                    if (headerAmountRead == headerReqLength)
                    {
                        packetID = BitConverter.ToInt32(packetHeader, 0);
                        packetSize = BitConverter.ToInt32(packetHeader, 4);
                        packetHeaderComplete = true;
                        packetBody = new byte[packetSize];
                        packetBodyIndex = 0;


                        if (IsServer)
                        {
                            if (!PacketTypesFinder.C2STypes[packetID].IsRightLength(packetSize))
                            {
                                NetMain.ServerLogger.Error($"Disconnecting client because their packet of type {PacketTypesFinder.C2STypes[packetID].Type.FullName} had length {packetSize} which was not valid.");

                                Dispose();
                                return false;
                            }
                        }
                        else
                        {
                            if (!PacketTypesFinder.S2CTypes[packetID].IsRightLength(packetSize))
                            {
                                NetMain.ClientLogger.Error($"Disconnecting because their packet of type {PacketTypesFinder.S2CTypes[packetID].Type.FullName} had length {packetSize} which was not valid.");

                                Dispose();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        packetBodyIndex += headerAmountRead;
                        return false;
                    }
                }
                if (packetSize == 0)
                {
                    packet = FinishRecievePacket();
                    return true;
                }
                int reqLength = packetSize - packetBodyIndex;
                int amountRead = stream.Read(packetBody, packetBodyIndex, reqLength);
                if (reqLength == amountRead)
                {
                    packet = FinishRecievePacket();
                    return true;
                }
                else
                {
                    packetBodyIndex += amountRead;
                    return false;
                }
            }
            catch(Exception e)
            {
                logger.Warn("Error recieving packet, disconnecting. Error message shown below.");
                logger.Error(e);


                Dispose();
                return false;
            }
        }
        /// <summary>
        /// Check to see if any packets can be fully read
        /// </summary>
        private void CheckForPackets()
        {
            if (!Running) return;
            Packet packet;
            while (TryGetPacket(out packet))
            {
                HandleIncomingPacket(packet);
            }
        }
        /// <summary>
        /// Called when a packet is recieved, to execute the packet's code and whatnot
        /// </summary>
        /// <param name="packet"></param>
        public void HandleIncomingPacket(Packet packet)
        {
            try
            {
                Type type = packet.GetType();
                if (IsServer && !clientConnection.AuthFinished() && type != typeof(C2SWelcomePacket))
                {
                    logger.Warn("We recieved a packet other than the C2SWelcomePacket as our first packet, closing connection");
                    Dispose();
                    return;
                }
                try
                {
                    if (!Packet.ExecutePacket(packet, false, clientConnection))
                    {
                        Dispose();
                    }
                }
                catch(Exception e)
                {
                    logger.Warn("We had issues handling a packet; closing");
                    Dispose();

                    logger.Warn("Packet of type " + type.FullName + " could not be recieved:\n" + e.ToString());
                }
            }
            catch
            {
                if (this != null)
                {
                    logger.Warn("We had issues handling a packet; closing");
                    Dispose();
                }
            }
        }
        /// <summary>
        /// Finish recieving a packet
        /// </summary>
        /// <returns></returns>
        private Packet FinishRecievePacket()
        {
            try
            {
                Packet packet = Packet.GetPacket(packetID, packetBody, !IsServer);
                PacketCleanup();
                if (IsServer) lastCheckedIn = DateTime.UtcNow;
                return packet;
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
                Dispose();
                return null;
            }

        }
        /// <summary>
        /// Cleans up variables after a packet is recieved
        /// </summary>
        private void PacketCleanup()
        {
            packetID = -1;
            packetBody = null;
            packetBodyIndex = 0;
            packetHeader = new byte[8];
            packetSize = -1;
            packetHeaderComplete = false;

        }
        /// <summary>
        /// The main constructor
        /// </summary>
        /// <param name="stream">The stream to send and recieve from</param>
        /// <param name="isServer">Whether or not it is a server, used for packet decoding</param>
        /// <param name="onError">The function to be called on errors</param>
        internal PacketStream(NetworkStream stream, bool isServer, IDisposable controller)
        {
            this.stream = stream;
            this.IsServer = isServer;
            this.logger = isServer ? NetMain.ServerLogger : NetMain.ClientLogger;
            Controller = controller;
            clientConnection = controller as IClientConnection;
        }
        ~PacketStream()
        {
            Dispose();
        }

        /// <summary>
        /// Takes the packet queue, iterates through them, removes them if stale, otherwise processes them
        /// </summary>
        internal void Update()
        {
            if (!Running) return;
            DateTime now = DateTime.UtcNow;





            if (!IsServer && SupaMath.TimeBetween(lastCheckedIn, now) > 5)
            {
                SendPacket(new C2SBlankPacket());
                lastCheckedIn = now;
            }
            if (IsServer && SupaMath.TimeBetween(lastCheckedIn, now) > MaxUncheckedTime)
            {
                logger.Warn("We kicked a client because they waited too long to check in");
                Dispose();
            }

            //Check for new packets to recieve
            CheckForPackets();
        }
        /// <summary>
        /// Called to ease up the Garbage collection by disposing manually
        /// </summary>
        public void Dispose()
        {
            if (!Running) return;
            Running = false;
            Controller.Dispose();
            GC.SuppressFinalize(this);
            OnDisconnected?.Invoke();
            if (IsServer)
            {
                NetMain.ServerLogger.Warn("Client decided to disconnect!");
            }
            else
            {
                NetMain.ClientLogger.Warn("Server aborted connection!");
            }
        }
        /// <summary>
        /// Send a packet
        /// </summary>
        /// <param name="packet"></param>
        internal void SendPacket(Packet packet)
        {
            if (!Running) return;
            try
            {
                stream.Write(packet.GenerateHeader(), 0, 8);
                stream.Write(packet.Data, 0, packet.Data.Length);
            }catch
            {
                logger.Warn("Failed to send a packet, probably because they closed their side. Either way, closing");
                Dispose();
            }

        }
        internal event Action OnDisconnected;

    }
}