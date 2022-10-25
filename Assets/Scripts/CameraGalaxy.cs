using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGalaxy : MonoBehaviour
{
    [SerializeField]
    public static CameraGalaxy main;

    [SerializeField]
    PlayerCTRL viewer;

    // Start is called before the first frame update
    void Start()
    {
        main = this;    
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransformPlayer();
    }

    //��������� ������ �������
    static public void SetLocalPlayer(PlayerCTRL playerCTRL) {
        main.viewer = playerCTRL;
    }

    void UpdateTransformPlayer() {
        if (!viewer)
            return;


        //��������� � ���������
        gameObject.transform.localPosition = viewer.NumCell + (viewer.PosInCell/CellS.size);

        //��������� ������ �� ����������� �������� ������
        gameObject.transform.rotation = viewer.GetSpaceView();
    }
    void UpdateTransformNoPlayer() {
        if (viewer)
            return;


    }

}
