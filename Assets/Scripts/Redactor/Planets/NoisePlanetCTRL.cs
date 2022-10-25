using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Редактирует конкретный шум в редакторе планет
public class NoisePlanetCTRL : MonoBehaviour
{
    [SerializeField]
    SliderCTRL sliderScaleX;
    [SerializeField]
    SliderCTRL sliderScaleY;
    [SerializeField]
    SliderCTRL sliderScaleZ;

    [SerializeField]
    SliderCTRL sliderOctaves;
    [SerializeField]
    SliderCTRL sliderFreq;

    NoiseData perlinData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Свернуть
    /// </summary>
    void ClickСollapse() {
    
    }
    /// <summary>
    /// Удалить данный шум
    /// </summary>
    void ClickDelete() {
        
    }
}
