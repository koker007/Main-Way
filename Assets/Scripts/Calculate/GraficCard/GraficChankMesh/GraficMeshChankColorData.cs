using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grafic
{
    namespace Data
    {
        public class MeshChankColor
        {
            public bool calculated = false;

            //������� ���������� ������

            //���������� ������ 32*32*32
            //3 ���������� ������ � ������� (����, ���, ������)
            //2 ���������� ������������� �� ������ �������
            //3 ���������� ������ � ������������

            //���������� ������
            const int verticesCount = 32 * 32 * 32 * 4 * 3; //4 ������� �� 3 ������� �������� ���� ��������
            //���������� ������ ������������� 32 * 32 * 32 ���� �� 3 ������� �� ������ � ������ �� 2 ������������ � 3 ���������
            const int trianglesCount = 32 * 32 * 32 * 3 * 2 * 3;

            const int sizeXTextureColor = 180;
            const int sizeYTextureColor = 200;//183;

            public const int sizeOfVertices = sizeof(float) * 3; //vector3
            public const int sizeOfTriangles = sizeof(int);
            public const int sizeOfNormals = sizeof(float) * 3;
            public const int sizeOfUV = sizeof(float) * 2;

            //�esh
            public Vector3[] vertices;
            public int[] triangles;
            public Vector3[] normals;
            public Vector2[] uv;
            public Vector2[] uv2;

            public Texture2D mainTexture;

            public MeshChankColor()
            {
                vertices = new Vector3[verticesCount];
                uv = new Vector2[verticesCount];
                uv2 = new Vector2[verticesCount];
                triangles = new int[trianglesCount];
                normals = new Vector3[verticesCount];
                //mainTexture = new RenderTexture(sizeXTextureColor, sizeYTextureColor, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                mainTexture = new Texture2D(sizeXTextureColor, sizeYTextureColor, TextureFormat.RGBA32, false);
                mainTexture.filterMode = FilterMode.Point;
                mainTexture.Apply();
            }
        }
    }
}
