using System;
using System.Collections.Generic;
using System.Text;
using SupaStuff.Math;
using UnityEngine;
//https://schneide.blog/2016/07/15/generating-an-icosphere-in-c/
namespace SupaStuff.Unity
{
    public class IcosphereGenerator
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
                int vertIndex = 0;
                int[] newTriangles = new int[triangles.Length * 4];
                int reps = triangles.Length / 3;
                Console.WriteLine($"{vertices.Length} : {newverts.Length} : {triangles.Length} : {newTriangles.Length}");
                for (int j = 0; j < reps; j++)
                {
                    Console.WriteLine("J = " + j);
                    triangles[j * 3] = triangles[j * 3];
                    vertices[triangles[j * 3]] = vertices[triangles[j * 3]];
                    int v1index = IndexOfV(vertices[triangles[j * 3]]);
                    if(v1index == -1)
                    {
                        v1index = vertIndex;
                        newverts[vertIndex] = vertices[triangles[j * 3]];
                    }
                    int v2index = IndexOfV(vertices[triangles[j * 3]]);
                    if (v2index == -1)
                    {
                        v2index = vertIndex;
                        newverts[vertIndex++] = vertices[triangles[j * 3 + 1]];
                    }
                    int v3index = IndexOfV(vertices[triangles[j * 3]]);
                    if (v3index == -1)
                    {
                        v3index = vertIndex;
                        newverts[vertIndex++] = vertices[triangles[j * 3 + 2]];
                    }
                    Vector3 v4 = SupaMath.Average(newverts[v1index], newverts[v2index]).normalized * radius;
                    int v4index = IndexOfV(v4);
                    if(v4index == -1)
                    {
                        v4index = vertIndex;
                        newverts[vertIndex++] = v4;
                    }
                    Vector3 v5 = SupaMath.Average(newverts[v2index], newverts[v3index]).normalized * radius;
                    int v5index = IndexOfV(v5);
                    if (v5index == -1)
                    {
                        v5index = vertIndex;
                        newverts[vertIndex++] = v5;
                    }
                    Vector3 v6 = SupaMath.Average(newverts[v3index], newverts[v1index]).normalized * radius;
                    int v6index = IndexOfV(v6);
                    if (v6index == -1)
                    {
                        v6index = vertIndex;
                        newverts[vertIndex++] = v6;
                    }


                    // triangles between: 
                    // v1,nv1,nv3
                    // v2,nv2,nv1
                    // v3,nv3,nv2
                    // nv1,nv2,nv3
                    // Total: 12 points for triangles

                    newTriangles[j * 12] = v1index;
                    newTriangles[j * 12 + 1] = v4index;
                    newTriangles[j * 12 + 2] = v6index;

                    newTriangles[j * 12 + 3] = v2index;
                    newTriangles[j * 12 + 4] = v5index;
                    newTriangles[j * 12 + 5] = v4index;

                    newTriangles[j * 12 + 6] = v3index;
                    newTriangles[j * 12 + 7] = v6index;
                    newTriangles[j * 12 + 8] = v5index;

                    newTriangles[j * 12 + 9] = j * 6 + 3;
                    newTriangles[j * 12 + 10] = j * 6 + 4;
                    newTriangles[j * 12 + 11] = j * 6 + 5;
                }
                triangles = newTriangles;
                vertices = newverts;
                int IndexOfI(int val)
                {
                    return SupaMath.IndexOf(newTriangles, val);
                }
                int IndexOfV(Vector3 val)
                {
                    return SupaMath.IndexOf(newverts, val);
                }
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
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

            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= radius;
            }
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
