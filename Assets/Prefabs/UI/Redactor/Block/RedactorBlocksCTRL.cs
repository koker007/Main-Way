using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorBlocksCTRL : MonoBehaviour
{
    public static RedactorBlocksCTRL main;
    public const float voxSize = 0.0625f;

    [Header("Other")]
    [SerializeField]
    Image shtorka;

    BlockData blockDataLocal;
    static public BlockData blockData{
        get {
            if (main == null)
                return null;

            return main.blockDataLocal;
        }
    }

    bool needSave = false;

    [SerializeField]
    InputFieldCTRL inputModName;
    [SerializeField]
    InputFieldCTRL inputBlockName;

    [Header("Visual")]
    [SerializeField]
    SliderCTRL sliderTransparent;
    [SerializeField]
    SliderCTRL sliderTransparentPower;
    [SerializeField]
    SliderCTRL sliderLight;
    [SerializeField]
    SliderCTRL sliderLightRange;

    // Start is called before the first frame update
    void Start()
    {
        main = this;

        SetRandomVoxel();
    }

    void SetRandomVoxel() {
        blockDataLocal = new BlockData();

        blockDataLocal.wallFace = new BlockWall(Side.face);
        blockDataLocal.wallBack = new BlockWall(Side.back);
        blockDataLocal.wallRight = new BlockWall(Side.right);
        blockDataLocal.wallLeft = new BlockWall(Side.left);
        blockDataLocal.wallUp = new BlockWall(Side.up);
        blockDataLocal.wallDown = new BlockWall(Side.down);

        for (int x = 0; x < blockDataLocal.wallFace.forms.voxel.GetLength(0); x++) {
            for (int y = 0; y < blockDataLocal.wallFace.forms.voxel.GetLength(1); y++) {
                blockDataLocal.wallFace.forms.voxel[x, y] = Random.Range(0, 1.0f);
                blockDataLocal.wallBack.forms.voxel[x, y] = Random.Range(0, 1.0f);
                blockDataLocal.wallRight.forms.voxel[x, y] = Random.Range(0, 1.0f);
                blockDataLocal.wallLeft.forms.voxel[x, y] = Random.Range(0, 1.0f);
                blockDataLocal.wallUp.forms.voxel[x, y] = Random.Range(0, 1.0f);
                blockDataLocal.wallDown.forms.voxel[x, y] = Random.Range(0, 1.0f);
            }
        }
        
        //���������� �������� ��������
        blockDataLocal.wallFace.SetTextureTest();
        blockDataLocal.wallBack.SetTextureTest();
        blockDataLocal.wallRight.SetTextureTest();
        blockDataLocal.wallLeft.SetTextureTest();
        blockDataLocal.wallUp.SetTextureTest();
        blockDataLocal.wallDown.SetTextureTest();

        blockDataLocal.wallFace.calcVertices();
        blockDataLocal.wallBack.calcVertices();
        blockDataLocal.wallRight.calcVertices();
        blockDataLocal.wallLeft.calcVertices();
        blockDataLocal.wallUp.calcVertices();
        blockDataLocal.wallDown.calcVertices();
    } 

    // Update is called once per frame
    void Update()
    {
        updateShtorka();
    }

    void updateShtorka()
    {
        //���� � ������ �����
        if (WindowMenuCTRL.buffer.Count == 0)
        {
            if (shtorka.gameObject.activeSelf)
                shtorka.gameObject.SetActive(false);

            return;
        }

        //���� � ������ ���-�� ����
        if (!shtorka.gameObject.activeSelf)
            shtorka.gameObject.SetActive(true);

        if (needSave)
        {
            shtorka.color = new Color(1, 0, 0, 0.5f);
        }
        else
        {
            shtorka.color = new Color(0, 0, 0, 0.5f);
        }
    }

    public void acceptModName() {
        //����� ��������� ������ ���� ���� ����� ����
        if (inputModName == null ||
            inputModName.inputField.text == null ||
            inputModName.inputField.text.Length == 0
            ) {
            return;
        }

        //��������� ��� �����
        if (inputModName.inputField.text[0] == ' ') {
            string textNew = "";
            bool symbolOld = false;
            for (int x = 0; x < inputModName.inputField.text.Length; x++) {
                if (inputModName.inputField.text[x] == ' ')
                {
                    if (!symbolOld)
                        continue;
                }
                else {
                    symbolOld = true;
                }
                textNew += inputModName.inputField.text;
            }

            inputModName.inputField.text = textNew;
        }

        blockDataLocal.mod = inputModName.inputField.text;

    }
    public void acceptBlockName() {
        if (inputBlockName == null ||
           inputBlockName.inputField.text == null ||
           inputBlockName.inputField.text.Length == 0) {
            return;
        }

        //��������� ��� �����
        if (inputBlockName.inputField.text[0] == ' ')
        {
            string textNew = "";
            bool symbolOld = false;
            for (int x = 0; x < inputBlockName.inputField.text.Length; x++)
            {
                if (inputBlockName.inputField.text[x] == ' ')
                {
                    if (!symbolOld)
                        continue;
                }
                else
                {
                    symbolOld = true;
                }
                textNew += inputBlockName.inputField.text;
            }

            inputBlockName.inputField.text = textNew;
        }

        blockDataLocal.name = inputBlockName.inputField.text;
    }

    public void acceptTransparent() {
        if (sliderTransparent.slider.value == (int)TypeBlockTransparent.NoTransparent) {
            sliderTransparent.SetValueText("Off", KeyBlockTransparent.keyOff);
        }
        else if (sliderTransparent.slider.value == (int)TypeBlockTransparent.CutOff) {
            sliderTransparent.SetValueText("CutOff", KeyBlockTransparent.keyCutOff);
        }
        else if (sliderTransparent.slider.value == (int)TypeBlockTransparent.Alpha) {
            sliderTransparent.SetValueText("Alpha", KeyBlockTransparent.keyAlpha);
        }
        else {
            sliderTransparent.SetValueText();
        }
    }
    public void acceptTransparentPower() {
        sliderTransparentPower.SetValueText((sliderTransparentPower.slider.value/100.0f) + "");
    }

    public void acceptLight() {
        if (sliderLight.slider.value == 0)
        {
            sliderLight.SetValueText("Off", "keyTextOff");
        }
        else if(sliderLight.slider.value == 1) {
            sliderLight.SetValueText("On", "keyTextOn");
        }
        else
        {
            sliderLight.SetValueText();
        }
    }

    public void acceptLightRange()
    {
        sliderLightRange.SetValueText();
    }

    public void clickButtonSave() {
        //��������� ��� ��� ���� ����
        if (blockDataLocal.mod == null || blockDataLocal.mod.Length == 0) {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //��������� ��� ��� ���� ������ 3
        if (blockDataLocal.mod.Length < 3) {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //��������� ��� ��� ����� ����
        if (blockDataLocal.name == null || blockDataLocal.name.Length == 0) {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //���������� ��� ��� ������ 3� ��������
        if (blockDataLocal.name.Length < 3) {
            Debug.Log("NotSave Need Block Name Lenght > 3");
            return;
        }

        BlockData.SaveData(blockDataLocal);
    }
    public void clickButtonLoad() {
        WindowMenuCTRL.CloseALL(true);
        WindowMenuCTRL.ClickRedactorBlockLoad();
    }

    public void loadBlock(string pathBlock) {
        blockDataLocal = BlockData.LoadData(pathBlock);
        reDrawBlock();
    }

    public void reDrawBlock() {
        if (blockDataLocal.wallFace != null)
        {
            blockDataLocal.wallFace.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.wallFace);
        }
        if (blockDataLocal.wallBack != null)
        {
            blockDataLocal.wallBack.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.wallBack);
        }
        if (blockDataLocal.wallLeft != null)
        {
            blockDataLocal.wallLeft.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.wallLeft);
        }
        if (blockDataLocal.wallRight != null)
        {
            blockDataLocal.wallRight.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.wallRight);
        }
        if (blockDataLocal.wallUp != null)
        {
            blockDataLocal.wallUp.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.wallUp);
        }
        if (blockDataLocal.wallDown != null)
        {
            blockDataLocal.wallDown.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.wallDown);
        }
    }
}

public enum TypeBlockTransparent {
    NoTransparent = 0,
    CutOff = 1,
    Alpha = 2
}
public struct KeyBlockTransparent {
    public const string keyOff = "keyOff";
    public const string keyCutOff = "keyCutOff";
    public const string keyAlpha = "keyAlpha";
}
