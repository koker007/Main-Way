using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redactor
{
    public class PanelRightRuleParameters : PanelRightRedactorBiome
    {


        [SerializeField]
        SliderCTRL sliderPerlinScale;
        [SerializeField]
        SliderCTRL sliderPerlinOctaves;
        [SerializeField]
        SliderCTRL sliderPerlinFreq;
        [SerializeField]
        SliderCTRL sliderPerlinScaleX;
        [SerializeField]
        SliderCTRL sliderPerlinScaleY;
        [SerializeField]
        SliderCTRL sliderPerlinScaleZ;

        // Start is called before the first frame update
        void Start()
        {
            RedactorBiomeCTRL.changeBiome += updatePerlinSliders;
        }

        // Update is called once per frame
        void Update()
        {

        }


        public void acceptPerlin() {
            RedactorBiomeCTRL.main.SetSelectRuleParameters(
                sliderPerlinScale.slider.value, 
                (int)sliderPerlinOctaves.slider.value, 
                (int)sliderPerlinFreq.slider.value, 
                new Vector3(sliderPerlinScaleX.slider.value, sliderPerlinScaleY.slider.value, sliderPerlinScaleZ.slider.value));

            
        }

        void updatePerlinSliders() {
            BiomeData.GenRule genRule = RedactorBiomeCTRL.main.GetSelectGenRule();
            sliderPerlinScale.slider.value = genRule.scaleAll;

            sliderPerlinOctaves.slider.value = genRule.octaves;
            sliderPerlinFreq.slider.value = genRule.freq;
            sliderPerlinScaleX.slider.value = genRule.scaleX;
            sliderPerlinScaleY.slider.value = genRule.scaleY;
            sliderPerlinScaleZ.slider.value = genRule.scaleZ;

            sliderPerlinOctaves.SetValueText();
            sliderPerlinFreq.SetValueText();
            sliderPerlinScale.SetValueText();
            sliderPerlinScaleX.SetValueText();
            sliderPerlinScaleY.SetValueText();
            sliderPerlinScaleZ.SetValueText();
        }
    }
}
