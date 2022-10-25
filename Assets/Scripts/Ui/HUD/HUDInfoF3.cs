using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDInfoF3 : MonoBehaviour
{
    static HUDInfoF3 main;

    [SerializeField]
    TranslaterComp TextCompGalaxy;
    [SerializeField]
    TranslaterComp TextCompPlanet;
    [SerializeField]
    TranslaterComp TextCompZone;
    [SerializeField]
    TranslaterComp TextCompPlanetsCount;

    string KeyPosGalaxy = "InfoPosGalaxy";
    string KeyPosPlanet = "InfoPosPlanet";
    string KeyPosZone = "InfoPosZone";
    string KeyPosPlanetsCount = "InfoPlanetsCount";

    string ZoneGlobalOld = "";
    bool NeedUpdatePlanets = true;

    // Start is called before the first frame updat

    void Start()
    {
        Inicialize();
    }

    public void Inicialize() {
        main = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePos();
    }

    public static void reActiveF3() {
        if (!PlayerCTRL.local)
        {
            main.gameObject.SetActive(false);
            return;
        }


        main.gameObject.SetActive(!main.gameObject.activeSelf);
    }
    public static void Close() {
        main.gameObject.SetActive(true);
    }

    void UpdatePos() {

        UpdateGalaxy();
        UpdateCellOrPlanet();
        UpdateGlobalZone();

        UpdateCountPlanets();

        //Отобразить позицию в галактике
        void UpdateGalaxy() {
            Vector3 GalaxyPos = PlayerCTRL.local.NumCell + PlayerCTRL.local.PosInCell / 1000000;
            Language.SetTextFromKey(KeyPosGalaxy, "Galaxy: " + GalaxyPos);
            TextCompGalaxy.UpdateText();
        }

        //Отобразить позицию в ячейке или на планете
        void UpdateCellOrPlanet() {
            if (!PlayerCTRL.local)
                return;

            string textStr = "-----";

            //Показать координату на планете
            if (PlayerCTRL.local.PlanetCurrent != null)
            {
                textStr = "Planet: " + (PlayerCTRL.local.NumZone * 1000 + PlayerCTRL.local.transform.position);
            }
            else {
                //Показать координату в космической ячейке
                string xStr = GetNumStr(PlayerCTRL.local.NumZone.x);
                string yStr = GetNumStr(PlayerCTRL.local.NumZone.y);
                string zStr = GetNumStr(PlayerCTRL.local.NumZone.z);

                string xLocal = PlayerCTRL.local.transform.position.x.ToString();
                string yLocal = PlayerCTRL.local.transform.position.y.ToString();
                string zLocal = PlayerCTRL.local.transform.position.z.ToString();

                if (xLocal.Length >= 5)
                    xLocal = xLocal.Substring(0,5);
                if (yLocal.Length >= 5)
                    yLocal = yLocal.Substring(0, 5);
                if (zLocal.Length >= 5)
                    zLocal = zLocal.Substring(0, 5);

                xStr += xLocal;
                yStr += yLocal;
                zStr += zLocal;

                textStr = "Cell: " + xStr + " " + yStr + " " + zStr;
            }

            Language.SetTextFromKey(KeyPosPlanet, textStr);
            TextCompPlanet.UpdateText();

            string GetNumStr(int num) {
                string result = "";

                string numStr = "" + num;
                for (int x = 0; x < 6 - numStr.Length ; x++) {
                    result += " ";
                }

                result += num;

                return result;
            }
        }

        //Отобразить зону игрока
        void UpdateGlobalZone() {
            if (ZoneGlobalOld == PlayerCTRL.local.posGlobal)
                return;

            ZoneGlobalOld = PlayerCTRL.local.posGlobal;
            Language.SetTextFromKey(KeyPosZone, "Zone: " + ZoneGlobalOld);
            TextCompZone.UpdateText();

            NeedUpdatePlanets = true;
        }

        void UpdateCountPlanets() {
            if (!NeedUpdatePlanets || GalaxyCtrl.galaxy == null || GalaxyCtrl.galaxy.cells == null ||
                PlayerCTRL.local.NumCell.x < 0 ||
                PlayerCTRL.local.NumCell.y < 0 ||
                PlayerCTRL.local.NumCell.z < 0 ||
                PlayerCTRL.local.NumCell.x >= GalaxyCtrl.galaxy.cells.GetLength(0) ||
                PlayerCTRL.local.NumCell.y >= GalaxyCtrl.galaxy.cells.GetLength(1) ||
                PlayerCTRL.local.NumCell.z >= GalaxyCtrl.galaxy.cells.GetLength(2))
                return;

            int planetsSum = 0;
            int moonsSum = 0;

            if (GalaxyCtrl.galaxy.cells[PlayerCTRL.local.NumCell.x, PlayerCTRL.local.NumCell.y, PlayerCTRL.local.NumCell.z].mainObjs != null)
                foreach (SpaceObjData planetData in GalaxyCtrl.galaxy.cells[PlayerCTRL.local.NumCell.x, PlayerCTRL.local.NumCell.y, PlayerCTRL.local.NumCell.z].mainObjs.childs)
                {
                    planetsSum++;

                    CalcMoons(planetData);
                }

            Language.SetTextFromKey(KeyPosPlanetsCount, "Planets: " + planetsSum + " Moons: " + moonsSum);
            TextCompPlanetsCount.UpdateText();

            //Посчитать в рекурсии сколько лун
            void CalcMoons(SpaceObjData objData) {

                if (objData.childs == null || objData.childs.Length <= 0)
                    return;

                foreach (SpaceObjData moonData in objData.childs) {
                    moonsSum++;

                    CalcMoons(moonData);
                }
            }
        }
    }
}
