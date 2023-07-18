using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace �osmos
{
    public class PlanetData : ObjData
    {
        HeightMap[] heightMaps;

        public PlanetData(CellS cell)
        {
            this.cell = cell;
        }

        public override void GenData(ObjData parent, float perlin) {
            base.GenData(parent, perlin);

            GenHeightMap();

            void GenHeightMap()
            {
                //�� ������ ������� ������� ������� ������ ��� ��������� ���������� ������� �������
                int needTextures = (int)size;

                //������� ������ �������
                heightMaps = new HeightMap[needTextures];
            }
        }

        public override float[,] GetHeightMap()
        {
            throw new System.NotImplementedException();
        }

        public override Texture2D GetMainTexture(Size quality)
        {
            throw new System.NotImplementedException();
        }
    }
}
