using System;
using System.Collections.Generic;
using System.Text;
using SupaStuff.Math;
using UnityEngine;
//https://schneide.blog/2016/07/15/generating-an-icosphere-in-c/
namespace SupaStuff.Unity
{
    internal class IcosphereGenerator
    {
        public Mesh mesh;
        private Vector3[] vertices;
        private int[] triangles;
        public readonly int radius;

        public static Mesh StarterSphere
        {
            get
            {
                const float X = .525731112119133606f;
                const float Z = .850650808352039932f;
                const float N = 0f;
                Mesh mesh = new Mesh();
                mesh.vertices = new Vector3[]
                {
                        new Vector3(-X,N,Z),new Vector3(X,N,Z),new Vector3(-X,N,-Z),new Vector3(X,N,-Z),
                        new Vector3(N,Z,X),new Vector3(N,Z,-X),new Vector3(N,-Z,X),new Vector3(N,-Z,-X),
                        new Vector3(Z,X,N),new Vector3(-Z,X,N),new Vector3(Z,-X,N),new Vector3(-Z,-X,N)
                };
                mesh.triangles = new int[]
                {
                        0,4,1,  0,9,4,  9,5,4,  4,5,8,  4,8,1,
                        8,10,1,  8,3,10,  5,3,8,  5,2,3,  2,7,3,
                        7,10,3,  7,6,10,  7,11,6,  11,0,6,  0,1,6,
                        6,1,10,  9,0,11,  9,11,2,  9,2,5,  7,2,11
                };
                return mesh;
            }
        }


        public void Subdivide(int times)
        {
            for (int i = 0; i < times; i++)
            {
                Vector3[] newverts = new Vector3[vertices.Length * 2];
                int[] newTriangles = new int[triangles.Length * 4];
                for (int j = 0; j < triangles.Length / 3; j++)
                {
                    newverts[j * 6] = vertices[j * 3];
                    newverts[j * 6 + 1] = vertices[j * 3 + 1];
                    newverts[j * 6 + 2] = vertices[j * 3 + 2];

                    newverts[j * 6 + 3] = SupaMath.Average(newverts[j * 6], newverts[j * 6 + 1]).normalized * radius;
                    newverts[j * 6 + 4] = SupaMath.Average(newverts[j * 6 + 1], newverts[j * 6 + 2]).normalized * radius;
                    newverts[j * 6 + 5] = SupaMath.Average(newverts[j * 6 + 2], newverts[j * 6]).normalized * radius;


                    // triangles between: 
                    // v1,nv1,nv3
                    // v2,nv2,nv1
                    // v3,nv3,nv2
                    // nv1,nv2,nv3
                    // Total: 12 points for triangles

                    newTriangles[j * 12] = j * 6;
                    newTriangles[j * 12 + 1] = j * 6 + 3;
                    newTriangles[j * 12 + 2] = j * 6 + 5;

                    newTriangles[j * 12 + 3] = j * 6 + 1;
                    newTriangles[j * 12 + 4] = j * 6 + 4;
                    newTriangles[j * 12 + 5] = j * 6 + 3;

                    newTriangles[j * 12 + 6] = j * 6 + 2;
                    newTriangles[j * 12 + 7] = j * 6 + 5;
                    newTriangles[j * 12 + 8] = j * 6 + 4;

                    newTriangles[j * 12 + 9] = j * 6 + 3;
                    newTriangles[j * 12 + 10] = j * 6 + 4;
                    newTriangles[j * 12 + 11] = j * 6 + 5;
                    triangles = newTriangles;
                    vertices = newverts;
                }
                mesh.vertices = vertices;
                mesh.triangles = triangles;
            }
        }



        public IcosphereGenerator(int radius)
        {
            this.radius = radius;
            const float X = .525731112119133606f;
            const float Z = .850650808352039932f;
            const float N = 0f;
            mesh = new Mesh();
            vertices = new Vector3[]
            {
                        new Vector3(-X,N,Z),new Vector3(X,N,Z),new Vector3(-X,N,-Z),new Vector3(X,N,-Z),
                        new Vector3(N,Z,X),new Vector3(N,Z,-X),new Vector3(N,-Z,X),new Vector3(N,-Z,-X),
                        new Vector3(Z,X,N),new Vector3(-Z,X,N),new Vector3(Z,-X,N),new Vector3(-Z,-X,N)
            };
            triangles = new int[]
            {
                        0,4,1,  0,9,4,  9,5,4,  4,5,8,  4,8,1,
                        8,10,1,  8,3,10,  5,3,8,  5,2,3,  2,7,3,
                        7,10,3,  7,6,10,  7,11,6,  11,0,6,  0,1,6,
                        6,1,10,  9,0,11,  9,11,2,  9,2,5,  7,2,11
            };
            mesh.vertices = vertices;
            mesh.triangles = triangles;

        }
    }
}
