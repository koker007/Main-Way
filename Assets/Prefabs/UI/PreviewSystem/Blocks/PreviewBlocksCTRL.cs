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
        //����� ��������� ������
        PreviewBlock preview = main.GetFreePreview();
        preview.SetNewData(blockData);

        return preview;
    }

    PreviewBlock GetFreePreview(){

        //���������� ���� ������ � ������� ���������� ������
        foreach (PreviewBlock preview in previews) {
            if (preview.isFree) {
                return preview;
            }
        }

        //���������� �� �������, ��������� �����
        GameObject prefab = Instantiate(prefabPreview.gameObject, transform);

        PreviewBlock previewNew = prefab.GetComponent<PreviewBlock>();
        previewNew.SetPosFromID(previews.Count);
        previews.Add(previewNew);

        return previewNew;
    }
}
