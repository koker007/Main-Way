using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowMenu : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    Image ramka;
    [SerializeField]
    Image background;

    public enum Move{
        left,
        right
    }

    public Move openFrom = Move.right;
    public Move closeTo = Move.left;
    bool isOpening = false;
    bool isClosing = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayAnimOpen();
    }

    private void OnEnable()
    {
        PlayAnimOpen();
    }

    // Update is called once per frame
    void Update()
    {
        testClosing();
    }

    void testClosing() {
        if (isClosing) 
            return;

        //≈сли оно первое в списке то закрывать не нужно
        if (WindowMenuCTRL.buffer.Count > 0 &&
            WindowMenuCTRL.buffer[0] == this)
        {
            return;
        }

        bool found = false;
        foreach (WindowMenu window in WindowMenuCTRL.buffer) {
            //≈сли это окно есть в списке, то удал€ть не надо
            if (window == this) {
                found = true;
            }
        }

        //≈сли окно нет в списке то закрываем в право
        if (!found)
        {
            closeTo = Move.right;
        }
        //≈сли окно все еще в списке
        else {
            closeTo = Move.left;
        }

        //закрываем окно
        PlayAnimClose();
    }


    public void PlayOpenLeft() {
        openFrom = Move.left;
        PlayAnimOpen();
    }
    public void PlayOpenRight() {
        openFrom = Move.right;
        PlayAnimOpen();
    }

    void PlayAnimOpen() {
        if (isOpening) 
            return;

        if (openFrom == Move.left) {
            animator.Play("WindowMenuOpenLeft");
        }
        else if (openFrom == Move.right) {
            animator.Play("WindowMenuOpenRight");
        }

        isOpening = true;
        isClosing = false;
    }
    void PlayAnimClose() {
        if (closeTo == Move.left)
        {
            animator.Play("WindowMenuCloseLeft");
        }
        else if (closeTo == Move.right)
        {
            animator.Play("WindowMenuCloseRight");
        }

        isOpening = false;
        isClosing = true;
    }

    public void Delete() {
        Destroy(gameObject);
    }

    public void ClickButtonClose() {
        WindowMenuCTRL.CloseFirstInBuffer(this);
    }
}
