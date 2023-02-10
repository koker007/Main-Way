using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorUICTRL : MonoBehaviour
{
    public static RedactorUICTRL main;

    [SerializeField]
    RedactorBlocksCTRL redactorBlocks;
    [SerializeField]
    RedactorBiomeCTRL redactorBiome;
    [SerializeField]
    RedactorPlanetsCTRL redactorPlanets;


    // Start is called before the first frame update
    void Start()
    {
        iniRedactorUI();
    }

    public void iniRedactorUI() {
        main = this;

        if (!redactorBlocks)
            redactorBlocks = GetComponentInChildren<RedactorBlocksCTRL>();
        if (!redactorPlanets)
            redactorPlanets = GetComponentInChildren<RedactorPlanetsCTRL>();

        redactorBlocks.gameObject.SetActive(false);
        redactorPlanets.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void OpenRedactorBlocks() {
        main.gameObject.SetActive(true);

        main.redactorBlocks.gameObject.SetActive(true);

        WindowMenuCTRL.CloseALL();
        UICTRL.IsOpenMainMenu = false;
    }

    static public void OpenRedactorBiomes() {
        main.gameObject.SetActive(true);
        main.redactorBiome.gameObject.SetActive(true);

        WindowMenuCTRL.CloseALL();
        UICTRL.IsOpenMainMenu = false;
    }

    static public void OpenRedactorPlanets() {
        main.gameObject.SetActive(true);

        main.redactorPlanets.gameObject.SetActive(true);

        WindowMenuCTRL.CloseALL();
        UICTRL.IsOpenMainMenu = false;
    }

}
