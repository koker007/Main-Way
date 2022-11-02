using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Отвечает за редактирование коллайдеров в редакторе блоков
public class RedactorBlocksPhysicsCollider : MonoBehaviour
{
    //Зона коллайдеров
    [SerializeField]
    SliderCTRL collidersMax; //Сколько максимум коллайдеров
    [SerializeField]
    SliderCTRL colliderSelect; //Какой коллайдер выбран сейчас
    [SerializeField]
    SliderCTRL zonePosX; //Какая стартовая позиция у коллайдера
    [SerializeField]
    SliderCTRL zonePosY;
    [SerializeField]
    SliderCTRL zonePosZ;
    [SerializeField]
    SliderCTRL zoneSizeX; //Какой размер у коллайдера
    [SerializeField]
    SliderCTRL zoneSizeY;
    [SerializeField]
    SliderCTRL zoneSizeZ;

    [SerializeField]
    GameObject visualizatorPrefabObj;
    [SerializeField]
    GameObject visualizatorParent;

    //Запомненные зоны
    BlockPhysics.ColliderZone[] zonesBuffer = new BlockPhysics.ColliderZone[0];

    const int sliderSizeMax = 100;

    string collidersMaxZero = "All";

    private void OnEnable()
    {
        IniAll();
    }
    private void OnDisable()
    {
        visualizatorParent.SetActive(false);
    }

    void IniAll() {
        if (RedactorBlocksCTRL.blockData == null)
            return;

        collidersMax.slider.minValue = 0;
        collidersMax.slider.maxValue = 4;

        //Вытаскиваем колайдеры
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;

        //Если зон нет то удаляем
        if (collidersMax.slider.value == 0)
        {
            physics.zones = null;
        }
        else {
            updateColliderMax(physics);
        }
            

    }

    // Update is called once per frame
    void Update()
    {
        updateVisualize();
    }

    public void clearVisualize() {
        //Получаем детей с меткой-скриптом
        LabelScript[] childs = visualizatorParent.GetComponentsInChildren<LabelScript>();
        foreach (LabelScript child in childs) {
            if (child == null)
                continue;

            Destroy(child.gameObject);
        }

    }
    public void updateVisualize() {
        visualizatorParent.SetActive(true);

        //Получаем детей с меткой-скриптом
        LabelScript[] childs = visualizatorParent.GetComponentsInChildren<LabelScript>();
        //Вытаскиваем колайдеры
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;

        //Если количество детей отличается от количества зон или зон нет и детей не равно 1
        if (physics.zones != null && physics.zones.Length != childs.Length ||
            physics.zones == null && childs.Length != 1) {
            //Удаляем что есть
            for (int num = 0; num < childs.Length; num++) {
                Destroy(childs[num].gameObject);
            }
            //Узнаем сколько надо новых детей
            int childCountNeed = 1;
            if (physics.zones != null) {
                childCountNeed = physics.zones.Length;
            }

            //Создаем
            for (int num = 0; num < childCountNeed; num++) {
                Instantiate(visualizatorPrefabObj, visualizatorParent.transform);
            }

            //Получаем новый список
            childs = visualizatorParent.GetComponentsInChildren<LabelScript>();
        }


        //Есть готовый список
        //применяем на него значения зон если зоны есть
        if (physics.zones != null)
            for (int num = 0; num < physics.zones.Length; num++)
            {
                childs[num].transform.localPosition = physics.zones[num].pos.ToVector3();
                childs[num].transform.localScale = physics.zones[num].size.ToVector3();

                LineRenderer[] lineRenderers = childs[num].GetComponentsInChildren<LineRenderer>();
                if (num == colliderSelect.slider.value)
                {
                    foreach (LineRenderer line in lineRenderers)
                    {
                        line.startColor = new Color(1f, 1f, 1f);
                        line.endColor = new Color(1f, 1f, 1f);
                    }
                }
                else {
                    foreach (LineRenderer line in lineRenderers)
                    {
                        line.startColor = new Color(0.5f, 0.5f, 0.5f);
                        line.endColor = new Color(0.5f, 0.5f, 0.5f);
                    }
                }
            }
        else {
            childs[0].transform.localPosition = new Vector3(0,0,0);
            childs[0].transform.localScale = new Vector3(1,1,1);
        }

    }

