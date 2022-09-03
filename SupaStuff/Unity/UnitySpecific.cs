using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Unity
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple = false,Inherited = true)]
    public class UnitySpecific : Attribute
    {
    }
}
