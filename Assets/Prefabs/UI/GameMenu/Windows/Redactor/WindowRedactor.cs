using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowRedactor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickButtonBlocksRedactor()
    {
        RedactorUICTRL.OpenRedactorBlocks();
    }
    public void clickButtonBiomeRedactor()
    {
        RedactorUICTRL.OpenRedactorBiomes();
    }
    public void clickButtonPlanetRedactor() {
        RedactorUICTRL.OpenRedactorPlanets();
    }
}
