using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickButtonCreateGame() {
        WindowMenuCTRL.ClickServer();
    }
    public void ClickButtonConnect() {
        WindowMenuCTRL.ClickConnect();
    }
}
