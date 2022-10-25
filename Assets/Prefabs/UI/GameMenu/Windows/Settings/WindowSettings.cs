using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickButtonControl() {
        WindowMenuCTRL.ClickControl();
    }
    public void ClickButtonAudio() {
        WindowMenuCTRL.ClickSound();
    }
    public void ClickButtonGraffic() {
        WindowMenuCTRL.ClickGraffic();
    }
}
