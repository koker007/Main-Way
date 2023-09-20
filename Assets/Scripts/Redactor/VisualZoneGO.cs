using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Unity.Jobs;

namespace Game
{
    namespace Space
    {
        /// <summary>
        /// Игровая зона в которой генерируются чанки, каждая зона представляет собой отдельную планету, луну, астеройд и т.д.
        /// </summary>
        public abstract class VisualZoneGO : MonoBehaviour
        {
            //Максимальный размер на одну зону
            const int minizoneSizeMax = 1000;

            static protected List<VisualZoneGO> buffer = new List<VisualZoneGO>();
            int bufferNum = 0;

            protected ObjData data;
            protected ChankGO[][,,] chanks;

            protected Queue<ChankGO> startDrawQueue = new Queue<ChankGO>();
            protected Queue<ChankGO> endDrawQueue = new Queue<ChankGO>();

            //Пространство в котором сейчас находится игрок
            static protected VisualZoneGO now;

            [SerializeField]
            protected GameObject ParentChanks1;
            [SerializeField]
            protected GameObject ParentChanks2;
            [SerializeField]
            protected GameObject ParentChanks4;
            [SerializeField]
            protected GameObject ParentChanks8;
            [SerializeField]
            protected GameObject ParentChanks16;
            [SerializeField]
            protected GameObject ParentChanks32;
            [SerializeField]
            protected GameObject ParentChanks64;
            [SerializeField]
            protected GameObject ParentChanks128;

            abstract public void Inizialize(ObjData data, bool isNow = false);

            /// <summary>
            /// Почистить этот объект для использования другими
            /// </summary>
            virtual public void Clear() {
                data = null;

                if (chanks == null)
                    return;

                //Проходимся по всем чанкам и освобождаем их
                for (int size = 0; size < chanks.Length; size++) {
                    if (chanks[size] == null)
                        continue;

                    for (int x = 0; x < chanks.GetLength(0); x++) {
                        for (int y = 0; y < chanks.GetLength(1); y++) {
                            for (int z = 0; z < chanks.GetLength(2); z++) {
                                if (chanks[size][x, y, z] == null)
                                    continue;

                                chanks[size][x, y, z].Clear();
                                chanks[size][x, y, z] = null;
                            }
                        }
                    }

                    chanks[size] = null;
                }
                
            }

            protected struct JobGenerate : IJob
            {
                VisualZoneGO visualZone;
                Size sizeGenMin;

                public JobGenerate(VisualZoneGO Zone, Size sizeGenMin)
                {
                    this.visualZone = Zone;
                    this.sizeGenMin = sizeGenMin;
                }

                public void Execute()
                {
                    visualZone.JobStartGenerate(sizeGenMin);
                }
            }

            abstract public void JobStartGenerate(Size sizeGenMin);
            abstract public void iniChanks();

            protected void AddReDraw(ChankGO chankGO) {
                startDrawQueue.Enqueue(chankGO);
            }

            void Awake()
            {
                //новая игровая зона, запихиваем в буфер
                buffer.Add(this);
            }


            protected void TransformWorld()
            {
                //Основной мир всегда в нулевых координатах c размером 1к1
                if (now == this)
                {
                    transform.position = new Vector3();
                    transform.localScale = new Vector3(1,1,1);
                    return;
                }

                //иначе
                //Уменьшаем зону в размерах как минимум в 10 раз
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                //выбираем позицию для этой области на основе номера в буфере
                //Номер в буфере
                GetBufferNum();

                Vector3 position = new Vector3(0, -5000, 0);

                //Справа
                if (bufferNum % 2 == 0)
                    position.x = (bufferNum / 2 + 1) * minizoneSizeMax;
                //Слева
                else position.x = (-bufferNum / 2 + 1) * minizoneSizeMax;

                transform.position = position;
            }

            int GetBufferNum() {
                //Если номер меньше размера буфера и указывает на эту зону, то все правильно
                if (bufferNum < buffer.Count && buffer[bufferNum] == this)
                    return bufferNum;

                //Иначе перебираем все элементы пока не найдем искомое
                for (int num = 0; num < buffer.Count; num++) {
                    if (buffer[num] != this)
                        continue;

                    //нашелся текущий
                    bufferNum = num;
                    break;
                }

                return bufferNum;
            }

            void FixedReDraw() {
                //Запуск отрисовки чанков
                if (startDrawQueue.Count == 0)
                    return;

                ChankGO chankGO = startDrawQueue.Dequeue();
                chankGO.StartReDrawAll();
                endDrawQueue.Enqueue(chankGO);
            }

            //Завершение отрисовки чанков
            void LateReDraw() {
                if (endDrawQueue.Count == 0)
                    return;

                ChankGO chankGO = endDrawQueue.Peek();
                if(chankGO.LateReDrawAwait())
                    endDrawQueue.Dequeue();
            }

            
            private void FixedUpdate()
            {
                FixedReDraw();
            }
            private void LateUpdate()
            {
                LateReDraw();
            }
        }
    }
}
