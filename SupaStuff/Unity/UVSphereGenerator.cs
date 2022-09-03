using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace SupaStuff.Unity
{
    [UnitySpecific]
    public class UVSphereGenerator
    {
        public Mesh MakeMesh(float radius,int rings,int segments)
        {
            Mesh mesh = new Mesh();

            return mesh;
        }
        public Mesh MakeMesh(float[][] grid)
        {
            Mesh mesh = new Mesh();
            int[] triangles;
            Vector3[] verts;
            int triangleindex;
            int vertsindex;
            int rings = grid.Length - 2;
            int segments = grid[1].Length;
            for(int i = 0;i < rings;i++)
            {

            }
            return mesh;
        }
    }
}
