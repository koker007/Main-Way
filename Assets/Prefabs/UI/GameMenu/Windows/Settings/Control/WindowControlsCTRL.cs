using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//примен€ет новые настройки управлени€ игры
public class WindowControlsCTRL : MonoBehaviour
{
    //ќжидает ли кака€ нибудь из кнопок нажати€
    string WaitPress = "";

    string keyPleasePress = "WindowControlsPleasePress";
    string strPleasePress = "Please Press";

    string strMoveForvard = "Forvard";
    string strMoveBack = "Back";
    string strMoveRight = "Right";
    string strMoveLeft = "Left";
    string strMoveUp = "Up";
    string strMoveDown = "Down";
    string strMoveJump = "Jump";
    string strMoveSit = "Sit";
    string strAttack = "Attack";
    string strInteraction = "Interation";
    string strInventory = "Inventory";

    string strItem1 = "Item1";
    string strItem2 = "Item2";
    string strItem3 = "Item3";
    string strItem4 = "Item4";
    string strItem5 = "Item5";
    string strItem6 = "Item6";
    string strItem7 = "Item7";
    string strItem8 = "Item8";
    string strItem9 = "Item9";

    [SerializeField]
    ButtonCTRL ButtonMoveForvard;
    [SerializeField]
    ButtonCTRL ButtonMoveBack;
    [SerializeField]
    ButtonCTRL ButtonMoveRight;
    [SerializeField]
    ButtonCTRL ButtonMoveLeft;
    [SerializeField]
    ButtonCTRL ButtonMoveUp;
    [SerializeField]
    ButtonCTRL ButtonMoveDown;
    [SerializeField]
    ButtonCTRL ButtonMoveJump;
    [SerializeField]
    ButtonCTRL ButtonMoveSit;
    [SerializeField]
    ButtonCTRL ButtonAttack;
    [SerializeField]
    ButtonCTRL ButtonInteraction;
    [SerializeField]
    ButtonCTRL ButtonInventory;

    [SerializeField]
    ButtonCTRL ButtonItem1;
    [SerializeField]
    ButtonCTRL ButtonItem2;
    [SerializeField]
    ButtonCTRL ButtonItem3;
    [SerializeField]
    ButtonCTRL ButtonItem4;
    [SerializeField]
    ButtonCTRL ButtonItem5;
    [SerializeField]
    ButtonCTRL ButtonItem6;
    [SerializeField]
    ButtonCTRL ButtonItem7;
    [SerializeField]
    ButtonCTRL ButtonItem8;
    [SerializeField]
    ButtonCTRL ButtonItem9;

    // Start is called before the first frame update
    void Start()
    {
        LoadTranslate();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        TestPress();
    }

    void TestPress() {
        //≈сли не ожидаетс€ нажатие или в данный момент никака€ кнопка не нажата
        if (WaitPress.Length <= 0 || !Input.anyKey)
            return;

        KeyCode keyPress = Controls.GetPressKey();
        if (keyPress == KeyCode.None)
            return;

        switch (WaitPress) {
            case Controls.strMoveForward: 
                Controls.keyMoveForward = keyPress;
                break;
            case Controls.strMoveBack:
                Controls.keyMoveBack = keyPress;
                break;
            case Controls.strMoveRight:
                Controls.keyMoveRight = keyPress;
                break;
            case Controls.strMoveLeft:
                Controls.keyMoveLeft = keyPress;
                break;
            case Controls.strMoveUp:
                Controls.keyMoveUp = keyPress;
                break;
            case Controls.strMoveDown:
                Controls.keyMoveDown = keyPress;
                break;
            case Controls.strMoveJump:
                Controls.keyMoveJump = keyPress;
                break;
            case Controls.strMoveSit:
                Controls.keyMoveSit = keyPress;
                break;
            case Controls.strAttack:
                Controls.keyAttack = keyPress;
                break;
            case Controls.strOpenInteraction:
                Controls.keyOpenInteraction = keyPress;
                break;
            case Controls.strOpenInventory:
                Controls.keyOpenInventory = keyPress;
                break;

            default: 
                break;
        }

        //нова€ клавиша применена
        WaitPress = "";

        UpdateText();
    }

