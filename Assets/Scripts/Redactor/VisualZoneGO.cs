using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace Game
{
    namespace Space
    {
        /// <summary>
        /// ������� ���� � ������� ������������ �����, ������ ���� ������������ ����� ��������� �������, ����, �������� � �.�.
        /// </summary>
        public abstract class VisualZoneGO : MonoBehaviour
        {
            //������������ ������ �� ���� ����
            const int minizoneSizeMax = 1000;

            static protected List<VisualZoneGO> buffer = new List<VisualZoneGO>();
            int bufferNum = 0;

            protected ChankGO[][,,] chanks;

            //������������ � ������� ������ ��������� �����
            static protected VisualZoneGO now;

            abstract public void Inizialize(PlanetData data, bool isNow = false);

            void Awake()
            {
                //����� ������� ����, ���������� � �����
                buffer.Add(this);
            }

            protected void TransformWorld()
            {
                //�������� ��� ������ � ������� �����������
                if (now == this)
                {
                    transform.position = new Vector3();
                    transform.localScale = new Vector3(1,1,1);
                    return;
                }

                //�����
                //��������� ���� � �������� ��� ������� � 10 ���
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                //�������� ������� ��� ���� ������� �� ������ ������ � ������
                //����� � ������
                GetBufferNum();

                Vector3 position = new Vector3(0, -5000, 0);

                
            }

            int GetBufferNum() {
                //���� ����� ������ ������� ������ � ��������� �� ��� ����, �� ��� ���������
                if (bufferNum < buffer.Count && buffer[bufferNum] == this)
                    return bufferNum;

                //����� ���������� ��� �������� ���� �� ������ �������
                for (int num = 0; num < buffer.Count; num++) {
                    if (buffer[num] != this)
                        continue;

                    //������� �������
                    bufferNum = num;
                    break;
                }

                return bufferNum;
            }
        }
    }
}
