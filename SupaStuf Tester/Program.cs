using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Unity;
using SupaStuff.Net.Example;


namespace SupaStuff_Tester
{
    public class Program
    {
        public static void Main()
        {
            TestSphere();
        }
        public static void TestSphere()
        {
            IcosphereGenerator gen = new IcosphereGenerator(5);
            gen.Subdivide(1);
            int[] triangles = gen.triangles;
            int vertslength = gen.vertices.Length;
            int HighestValue = 0;
            for(int i = 0;i < triangles.Length;i++)
            {
                int tri = triangles[i];
                if(tri >= vertslength)
                {
                    Console.WriteLine($"Triangle with index {i} is out of the vertices array range of {vertslength} with value {tri}");
                }
                if(tri > HighestValue) HighestValue = tri;
            }
            Console.WriteLine($"Highest value: {HighestValue}, vertices array length: {vertslength}");
            Console.WriteLine("Complete");
        }
        public static void TestNet()
        {
            new ExampleDemo();

        }
    }
}