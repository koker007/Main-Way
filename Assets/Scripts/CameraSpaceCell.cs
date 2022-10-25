using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpaceCell : MonoBehaviour
{
    [SerializeField]
    public static CameraSpaceCell main;

    [SerializeField]
    PlayerCTRL viewer; //����� ��� �������� ����� ����������

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    void Update()
    {
        UpdateTransformPlayer();
    }

    static public void SetLocalPlayer(PlayerCTRL playerCTRL) {
        main.viewer = playerCTRL;
    }

    void UpdateTransformPlayer()
    {
        gameObject.transform.position = new Vector3(0,0,0);

        if (!viewer)
            return;

        //��������� � ���������
        //Vector3 posInCell = (viewer.PosInCell / CellS.size);

        //gameObject.transform.localPosition = (viewer.NumCell + posInCell) * CellS.sizeVisual;

        //��������� ������ �� ����������� �������� ������
        gameObject.transform.rotation = viewer.GetSpaceView();
    }
    void UpdateTransformNoPlayer()
    {

    }

}
