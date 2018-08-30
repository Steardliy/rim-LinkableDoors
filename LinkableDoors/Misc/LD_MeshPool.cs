using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public class MeshPosSet
    {
        public Vector3[] coefficients;
        public Vector3[] offsets;
        public Mesh[] meshes;

        public MeshPosSet() { }
        public MeshPosSet(Vector3[] c, Vector3[] o, Mesh[] m)
        {
            this.coefficients = c;
            this.offsets = o;
            this.meshes = m;
        }
    }

    [StaticConstructorOnStartup]
    public static class LD_MeshPool
    {
        public static readonly Mesh plane10Fill;
        public static readonly Mesh plane10FillHalf;

        public static readonly MeshPosSet[] doorMeshPosSet;

        static LD_MeshPool()
        {
            Vector2[] uvs = new Vector2[4]
            {
                new Vector2(0.06f, 0f),
                new Vector2(0.06f, 1f),
                new Vector2(0.061f, 1f),
                new Vector2(0.061f, 0f)
            };
            LD_MeshPool.plane10FillHalf = LD_MeshPool.NewPlaneMesh(new Vector2(0.57f, 1f), false, uvs);
            LD_MeshPool.plane10Fill = LD_MeshPool.NewPlaneMesh(new Vector2(1.2f, 1f), false, uvs);

            Vector3[] vectors = new Vector3[2]
            {
                new Vector3(0f, 0f, -1f),
                new Vector3(0f, 0f, 1f)
            };
            Vector3[] offsets = new Vector3[6]
            {
                new Vector3(0f, 0.1f, 0.1f),
                new Vector3(0f, 0.1f, -0.1f),
                new Vector3(0f, 0f, 0.5f),
                new Vector3(0f, 0.1f, -0.23f),
                new Vector3(0f, 0f, -0.5f),
                new Vector3(0f, 0.1f, 0.23f)
            };

            LD_MeshPool.doorMeshPosSet = new MeshPosSet[]
            {
                new MeshPosSet(
                    new Vector3[] { vectors[0], default(Vector3) },
                    new Vector3[] { offsets[0], default(Vector3) },
                    new Mesh[] { LD_MeshPool.plane10Fill, null }),
                new MeshPosSet(
                    new Vector3[] { vectors[1], default(Vector3) },
                    new Vector3[] { offsets[1], default(Vector3) },
                    new Mesh[] { LD_MeshPool.plane10Fill, null }),
                new MeshPosSet(
                    new Vector3[] { vectors[0], vectors[0] },
                    new Vector3[] { offsets[2], offsets[3] },
                    new Mesh[] { MeshPool.plane10, LD_MeshPool.plane10FillHalf }),
                new MeshPosSet(
                    new Vector3[] { vectors[1], vectors[1] },
                    new Vector3[] { offsets[4], offsets[5] },
                    new Mesh[] { MeshPool.plane10Flip, LD_MeshPool.plane10FillHalf }),
                new MeshPosSet(
                    new Vector3[] { vectors[0], vectors[1] },
                    new Vector3[] { default(Vector3), default(Vector3) },
                    new Mesh[] { MeshPool.plane10, MeshPool.plane10Flip })
            };
        }

        static Mesh NewPlaneMesh(Vector2 size, bool flipped, Vector2[] uvs = null)
        {
            Vector3[] array = new Vector3[4];
            array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
            array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
            array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
            array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);
            
            Vector2[] array2 = uvs;
            if (array2 == null)
            {
                array2 = new Vector2[4];
                if (!flipped)
                {
                    array2[0] = new Vector2(0f, 0f);
                    array2[1] = new Vector2(0f, 1f);
                    array2[2] = new Vector2(1f, 1f);
                    array2[3] = new Vector2(1f, 0f);
                }
                else
                {
                    array2[0] = new Vector2(1f, 0f);
                    array2[1] = new Vector2(1f, 1f);
                    array2[2] = new Vector2(0f, 1f);
                    array2[3] = new Vector2(0f, 0f);
                }
            }

            int[] array3 = new int[6];
            array3[0] = 0;
            array3[1] = 1;
            array3[2] = 2;
            array3[3] = 0;
            array3[4] = 2;
            array3[5] = 3;
            Mesh mesh = new Mesh();
            mesh.name = "NewPlaneMesh()";
            mesh.vertices = array;
            mesh.uv = array2;
            mesh.SetTriangles(array3, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
