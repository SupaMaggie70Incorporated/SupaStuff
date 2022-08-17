using System.Collections;
using System.Collections.Generic;
using System;

namespace SupaStuff.Math
{
    public class PerlinNoise2D : PerlinNoise
    {
        static int noise2(int x, int y, byte seed)
        {
            if (x < 0) x *= -1;
            if (y < 0) y *= -1;
            int tmp = hash[(y + seed) % 256];
            return hash[(tmp + x) % 256];
        }

        static float lin_inter(float x, float y, float s)
        {
            return x + s * (y - x);
        }

        static float smooth_inter(float x, float y, float s)
        {
            return lin_inter(x, y, s * s * (3 - 2 * s));
        }

        static float noise2d(float x, float y, byte seed)
        {
            int x_int = (int)System.Math.Floor(x);
            int y_int = (int)System.Math.Floor(y);
            float x_frac = x - x_int;
            float y_frac = y - y_int;
            int s = noise2(x_int, y_int, seed);
            int t = noise2(x_int + 1, y_int, seed);
            int u = noise2(x_int, y_int + 1, seed);
            int v = noise2(x_int + 1, y_int + 1, seed);
            float low = smooth_inter(s, t, x_frac);
            float high = smooth_inter(u, v, x_frac);
            return smooth_inter(low, high, y_frac);
        }

        static float perlin2d(float x, float y, float freq, int depth, byte seed)
        {
            float xa = x * freq;
            float ya = y * freq;

            float amp = 1.0f;
            float fin = 0;
            float div = 0.0f;


            for (int i = 0; i < depth; i++)
            {
                div += 256 * amp;
                fin += noise2d(xa, ya, seed) * amp;
                amp /= 2;
                xa *= 2;
                ya *= 2;
            }

            return fin / div;
        }
        public PerlinNoise2D(byte seed, float frequency = 1f, int depth = 1) : base(seed, frequency, depth)
        {
        }
        public PerlinNoise2D(float frequency = 1f, int depth = 1) : base(RandByte(), frequency, depth)
        {
        }
        public float Get(float x, float y)
        {
            return perlin2d(x, y, Frequency, Depth, Seed);
        }
        public static float Get(float x, float y, float frequency = 1f, int depth = 1, byte seed = 0)
        {
            return perlin2d(x, y, 0.1f, 4, seed);
        }
    }
}