using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyObjCtrl : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField]
    public SpriteRenderer Pixel;
    [SerializeField]
    public GameObject PixelMain;

    [Header("Data")]
    [SerializeField]
    public SpaceObjData data;

    float timeLookAtOld = 0;

    public void Ini(SpaceObjData data) {
        //Связываем информацию и объект друг с другом
        this.data = data;
        this.data.cell.visual = this;

        //Ставим объект на позицию
        //Если у этого объекта есть родитель
        if (this.data.parent != null)
        {
            
        }
        //Если нет родителя
        else {
            //Ставим в позицию ячейки
            transform.localPosition = this.data.cell.pos;

            float perlin = this.data.cell.perlinGlob;
            Color color = new Color(perlin, perlin, perlin);

            Pixel.color = color;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    void InvokeLookAt() {
        float dist = Vector3.Distance(CameraGalaxy.main.transform.position, gameObject.transform.position);

        //Если c последней проверки прошло больше времени чем растояние между камерой и звездой то звезду нужно заного развезнуть
        if (enabled && Time.unscaledTime - timeLookAtOld > dist) {
            updateObj();

        }
        

        //проверить снова
        Invoke("InvokeLookAt", Random.Range(0.10f, dist/2));
    }

    
    public void updateObj() {

        //Визуализировать можно
        updateTransform();
    }

    void updateTransform() {
        float dist = Vector3.Distance(CameraGalaxy.main.transform.position, gameObject.transform.position);
        timeLookAtOld = Time.unscaledTime;
        gameObject.transform.LookAt(CameraGalaxy.main.transform);

        float sizeData = Calc.GetSizeInt(data.size);
        sizeData /= CellS.size;

        float coof = (int)data.size;
        float visualSize = (dist * 0.3333f * 10) / coof;
        if (visualSize < sizeData * 50)
            visualSize = sizeData * 50;

        float distToMinSize = 15;
        if (dist > distToMinSize)
            visualSize /= (dist/distToMinSize);


        PixelMain.transform.localScale = new Vector3(visualSize, visualSize, visualSize);
    }
}
