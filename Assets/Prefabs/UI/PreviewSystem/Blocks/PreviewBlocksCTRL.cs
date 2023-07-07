using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBlocksCTRL : MonoBehaviour
{
    static PreviewBlocksCTRL main;

    List<PreviewBlock> previews = new List<PreviewBlock>();

    [SerializeField]
    PreviewBlock prefabPreview;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    static public PreviewBlock GetPreview(BlockData blockData) {
        //Ѕерем свободное превью
        PreviewBlock preview = main.GetFreePreview();
        preview.SetNewData(blockData);

        return preview;
    }

    PreviewBlock GetFreePreview(){

        //ѕеребираем весь список в поисках свободного превью
        foreach (PreviewBlock preview in previews) {
            if (preview.isFree) {
                return preview;
            }
        }

        //—вободного не нашлось, добавл€ем новое
        GameObject prefab = Instantiate(prefabPreview.gameObject, transform);

        PreviewBlock previewNew = prefab.GetComponent<PreviewBlock>();
        previewNew.SetPosFromID(previews.Count);
        previews.Add(previewNew);

        return previewNew;
    }
}
