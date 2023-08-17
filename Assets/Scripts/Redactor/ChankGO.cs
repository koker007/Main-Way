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
                //Создался новый игровой объект чанка, запихиваем в буфер
                buffer.Add(this);
            }

            /// <summary>
            /// Получить свободный чанк
            /// </summary>
            /// <returns></returns>
            static public ChankGO GetChankGO()
            {
                //Перебираем все чанки, ищем свободный
                foreach (ChankGO chankGO in buffer)
                {
                    if (chankGO.data == null)
                        return chankGO;
                }

                //Необходимо создать новый префаб чанка
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
