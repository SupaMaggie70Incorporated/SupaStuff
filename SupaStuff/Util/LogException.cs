using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupaStuff.Util
{
    public class LogException : Exception
    {
        public override string ToString()
        {
            return Message;
        }
        public LogException() : base("Unspecified log exception thrown!")
        {
        }
        public LogException(string message) : base(message)
        {
        }
        public static LogException NotUnity()
        {
            return new LogException("You tried to call a unity function when it is not currently running on unity mode!");
        }
    }
}