    public void clickCollidersMax() {
        collidersMax.slider.minValue = 0;
        collidersMax.slider.maxValue = 4;

        //настраиваем выбранный коллайдер
        colliderSelect.slider.minValue = 0;
        if (colliderSelect.slider.value >= collidersMax.slider.maxValue)
            colliderSelect.slider.value = collidersMax.slider.maxValue - 1;

        //Обновляем текст
        if (collidersMax.slider.value == 0) {
            collidersMax.SetValueText(collidersMaxZero);
        }
        else {
            collidersMax.SetValueText();
        }

        //Вытаскиваем колайдеры
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        updateColliderMax(physics);
    }

    public void updateColliderMax(BlockPhysics physics) {
        updateColliderMax(physics, false);
    }
    public void updateColliderMax(BlockPhysics physics, bool dataPriority) {

        if (!dataPriority)
        {
            //Если зон не должно быть
            if (collidersMax.slider.value == 0)
            {
                physics.zones = null;
                return;
            }
        }
        else {
            if (physics.zones == null)
            {
                collidersMax.slider.value = 0;
                
            }
            else {
                collidersMax.slider.value = physics.zones.Length;
                colliderSelect.slider.value = 0;
                colliderSelect.slider.maxValue = physics.zones.Length - 1;
                colliderSelect.slider.minValue = 0;
            }
        }


        if (physics.zones != null && physics.zones.Length == collidersMax.slider.value)
            return;

        //Сперва сохраняем если можно
        BufferSave();
        //потом загружаем если есть что
        BufferLoad();

        colliderSelect.slider.maxValue = collidersMax.slider.value - 1;
        if (colliderSelect.slider.value > colliderSelect.slider.maxValue) {
            colliderSelect.slider.value = colliderSelect.slider.maxValue;
        }

        updateColliderSelect();


        void BufferSave()
        {
            if (physics.zones == null)
                return;

            if (physics.zones.Length >= zonesBuffer.Length)
                zonesBuffer = physics.zones;
            else
            {
                for (int num = 0; num < physics.zones.Length; num++)
                {
                    zonesBuffer[num] = physics.zones[num];
                }
            }
        }
        void BufferLoad()
        {
            if (collidersMax.slider.value == 0)
            {
                physics.zones = null;
                return;
            }

            //Загружаем столько сколько надо
            physics.zones = new BlockPhysics.ColliderZone[(int)collidersMax.slider.value];
            for (int num = 0; num < physics.zones.Length; num++)
            {
                if (num < zonesBuffer.Length)
                    physics.zones[num] = zonesBuffer[num];
                else
                    physics.zones[num] = new BlockPhysics.ColliderZone();
            }
        }
    }

    public void clickCollidersSelect() {
        colliderSelect.SetValueText();

        updateColliderSelect();
    }

