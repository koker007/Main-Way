using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowAudio : MonoBehaviour
{
    [SerializeField]
    SliderCTRL VolumeAll;
    [SerializeField]
    SliderCTRL VolumeMusic;
    [SerializeField]
    SliderCTRL VolumeEffects;

    public const string keyVolumeAll = "AudioVolumeAll";
    public const string keyVolumeMusic = "AudioVolumeMusic";
    public const string keyVolumeEffect = "AudioVolumeEffect";
    public const string keyVolumeMAX = "AudioVolumeMAX";
    public const string keyVolumeOFF = "AudioVolumeOff";

    string strVolumeAll = "Volume all";
    string strVolumeMusic = "Volume music";
    string strVolumeEffect = "Volume effect";
    string strVolumeMAX = "MAX";
    string strVolumeOFF = "OFF";

    // Start is called before the first frame update
    void Start()
    {
        inicialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    void inicialize() {
        //Получить перевод из файла
        strVolumeAll = Language.GetTextFromKey(keyVolumeAll, strVolumeAll);
        strVolumeMusic = Language.GetTextFromKey(keyVolumeMusic, strVolumeMusic);
        strVolumeEffect = Language.GetTextFromKey(keyVolumeEffect, strVolumeEffect);

        strVolumeMAX = Language.GetTextFromKey(keyVolumeMAX, strVolumeMAX);
        strVolumeOFF = Language.GetTextFromKey(keyVolumeOFF, strVolumeOFF);

        //Установить текст в слайдер
        VolumeAll.SetDefaultText(strVolumeAll);
        VolumeMusic.SetDefaultText(strVolumeMusic);
        VolumeEffects.SetDefaultText(strVolumeEffect);

        //Установить слайдер в начальное значение
        UpdateSladers();

    }

    void UpdateSladers() {
        VolumeAll.slider.value = Settings.VolumeAll * 100;
        VolumeMusic.slider.value = Settings.VolumeMusic * 100;
        VolumeEffects.slider.value = Settings.VolumeEffects * 100;

    }

    void UpdateText() {
        if (VolumeAll.slider.value <= 0)
            VolumeAll.SetValueText(strVolumeOFF);
        else if (VolumeAll.slider.value >= 100)
            VolumeAll.SetValueText(strVolumeMAX);
        else
            VolumeAll.SetValueText(VolumeAll.slider.value.ToString());

        if (VolumeMusic.slider.value <= 0)
            VolumeMusic.SetValueText(strVolumeOFF);
        else if (VolumeMusic.slider.value >= 100)
            VolumeMusic.SetValueText(strVolumeMAX);
        else
            VolumeMusic.SetValueText(VolumeMusic.slider.value.ToString());

        if (VolumeEffects.slider.value <= 0)
            VolumeEffects.SetValueText(strVolumeOFF);
        else if (VolumeEffects.slider.value >= 100)
            VolumeEffects.SetValueText(strVolumeMAX);
        else
            VolumeEffects.SetValueText(VolumeEffects.slider.value.ToString());
    }


    //Установить громкость игры
    public void setVolumeAll() {
        Settings.VolumeAll = VolumeAll.slider.value / 100;
        UpdateText();
    }
    //Установить громкость музыки
    public void setVolumeMusic() {
        Settings.VolumeMusic = VolumeMusic.slider.value / 100;
        UpdateText();
    }
    //Установить громкость эффектов
    public void setVolumeEffect()
    {
        Settings.VolumeEffects = VolumeEffects.slider.value / 100;
        UpdateText();
    }

}