    void LoadTranslate() {
        strMoveForvard = Language.GetTextFromKey(Controls.strMoveForward, strMoveForvard);
        strMoveBack = Language.GetTextFromKey(Controls.strMoveBack, strMoveBack);
        strMoveLeft = Language.GetTextFromKey(Controls.strMoveLeft, strMoveLeft);
        strMoveRight = Language.GetTextFromKey(Controls.strMoveRight, strMoveRight);
        strMoveUp = Language.GetTextFromKey(Controls.strMoveUp, strMoveUp);
        strMoveDown = Language.GetTextFromKey(Controls.strMoveDown, strMoveDown);
        strMoveJump = Language.GetTextFromKey(Controls.strMoveJump, strMoveJump);
        strMoveSit = Language.GetTextFromKey(Controls.strMoveSit, strMoveSit);
        strAttack = Language.GetTextFromKey(Controls.strAttack, strAttack);
        strInteraction = Language.GetTextFromKey(Controls.strOpenInteraction, strInteraction);
        strInventory = Language.GetTextFromKey(Controls.strOpenInventory, strInventory);
    }
    void UpdateText() {
        ButtonMoveForvard.SetText(strMoveForvard + ":" + Controls.GetNameKeyName(Controls.keyMoveForward));
        ButtonMoveBack.SetText(strMoveBack + ":" + Controls.GetNameKeyName(Controls.keyMoveBack));
        ButtonMoveRight.SetText(strMoveRight + ":" + Controls.GetNameKeyName(Controls.keyMoveRight));
        ButtonMoveLeft.SetText(strMoveLeft + ":" + Controls.GetNameKeyName(Controls.keyMoveLeft));
        ButtonMoveUp.SetText(strMoveUp + ":" + Controls.GetNameKeyName(Controls.keyMoveUp));
        ButtonMoveDown.SetText(strMoveDown + ":" + Controls.GetNameKeyName(Controls.keyMoveDown));
        ButtonMoveJump .SetText(strMoveJump + ":" + Controls.GetNameKeyName(Controls.keyMoveJump));
        ButtonMoveSit.SetText(strMoveSit + ":" + Controls.GetNameKeyName(Controls.keyMoveSit));
        ButtonAttack.SetText(strAttack + ":" + Controls.GetNameKeyName(Controls.keyAttack));
        ButtonInteraction.SetText(strInteraction + ":" + Controls.GetNameKeyName(Controls.keyOpenInteraction));
        ButtonInventory.SetText(strInventory + ":" + Controls.GetNameKeyName(Controls.keyOpenInventory));
    }


    //Buttons Click
    public void ClickButtonSetDefault() {
        Controls.SetDefault();
        UpdateText();
    }
    public void ClickButtonMoveForvard() {
        ButtonMoveForvard.SetText(strPleasePress);
        WaitPress = Controls.strMoveForward;
    }
    public void ClickButtonMoveBack()
    {
        ButtonMoveBack.SetText(strPleasePress);
        WaitPress = Controls.strMoveBack;
    }
    public void ClickButtonMoveRight() {
        ButtonMoveRight.SetText(strPleasePress);
        WaitPress = Controls.strMoveRight;
    }
    public void ClickButtonMoveLeft()
    {
        ButtonMoveLeft.SetText(strPleasePress);
        WaitPress = Controls.strMoveLeft;
    }
    public void ClickButtonMoveUp()
    {
        ButtonMoveUp.SetText(strPleasePress);
        WaitPress = Controls.strMoveUp;
    }
    public void ClickButtonMoveDown()
    {
        ButtonMoveDown.SetText(strPleasePress);
        WaitPress = Controls.strMoveDown;
    }
    public void ClickButtonMoveJump()
    {
        ButtonMoveJump.SetText(strPleasePress);
        WaitPress = Controls.strMoveJump;
    }
    public void ClickButtonMoveSit()
    {
        ButtonMoveSit.SetText(strPleasePress);
        WaitPress = Controls.strMoveSit;
    }
    public void ClickButtonAttack()
    {
        ButtonAttack.SetText(strAttack);
        WaitPress = Controls.strAttack;
    }
    public void ClickButtonInteraction()
    {
        ButtonInteraction.SetText(strPleasePress);
        WaitPress = Controls.strOpenInteraction;
    }
    public void ClickButtonInventory()
    {
        ButtonInventory.SetText(strPleasePress);
        WaitPress = Controls.strOpenInventory;
    }
}
