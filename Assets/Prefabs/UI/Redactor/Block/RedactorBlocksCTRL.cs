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
    RedactorBlockRightPanelsCTRL panelsCTRL;

    [SerializeField]
    InputFieldCTRL inputModName;
    [SerializeField]
    InputFieldCTRL inputBlockName;

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
        
        //Установить тестовую текстуру
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
        //Если в буфере пусто
        if (WindowMenuCTRL.buffer.Count == 0)
        {
            if (shtorka.gameObject.activeSelf)
                shtorka.gameObject.SetActive(false);

            return;
        }

        //Если в буфере что-то есть
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

    void updateMainParameters() {
        //если блока нет - выходим
        if (blockDataLocal == null)
            return;

        getModName();
        getBlockName();

        void getModName() {
            if (blockDataLocal.mod == null ||
                blockDataLocal.mod.Length <= 0) {

                inputModName.inputField.text = "";
                return;
            }

            inputModName.inputField.text = blockDataLocal.mod;
        }
        void getBlockName() {
            if (blockDataLocal.name == null ||
                blockDataLocal.name.Length <= 0)
            {

                inputBlockName.inputField.text = "";
                return;
            }

            inputBlockName.inputField.text = blockDataLocal.name;
        }
    }
    public void acceptModName() {
        //Можем применять только если поле ввода есть
        if (inputModName == null ||
            inputModName.inputField.text == null ||
            inputModName.inputField.text.Length == 0
            ) {
            return;
        }

        //Проверяем сам текст
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

        //Проверяем сам текст
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

    public void clickButtonSave() {
        //проверяем что имя мода есть
        if (blockDataLocal.mod == null || blockDataLocal.mod.Length == 0) {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //проверяем что имя мода больше 3
        if (blockDataLocal.mod.Length < 3) {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //Проверяем что имя блока есть
        if (blockDataLocal.name == null || blockDataLocal.name.Length == 0) {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //Проверняем что имя больше 3х символов
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

        panelsCTRL.ReDrawingAll();

        updateMainParameters();
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
