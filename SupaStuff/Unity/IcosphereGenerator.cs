#define UseMesh
//#undef UseMesh
using System;
using System.Collections.Generic;
using System.Text;
using SupaStuff.Math;
using UnityEngine;
//https://schneide.blog/2016/07/15/generating-an-icosphere-in-c/

namespace SupaStuff.Unity
{
    [UnitySpecific]
    public class IcosphereGenerator
    {
#if UseMesh
        public Mesh mesh;
#endif
        public Vector3[] vertices { get;private set; }
        public int[] triangles { get; private set; }
        public readonly int radius;

        public static Mesh StarterSphere
        {
            get
            {
                // Values for ease of copying & pasting
                const float X = .525731112119133606f;
                const float Z = .850650808352039932f;
                const float N = 0f;
                Mesh mesh = new Mesh();

                // These are the starting triangles, which later get subdivided

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
                List<Vector3> newvertices = new List<Vector3>();
                List<int> newtriangles = new List<int>();

                int numtriangles = triangles.Length / 3;

                for(int tri = 0; tri < numtriangles; tri++)
                {
                    #region vertices
                    Vector3 v1 = vertices[triangles[tri * 3]];
                    Vector3 v2 = vertices[triangles[(tri * 3)+1]];
                    Vector3 v3 = vertices[triangles[(tri * 3) + 2]];

                    Vector3 v4 = SupaMath.Average(v1, v2);
                    Vector3 v5 = SupaMath.Average(v2, v3);
                    Vector3 v6 = SupaMath.Average(v3, v1);
                    #endregion

                    int i1 = newvertices.IndexOf(v1);
                    if(i1 == -1)
                    {
                        i1 = newvertices.Count;
                        newvertices.Add(v1);
                    }
                    #region repeat

                    int i2 = newvertices.IndexOf(v2);
                    if (i1 == -1)
                    {
                        i2 = newvertices.Count;
                        newvertices.Add(v2);
                    }

                    int i3 = newvertices.IndexOf(v3);
                    if (i3 == -1)
                    {
                        i3 = newvertices.Count;
                        newvertices.Add(v3);
                    }

                    int i4 = newvertices.IndexOf(v4);
                    if (i4 == -1)
                    {
                        i4 = newvertices.Count;
                        newvertices.Add(v4);
                    }

                    int i5 = newvertices.IndexOf(v5);
                    if (i5 == -1)
                    {
                        i5 = newvertices.Count;
                        newvertices.Add(v5);
                    }

                    int i6 = newvertices.IndexOf(v6);
                    if (i6 == -1)
                    {
                        i6 = newvertices.Count;
                        newvertices.Add(v6);
                    }
                    #endregion

                    #region triangles

                    newtriangles.Add(i1);
                    newtriangles.Add(i6);
                    newtriangles.Add(i4);

                    newtriangles.Add(i4);
                    newtriangles.Add(i5);
                    newtriangles.Add(i2);

                    newtriangles.Add(i6);
                    newtriangles.Add(i3);
                    newtriangles.Add(i5);

                    newtriangles.Add(i6);
                    newtriangles.Add(i5);
                    newtriangles.Add(i4);

                    #endregion
                }
                vertices = newvertices.ToArray();
                triangles = newtriangles.ToArray();
#if UseMesh
                Debug.Log($"Triangles: {triangles.Length}, vertices: {vertices.Length}");
#endif
            }
            // Update the mesh
#if UseMesh
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            Debug.Log($"Final: Triangles: {mesh.triangles.Length}, Vertices: {mesh.vertices.Length}");
#endif

        }
        


        public IcosphereGenerator(int radius)
        {
            this.radius = radius;
            const float X = .525731112119133606f;
            const float Z = .850650808352039932f;
            const float N = 0f;
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

#if UseMesh
            mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
#endif

        }
    }
}
