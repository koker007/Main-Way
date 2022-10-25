using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitleCTRL : MonoBehaviour
{
    static public GameTitleCTRL main;

    [SerializeField]
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void MenuIsOpen() {
        main.animator.SetBool("MenuIsOpen", true);
    }
    public static void ViewFirstMenu() {
        main.animator.SetBool("isHide", false);
    }
    public static void HideFirstMenu() {
        main.animator.SetBool("isHide", true);
    }
}
