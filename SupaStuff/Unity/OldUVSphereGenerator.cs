using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SupaStuff.Unity
{
    [UnitySpecific]
    public class OldUVSphereGenerator
    {
        public int rings;
        public int segments;
        public float[][] grid;
        public int[][] indices;
        public Mesh mesh;
        public Vector3[] vertices;
        public int vertsIndex;
        public int[] triangles;
        public int trianglesIndex;
        public Vector2[] uvs;
        public int uvindex;
        public UVSphereGenerator(float[][] grid, Vector3 center)
        {
            mesh = new Mesh();
            rings = grid.Length;
            segments = grid[0].Length;
            indices = new int[rings][];
            trianglesIndex = 0;
            uvindex = 0;
            vertsIndex = 0;
            for (int i = 0; i < rings; i++)
            {
                indices[i] = new int[segments];
            }
            int index = 0;
            vertices = new Vector3[rings * segments * 2];
            int _rings = rings + 1;
            int rounds = 0;
            for (int ring = 1; ring < _rings; ring++)
            {
                for (int segment = 0; segment < segments; segment++)
                {
                    float x = Mathf.Sin(Mathf.PI * ring / _rings) * Mathf.Cos(2 * Mathf.PI * segment / segments);
                    float y = Mathf.Sin(Mathf.PI * ring / _rings) * Mathf.Sin(2 * Mathf.PI * segment / segments);
                    float z = Mathf.Cos(Mathf.PI * ring / _rings);
                    vertices[index++] = new Vector3(x, y, z) * grid[ring - 1][segment];
                    indices[ring - 1][segment] = index - 1;
                    rounds++;
                }
            }
            for (int x = 0; x < rings - 1; x++)
            {
                for (int y = 0; y < segments; y++)
                {
                    if (x == rings - 1)
                    {
                        /*
                        triangles.Add(indices[x][y]);
                        triangles.Add(indices[0][y]);
                        triangles.Add(indices[0][0]);
                        triangles.Add(indices[0][0]);
                        triangles.Add(indices[x][0]);
                        triangles.Add(indices[x][y]);
                        */
                    }
                    else if (y == segments - 1)
                    {
                        AddTriangle(indices[x][y]);
                        AddTriangle(indices[x + 1][y]);
                        AddTriangle(indices[x + 1][0]);

                        AddTriangle(indices[x + 1][0]);
                        AddTriangle(indices[x][0]);
                        AddTriangle(indices[x][y]);

                    }
                    else
                    {
                        AddTriangle(indices[x][y]);
                        AddTriangle(indices[x + 1][y]);
                        AddTriangle(indices[x + 1][y + 1]);
                        AddTriangle(indices[x + 1][y + 1]);
                        AddTriangle(indices[x][y + 1]);
                        AddTriangle(indices[x][y]);
                    }
                }
            }
            float bottomRad = averageOf(grid[0]);
            float topRad = averageOf(grid[rings - 1]);
            AddVert(new Vector3(0, 0, bottomRad));
            AddUv(new Vector2(0, 0));
            AddUv(new Vector2(0, 0));
            AddVert(new Vector3(0, 0, -topRad));
            for (int i = 0; i < segments; i++)
            {
                if (i == segments - 1)
                {
                    AddTriangle(indices[0][i]);
                    AddTriangle(indices[0][0]);
                    AddTriangle(vertsIndex - 2);

                    AddTriangle(vertsIndex - 1);
                    AddTriangle(indices[rings - 1][0]);
                    AddTriangle(indices[rings - 1][i]);
                }
                else
                {
                    AddTriangle(indices[0][i]);
                    AddTriangle(indices[0][i + 1]);
                    AddTriangle(vertsIndex - 2);

                    AddTriangle(vertsIndex - 1);
                    AddTriangle(indices[rings - 1][i + 1]);
                    AddTriangle(indices[rings - 1][i]);
                }
            }
            Flush();
        }
        public static string vecToString(Vector3 vector)
        {
            return (vector.x + " , " + vector.y + " , " + vector.z);
        }
        new public byte[] bytify()
        {
            return new byte[0];
        }
        public float averageOf(float[] array)
        {
            float sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum / array.Length;
        }
        public void Flush()
        {
            
        }
        void AddTriangle(int index)
        {
            triangles[trianglesIndex++] = index;
        }
        void AddUv(Vector2 uv)
        {
            uvs[uvindex++] = uv;
        }
        int AddVert(Vector3 vert)
        {
            vertices[vertsIndex] = vert;
            return vertsIndex++;
        }
    }
}
