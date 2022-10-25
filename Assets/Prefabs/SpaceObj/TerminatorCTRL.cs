using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminatorCTRL : MonoBehaviour
{
    [SerializeField]
    SpaceObjCtrl spaceObj;

    [SerializeField]
    Transform Pos;

    [SerializeField]
    Transform UpLeft;
    [SerializeField]
    Transform UpRight;
    [SerializeField]
    Transform DownLeft;
    [SerializeField]
    Transform DownRight;

    [Header("Atmoshere")]
    [SerializeField]
    Transform AtmosULxU;
    [SerializeField]
    Transform AtmosULxL;
    [SerializeField]
    Transform AtmosURxU;
    [SerializeField]
    Transform AtmosURxR;
    [SerializeField]
    Transform AtmosDLxD;
    [SerializeField]
    Transform AtmosDLxL;
    [SerializeField]
    Transform AtmosDRxD;
    [SerializeField]
    Transform AtmosDRxR;

    [SerializeField]
    MeshRenderer[] AtmosAll;

    //полная атмосфера
    GameObject AtmosFullUp;
    GameObject AtmosFullDown;
    GameObject AtmosFullLeft;
    GameObject AtmosFullRight;

    // Update is called once per frame
    void Update()
    {
        if (spaceObj.data == null)
            return;

        Calc();
    }

    public void SetAtmosphereFull(MeshRenderer Left, MeshRenderer Right, MeshRenderer Down, MeshRenderer Up) {
        foreach (MeshRenderer mesh in AtmosAll) {
            mesh.material.color = Left.material.color;
        }

        AtmosFullUp = Up.gameObject;
        AtmosFullDown = Down.gameObject;
        AtmosFullRight = Right.gameObject;
        AtmosFullLeft = Left.gameObject;
    }

    void Calc() {
        //Выключаем терминатор если это зцезда
        if (spaceObj.data.parent == null)
        {
            AtmosFullUp.SetActive(true);
            AtmosFullDown.SetActive(true);
            AtmosFullRight.SetActive(true);
            AtmosFullLeft.SetActive(true);

            gameObject.SetActive(false);
        }

        SpaceObjCtrl starObj = spaceObj;
        //Находим звезду
        while (starObj.data.parent != null) {
            starObj = starObj.data.parent.visual;
        }

        //Находим вектор светила
        Vector3 VecSun = starObj.rotateObj.transform.position - spaceObj.rotateObj.transform.position;
        VecSun.Normalize();

        //Находим вектор лицевой стороны
        float AngleForvard = Vector3.Angle(transform.forward, VecSun);

        float AngleUp = Vector3.Angle(transform.up, VecSun);
        float AngleR = Vector3.Angle(transform.right, VecSun);

        SetTerminator(UpLeft, 0, 0);
        SetTerminator(UpRight, 0, 0);
        SetTerminator(DownLeft, 0, 0);
        SetTerminator(DownRight, 0, 0);


        //Обработка лево-право
        //Если солнце спереди //темная сторона
        if (AngleForvard < 90)
        {
            float intensiveR = AngleR / 90;
            intensiveR -= 1;
            intensiveR = Mathf.Abs(intensiveR);
            intensiveR = 1 - (intensiveR / 2);

            float intensiveUp = AngleUp / 90;
            intensiveUp -= 1;
            intensiveUp = Mathf.Abs(intensiveUp);
            intensiveUp = 1 - (intensiveUp / 2);

            //Правая сторона освещена
            if (AngleR < 90)
            {
                //Верх освещен
                if (AngleUp < 90)
                    SetUpRight(intensiveR, intensiveUp);
                else
                    SetDownRight(intensiveR, intensiveUp);
            }
            //Левая сторона освещена
            else {
                //Верх освещен
                if (AngleUp < 90)
                    SetUpLeft(intensiveR, intensiveUp);
                else
                    SetDownLeft(intensiveR, intensiveUp);
            }
        }
        //Если солнце сзади // Светлая сторона
        else
        {
            float intensiveR = AngleR / 90;
            intensiveR -= 1;
            intensiveR = Mathf.Abs(intensiveR);
            intensiveR = intensiveR / 2;

            float intensiveUp = AngleUp / 90;
            intensiveUp -= 1;
            intensiveUp = Mathf.Abs(intensiveUp);
            intensiveUp = 1 - intensiveUp / 2;

            //Правая сторона освещена
            if (AngleR < 90)
            {
                //Верх освещен
                if (AngleUp < 90)
                    SetUpRight(intensiveR, intensiveUp);
                else
                    SetDownRight(intensiveR, intensiveUp);
            }
            //Левая сторона освещена
            else
            {
                //Верх освещен
                if (AngleUp < 90)
                    SetUpLeft(intensiveR, intensiveUp);
                else
                    SetDownLeft(intensiveR, intensiveUp);
            }
        }



        ////////////////////////////////////////////////////////
        //Солнце с правого верха
        void SetUpRight(float x, float y) {
            Pos.localPosition = new Vector3(0,0,0);

            SetTerminator(UpRight, x, y);

            UpLeft.gameObject.SetActive(false);
            DownLeft.gameObject.SetActive(false);
            DownRight.gameObject.SetActive(false);

            AtmosULxU.transform.parent.gameObject.SetActive(false);
            AtmosURxU.transform.parent.gameObject.SetActive(true);
            AtmosDLxD.transform.parent.gameObject.SetActive(false);
            AtmosDRxD.transform.parent.gameObject.SetActive(false);

            AtmosURxR.localScale = new Vector3(1 - x, 1, 1);
            AtmosURxU.localScale = new Vector3(1 - y, 1, 1);

            //полные
            AtmosFullRight.SetActive(true);
            AtmosFullUp.SetActive(true);
            AtmosFullLeft.SetActive(false);
            AtmosFullDown.SetActive(false);
        }
        //Солнце с низа правого
        void SetDownRight(float x, float y)
        {
            Pos.localPosition = new Vector3(0, 1, 0);

            SetTerminator(DownRight, x, y);

            UpLeft.gameObject.SetActive(false);
            UpRight.gameObject.SetActive(false);
            DownLeft.gameObject.SetActive(false);

            AtmosULxU.transform.parent.gameObject.SetActive(false);
            AtmosURxU.transform.parent.gameObject.SetActive(false);
            AtmosDLxD.transform.parent.gameObject.SetActive(false);
            AtmosDRxD.transform.parent.gameObject.SetActive(true);

            AtmosDRxD.localScale = new Vector3(1 - y, 1, 1);
            AtmosDRxR.localScale = new Vector3(1 - x, 1, 1);

            AtmosFullRight.SetActive(true);
            AtmosFullUp.SetActive(false);

            AtmosFullLeft.SetActive(false);
            AtmosFullDown.SetActive(true);
        }
        //Солнце с низа левого
        void SetDownLeft(float x, float y)
        {
            Pos.localPosition = new Vector3(1, 1, 0);

            SetTerminator(DownLeft, x, y);
            UpLeft.gameObject.SetActive(false);
            UpRight.gameObject.SetActive(false);
            DownRight.gameObject.SetActive(false);

            AtmosULxU.transform.parent.gameObject.SetActive(false);
            AtmosURxU.transform.parent.gameObject.SetActive(false);
            AtmosDLxD.transform.parent.gameObject.SetActive(true);
            AtmosDRxD.transform.parent.gameObject.SetActive(false);

            AtmosDLxD.localScale = new Vector3(1 - y, 1, 1);
            AtmosDLxL.localScale = new Vector3(1 - x, 1, 1);

            AtmosFullRight.SetActive(false);
            AtmosFullUp.SetActive(false);

            AtmosFullLeft.SetActive(true);
            AtmosFullDown.SetActive(true);
        }
        //Солнце с верха левого
        void SetUpLeft(float x, float y)
        {
            Pos.localPosition = new Vector3(1, 0, 0);

            SetTerminator(UpLeft, x, y);
            UpRight.gameObject.SetActive(false);
            DownLeft.gameObject.SetActive(false);
            DownRight.gameObject.SetActive(false);

            AtmosULxU.transform.parent.gameObject.SetActive(true);
            AtmosURxU.transform.parent.gameObject.SetActive(false);
            AtmosDLxD.transform.parent.gameObject.SetActive(false);
            AtmosDRxD.transform.parent.gameObject.SetActive(false);

            AtmosULxU.localScale = new Vector3(1 - y, 1, 1);
            AtmosULxL.localScale = new Vector3(1 - x, 1, 1);

            AtmosFullRight.SetActive(false);
            AtmosFullUp.SetActive(true);

            AtmosFullLeft.SetActive(true);
            AtmosFullDown.SetActive(false);

        }

        void SetTerminator(Transform transform, float x, float y)
        {
            transform.gameObject.SetActive(true);
            transform.localScale = new Vector3(x, y, 1);
        }
    }
}
