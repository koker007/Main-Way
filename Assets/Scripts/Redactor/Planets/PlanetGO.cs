using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace Game
{
    namespace Space
    {
        public class PlanetGO : VisualZoneGO
        {
            PlanetData data;

            static public PlanetGO GetPlanetGO()
            {
                //Перебираем все чанки, ищем свободный
                foreach (VisualZoneGO visualZone in buffer)
                {
                    PlanetGO planetGO = visualZone as PlanetGO;

                    if (planetGO.data == null)
                        return planetGO;
                }

                //Необходимо создать новый префаб чанка
                PlanetGO planetNew = Instantiate(GameData.main.prefabPlanetGO);
                return planetNew;
            }

            public override void Inizialize(PlanetData data, bool isNow = false)
            {
                this.data = data;

                if (isNow)
                    now = this;

                TransformWorld();
            }

            // Start is called before the first frame update
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
