using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace Game
{
    namespace Space
    {
        /// <summary>
        /// »грова€ зона в которой генерируютс€ чанки, кажда€ зона представл€ет собой отдельную планету, луну, астеройд и т.д.
        /// </summary>
        public abstract class VisualZoneGO : MonoBehaviour
        {
            //ћаксимальный размер на одну зону
            const int minizoneSizeMax = 1000;

            static protected List<VisualZoneGO> buffer = new List<VisualZoneGO>();
            int bufferNum = 0;

            protected ChankGO[][,,] chanks;

            //ѕространство в котором сейчас находитс€ игрок
            static protected VisualZoneGO now;

            abstract public void Inizialize(PlanetData data, bool isNow = false);

            void Awake()
            {
                //нова€ игрова€ зона, запихиваем в буфер
                buffer.Add(this);
            }

            protected void TransformWorld()
            {
                //ќсновной мир всегда в нулевых координатах
                if (now == this)
                {
                    transform.position = new Vector3();
                    transform.localScale = new Vector3(1,1,1);
                    return;
                }

                //иначе
                //”меньшаем зону в размерах как минимум в 10 раз
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                //выбираем позицию дл€ этой области на основе номера в буфере
                //Ќомер в буфере
                GetBufferNum();

                Vector3 position = new Vector3(0, -5000, 0);

                
            }

            int GetBufferNum() {
                //≈сли номер меньше размера буфера и указывает на эту зону, то все правильно
                if (bufferNum < buffer.Count && buffer[bufferNum] == this)
                    return bufferNum;

                //»наче перебираем все элементы пока не найдем искомое
                for (int num = 0; num < buffer.Count; num++) {
                    if (buffer[num] != this)
                        continue;

                    //нашелс€ текущий
                    bufferNum = num;
                    break;
                }

                return bufferNum;
            }
        }
    }
}
