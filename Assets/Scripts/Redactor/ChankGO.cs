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
