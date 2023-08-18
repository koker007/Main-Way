using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    namespace Space
    {
        public class ChankGO : MonoBehaviour
        {
            static List<ChankGO> buffer = new List<ChankGO>();

            Chank data;

            public void Awake()
            {
                //�������� ����� ������� ������ �����, ���������� � �����
                buffer.Add(this);
            }

            /// <summary>
            /// �������� ��������� ����
            /// </summary>
            /// <returns></returns>
            static public ChankGO GetChankGO()
            {
                //���������� ��� �����, ���� ���������
                foreach (ChankGO chankGO in buffer)
                {
                    if (chankGO.data == null)
                        return chankGO;
                }

                //���������� ������� ����� ������ �����
                ChankGO chankNew = Instantiate(GameData.main.prefabChankGO);
                return chankNew;
            }

            public void Inicialize(Chank chank) {
                data = chank;
                //������ ������ �� ������ �����
                int sizeOneBlock = Calc.GetSizeInt(chank.sizeBlock);

                transform.localScale = new Vector3(sizeOneBlock, sizeOneBlock, sizeOneBlock);
            }
            /// <summary>
            /// �������� ���� ��� ����������� �������������
            /// </summary>
            public void Clear() {
                data = null;

                transform.localPosition = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(1, 1, 1);
            }

            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
