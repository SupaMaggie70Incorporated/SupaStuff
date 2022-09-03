using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Util
{
    internal class BetterIndexArray<T>
    {
        public T this[int index]
        {
            get
            {
                int i = index % array.Length;
                return array[i];
            }
            set
            {
                int i = index % array.Length;
                array[i] = value;
            }
        }
        public T[] array;
        public static explicit operator BetterIndexArray<T>(T[] arr)
        {
            return new BetterIndexArray<T>(arr);
        }
        public static explicit operator T[](BetterIndexArray<T> arr)
        {
            return arr.array;
        }

        public BetterIndexArray(T[] arr)
        {
            array = arr;
        }
    }
}