    public void clickZonePosX() {
        //Обновляем слайдеры в соответствии с выбранной зоной
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        physics.zones[(int)colliderSelect.slider.value].pos.x = (float)zonePosX.slider.value / sliderSizeMax;

        zonePosX.SetValueText(physics.zones[(int)colliderSelect.slider.value].pos.x.ToString());

        //Нужно убедиться что позиция не выходит за рамки блока
        float zoneSizeMaxX = 1 - physics.zones[(int)colliderSelect.slider.value].pos.x;
        zoneSizeX.slider.maxValue = zoneSizeMaxX * sliderSizeMax;


    }
    public void clickZonePosY() {
        //Обновляем слайдеры в соответствии с выбранной зоной
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        physics.zones[(int)colliderSelect.slider.value].pos.y = (float)zonePosY.slider.value / sliderSizeMax;

        zonePosY.SetValueText(physics.zones[(int)colliderSelect.slider.value].pos.y.ToString());

        //Нужно убедиться что позиция не выходит за рамки блока
        float zoneSizeMax = 1 - physics.zones[(int)colliderSelect.slider.value].pos.y;
        zoneSizeY.slider.maxValue = zoneSizeMax * sliderSizeMax;
    }
    public void clickZonePosZ()
    {
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        physics.zones[(int)colliderSelect.slider.value].pos.z = (float)zonePosZ.slider.value / sliderSizeMax;

        zonePosZ.SetValueText(physics.zones[(int)colliderSelect.slider.value].pos.z.ToString());

        //Нужно убедиться что позиция не выходит за рамки блока
        float zoneSizeMax = 1 - physics.zones[(int)colliderSelect.slider.value].pos.z;
        zoneSizeZ.slider.maxValue = zoneSizeMax * sliderSizeMax;
    }


    public void clickZoneSizeX() {
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        physics.zones[(int)colliderSelect.slider.value].size.x = (float)zoneSizeX.slider.value / sliderSizeMax;

        zoneSizeX.SetValueText(physics.zones[(int)colliderSelect.slider.value].size.x.ToString());
    }
    public void clickZoneSizeY() {
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        physics.zones[(int)colliderSelect.slider.value].size.y = (float)zoneSizeY.slider.value / sliderSizeMax;

        zoneSizeY.SetValueText(physics.zones[(int)colliderSelect.slider.value].size.y.ToString());
    }
    public void clickZoneSizeZ()
    {
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;
        physics.zones[(int)colliderSelect.slider.value].size.z = (float)zoneSizeZ.slider.value / sliderSizeMax;

        zoneSizeZ.SetValueText(physics.zones[(int)colliderSelect.slider.value].size.z.ToString());
    }

    public void updateColliderSelect() {
        //Обновляем слайдеры в соответствии с выбранной зоной
        BlockPhysics physics = RedactorBlocksCTRL.blockData.physics;

        if(physics == null ||
           physics.zones == null || 
           physics.zones.Length == 0)
                return;


        if (colliderSelect.slider.value >= physics.zones.Length)
            return;

        zonePosX.slider.minValue = 0;
        zonePosY.slider.minValue = 0;
        zonePosZ.slider.minValue = 0;
        zoneSizeX.slider.minValue = 0;
        zoneSizeY.slider.minValue = 0;
        zoneSizeZ.slider.minValue = 0;

        zonePosX.slider.maxValue = sliderSizeMax;
        zonePosY.slider.maxValue = sliderSizeMax;
        zonePosZ.slider.maxValue = sliderSizeMax;
        zoneSizeX.slider.maxValue = sliderSizeMax;
        zoneSizeY.slider.maxValue = sliderSizeMax;
        zoneSizeZ.slider.maxValue = sliderSizeMax;

        zonePosX.slider.value = physics.zones[(int)colliderSelect.slider.value].pos.x * sliderSizeMax;
        zonePosY.slider.value = physics.zones[(int)colliderSelect.slider.value].pos.y * sliderSizeMax;
        zonePosZ.slider.value = physics.zones[(int)colliderSelect.slider.value].pos.z * sliderSizeMax;
        zoneSizeX.slider.value = physics.zones[(int)colliderSelect.slider.value].size.x * sliderSizeMax;
        zoneSizeY.slider.value = physics.zones[(int)colliderSelect.slider.value].size.y * sliderSizeMax;
        zoneSizeZ.slider.value = physics.zones[(int)colliderSelect.slider.value].size.z * sliderSizeMax;

        clickZonePosX();
        clickZonePosY();
        clickZonePosZ();
        clickZoneSizeX();
        clickZoneSizeY();
        clickZoneSizeZ();
    }
}
