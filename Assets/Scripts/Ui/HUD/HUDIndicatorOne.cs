using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDIndicatorOne : MonoBehaviour
{
    [SerializeField]
    Image Back;
    [SerializeField]
    Image First;
    [SerializeField]
    Image Second;

    [SerializeField]
    Sprite[] FirstImages;

    // Start is called before the first frame update
    void Start()
    {
        iniBack();
        iniFirst();
        iniSecond();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartPos(HUDIndicator.Position position) {
        if (position == HUDIndicator.Position.Left)
        {
            if (Back) Back.fillOrigin = 0;
            if (First) First.fillOrigin = 0;
            if (Second) Second.fillOrigin = 0;
        }
        else {
            if (Back) Back.fillOrigin = 1;
            if (First) First.fillOrigin = 1;
            if (Second) Second.fillOrigin = 1;
        }
    }

    void iniBack() {
        if (Back.sprite == null) {
            Back.enabled = false;
        }
    }
    void iniFirst() {
        if (First.sprite == null && (FirstImages == null || FirstImages.Length == 0)) {
            First.enabled = false;
        }
    }
    void iniSecond() {
        if (Second.sprite == null) {
            Second.enabled = false;
        }
    }

    public void SetFirstPercent(int percent) {
        if (FirstImages != null && FirstImages.Length >= 1)
        {
            float percentF = percent / 100.0f;
            float partOneFrame = 1.0f / FirstImages.Length;
            int frameNum = (int)(percentF / partOneFrame);
            First.sprite = FirstImages[frameNum];
        }
        else
        {

            if (First.sprite)
            {
                First.fillAmount = PercentTo01(percent);
            }
            else
            {
                First.fillAmount = 0;
            }
        }
    }
    public void SetSecondPercent(int percent) {
        if (Second.sprite)
        {
            Second.fillAmount = PercentTo01(percent);
        }
        else
        {
            Second.fillAmount = 0;
        }

    }

    float PercentTo01(int percent) {
        if (percent > 100)
        {
            return 1f;
        }
        else if (percent > 90)
        {
            return 0.9f;
        }
        else if (percent > 80)
        {
            return 0.8f;
        }
        else if (percent > 70)
        {
            return 0.7f;
        }
        else if (percent > 60)
        {
            return 0.6f;
        }
        else if (percent > 50)
        {
            return 0.5f;
        }
        else if (percent > 40)
        {
            return 0.4f;
        }
        else if (percent > 30)
        {
            return 0.3f;
        }
        else if (percent > 20)
        {
            return 0.2f;
        }
        else if (percent > 10)
        {
            return 0.1f;
        }

        return 0;
    }
}
