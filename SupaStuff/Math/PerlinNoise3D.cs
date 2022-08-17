using System;
using System.Collections.Generic;
using System.Text;
/*
namespace SupaStuff.Math
{
    internal class PerlinNoise3D : PerlinNoise
    {
        
        static int noise3(int x,int y,int z,byte seed)
        {

        }
        static float lin_inter(float x,float y,float z,float s)
        {

        }
        static float smooth_inter(float x,float y,float z,float s)
        {

        }
        static float noise3d(float x,float y,float z,byte seed)
        {
            int x_int = (int)System.Math.Floor(x);
            int y_int = (int)System.Math.Floor(y);
            int z_int = (int)System.Math.Floor(z);
            float x_frac = x - x_int;
            float y_frac = y - y_int;
            float z_frac = z - z_int;
        }
        static float perlin3d(float x, float y, float z, float freq, int depth, byte seed)
        {
            float xa = x * freq;
            float ya = y * freq;
            float za = z * freq;

            float amp = 1.0f;
            float fin = 0;
            float div = 0.0f;


            for(int i = 0; i < depth; i++)
            {
                div += 256 * amp;
                fin += noise3d(xa, ya, za, seed);
                amp /= 2;
                xa *= 2;
                ya *= 2;
            }

            return fin / div;
        }


        public PerlinNoise3D(byte seed, float frequency = 1f, int depth = 1) : base(seed, frequency, depth)
        {

        }
        public PerlinNoise3D(float frequency = 1f, int depth = 1) : base(RandByte(), frequency, depth)
        {

        }


        public float Get(float x,float y,float z)
        {

        }
        public static float Get(float x, float y, float z, float frequency = 1f, int depth = 1, int seed = 0)
        {

        }
    }
}
*/