using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Util
{
    public static class SupaUtilities
    {
        public static T[] Merge<T>(T[][] arr)
        {
            int len = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null) continue;
                len += arr[i].Length;
            }
            T[] result = new T[len];
            int index = 0;
            for (int j = 0; j < arr.Length; j++)
            {
                if (arr[j] == null) continue;
                for (int k = 0; k < arr[j].Length; k++)
                {
                    result[index] = arr[j][k];
                    ++index;
                }
            }
            return result;
        }
        /// <summary>
        /// Untested
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static bool Contains<T>(T[] arr1, T[] arr2)
        {
            int index = 0;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (Equals(arr1[i], arr2[index]))
                {
                    index++;
                    if (index == arr2.Length) return true;
                }
                else
                {
                    index = 0;
                }
            }
            return false;
        }
    }
}
