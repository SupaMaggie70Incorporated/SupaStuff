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



namespace SupaStuff.Net
{
    public class PacketStream : IDisposable
    {
        internal bool isServer;
        internal bool isRunning = true;
        internal ServerSide.ClientConnection clientConnection = null;
        private NetworkStream stream;
        private Func<bool> customOnError;
        public Logger logger = NetMain.NetLogger;
        //Server only
        internal DateTime lastCheckedIn = DateTime.UtcNow;
        public static readonly int MaxUncheckedTime = 10;

        #region Packet buffer

        bool packetHeaderComplete = false;
        byte[] packetHeader = new byte[8];
        int packetID = -1;
        int packetSize = -1;
        byte[] packetBody = null;
        int packetBodyIndex = 0;
        #endregion
        /// <summary>
        /// Called when an error occurs
        /// </summary>
        private void onError()
        {
            try
            {
                customOnError();
            }
            catch
            {

            }
        }
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


                        if (isServer)
                        {
                            if (!PacketTypesFinder.c2sTypes[packetID].isRightLength(packetSize))
                            {
                                Dispose();
                                return false;
                            }
                        }
                        else
                        {
                            if (!PacketTypesFinder.s2cTypes[packetID].isRightLength(packetSize))
                            {
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
            catch
            {
                logger.Log("Error recieving packet, disconnecting");
                onError();
                Dispose();
                return false;
            }
        }
        /// <summary>
        /// Check to see if any packets can be fully read
        /// </summary>
        private void CheckForPackets()
        {
            if (!isRunning) return;
            Packet packet;
            while (TryGetPacket(out packet))
            {
                HandleIncomingPacket(packet);
            }
        }
        private static readonly Type[] builtinPackets = new Type[]
        {
            typeof(C2SDisconnectPacket),
            typeof(S2CKickPacket),
            typeof(C2SBlankPacket),
            typeof(C2SWelcomePacket)
        };
        /// <summary>
        /// Called when a packet is recieved, to execute the packet's code and whatnot
        /// </summary>
        /// <param name="packet"></param>
        public void HandleIncomingPacket(Packet packet)
        {
            try
            {
                Type type = packet.GetType();
                if (isServer && !clientConnection.finishAuth && type != typeof(C2SWelcomePacket))
                {
                    logger.Log("We recieved a packet other than the C2SWelcomePacket as our first packet, so fuck off hacker");
                    onError();
                    Dispose();
                    return;
                }
                if (!builtinPackets.Contains(type))
                {
                    RecievePacketEvent(packet);
                }
                packet.Execute(clientConnection);
            }
            catch
            {
                if (this != null)
                {
                    logger.Log("We had issues handling a packet, so we're gonna commit die");
                    onError();
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
                Packet packet = Packet.GetPacket(packetID, packetBody, !isServer);
                PacketCleanup();
                if (isServer) lastCheckedIn = DateTime.UtcNow;
                return packet;
            }
            catch
            {
                logger.Log("Failed to complete the packet");
                onError();
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
        public PacketStream(NetworkStream stream, bool isServer, Func<bool> onError)
        {
            this.stream = stream;
            this.isServer = isServer;
            customOnError = onError;
        }

        /// <summary>
        /// Takes the packet queue, iterates through them, removes them if stale, otherwise processes them
        /// </summary>
        public void Update()
        {
            DateTime now = DateTime.UtcNow;





            if (!isServer && SupaMath.TimeBetween(lastCheckedIn, now) > 5)
            {
                SendPacket(new C2SBlankPacket());
                lastCheckedIn = now;
            }
            if (isServer && SupaMath.TimeBetween(lastCheckedIn, now) > MaxUncheckedTime)
            {
                logger.Log("We kicked a client because they waited too long to check in");
                onError();
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
            DisconnectedEvent();
        }
        /// <summary>
        /// Send a packet
        /// </summary>
        /// <param name="packet"></param>
        public void SendPacket(Packet packet)
        {
            stream.Write(packet.GenerateHeader(), 0, 8);
            stream.Write(packet.data, 0, packet.data.Length);

        }
        /// <summary>
        /// Delegate function for when you recieve a packet
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        /// <summary>
        /// Called when a packet is recieved
        /// </summary>
        public event Action<Packet> OnRecievePacket;
        private void RecievePacketEvent(Packet packet)
        {
            if (OnRecievePacket == null) return;
            OnRecievePacket.Invoke(packet);
        }
        public event Action OnDisconnected;

        private void DisconnectedEvent()
        {
            if (!isRunning) return;
            isRunning = false;
            OnDisconnected?.Invoke();
            if (isServer)
            {
                NetMain.ServerLogger.Log("Client decided to disconnect!");
            }
            else
            {
                NetMain.ClientLogger.Log("Server aborted connection!");
            }
        }

    }
}