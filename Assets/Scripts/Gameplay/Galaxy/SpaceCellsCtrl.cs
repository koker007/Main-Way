using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//отвечает за создание звезд внутри космической ячейки
public class SpaceCellsCtrl : MonoBehaviour
{
    public static SpaceCellsCtrl main;

    [SerializeField]
    SpaceObjCtrl SpaceObjPrefab;

    //Список созданных объектов
    List<SpaceObjCtrl> spaceObjList = new List<SpaceObjCtrl>();

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }
    // Update is called once per frame
    void Update()
    {
        testVisualize();
        UpdateTransformPlayer();
    }

    public void testVisualize() {
        //Перебираем ячейки вокруг позиции в космосе
        //Получаем позицию камеры в ячейках

        //Визуализация всегда относительно локального игрока
        Vector3 camPos;
        //Если есть игрок берем, его позицию
        if (PlayerCTRL.local) {
            camPos =  PlayerCTRL.local.NumCell + (PlayerCTRL.local.PosInCell / CellS.size);
        }
        //Если игрока нет берем позицию из координат контроллера камеры
        else
        {
            camPos = SpaceCameraControl.main.spacePosCell;
        }

        int xMax = GalaxyCtrl.galaxy.cells.GetLength(0);
        int yMax = GalaxyCtrl.galaxy.cells.GetLength(1);
        int zMax = GalaxyCtrl.galaxy.cells.GetLength(2);

        //проверяем ячейки в радиусе 3-х клеток
        int radius = Settings.main.distance;
        for (int x = -radius; x < radius; x++){
            int xfact = (int)camPos.x + x;
            if (xfact < 0 || xfact >= xMax)
                continue;

            for (int y = -radius; y < radius; y++){
                int yfact = (int)camPos.y + y;
                if (yfact < 0 || yfact >= yMax)
                    continue;

                for (int z = -radius; z < radius; z++){
                    int zfact = (int)camPos.z + z;
                    if (zfact < 0 || zfact >= zMax)
                        continue;

                    //Визуализируем главную звезду и ее планеты и луны
                    if(GalaxyCtrl.galaxy.cells[xfact, yfact, zfact].visual != null)
                        Visualize(GalaxyCtrl.galaxy.cells[xfact, yfact, zfact].mainObjs, camPos);
                }
            }
        }

        void Visualize(SpaceObjData spaceObj, Vector3 camPos) {
            //выходим если нечего визуализировать
            if (spaceObj == null) 
                return;

            //Если визуализации нет - создаем
            if (spaceObj.visual == null)
            {
                if (canVisualize()) {

                    SpaceObjCtrl spaceObjVisual = GetSpaceObjCTRL();
                    spaceObjVisual.Ini(spaceObj);
                }
            }
            else {
                //Визуализировать нельзя удаляем объект
                if (!canVisualize()) {
                    spaceObj.visual.Deactivate();
                }
            }

            //Если лун нет
            if (spaceObj.childs == null)
                return;

            //Перебираем луны на визуализацию
            for (int i = 0; i < spaceObj.childs.Length; i++)
            {
                Visualize(spaceObj.childs[i], camPos);
            }

            //Можно ли визуализировать этот объект
            bool canVisualize() {
                bool result = false;

                //Если игрок находится внутри ячейки то визуализируем все
                if ((int)camPos.x == (int)spaceObj.cell.pos.x && (int)camPos.y == (int)spaceObj.cell.pos.y && (int)camPos.z == (int)spaceObj.cell.pos.z)
                    return true;

                //Находим растояние от центра звезды до камеры
                float dist = Vector3.Distance(spaceObj.cell.pos, camPos);

                //Если дистанция меньше чем размер звезды
                if (dist < Calc.GetSizeInt(spaceObj.size)/1000f) {
                    return true;
                }


                return result;
            }
        }
    }

    void UpdateTransformPlayer()
    {
        if (!PlayerCTRL.local)
            return;

        //положение в галактике
        Vector3 posInCell = (PlayerCTRL.local.PosInCell / CellS.size);

        gameObject.transform.localPosition = -(PlayerCTRL.local.NumCell + posInCell) * CellS.sizeVisual;

        //повернуть камеру по глобальному вращению игрока
        //gameObject.transform.rotation = PlayerCTRL.local.GetSpaceView();
    }

    //Получить свободный космический объект или создать новый
    SpaceObjCtrl GetSpaceObjCTRL() {
        //Ищем свободный
        foreach (SpaceObjCtrl spaceObj in spaceObjList) {
            //пропускаем, Если этот объект пустой
            if (spaceObj == null)
                continue;

            //Выбираем объект если он не активен
            if (!spaceObj.gameObject.activeSelf) {
                spaceObj.gameObject.SetActive(true);
                return spaceObj;
            }
        }

        //Если мы тут значит объект не был найден
        //Создаем новый
        GameObject spaceObjNew = Instantiate(SpaceObjPrefab.gameObject, transform);
        SpaceObjCtrl result = spaceObjNew.GetComponent<SpaceObjCtrl>();
        spaceObjList.Add(spaceObjNew.GetComponent<SpaceObjCtrl>());

        return result;
    }
}
