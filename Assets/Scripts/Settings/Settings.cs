using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//main game settings
public class Settings : MonoBehaviour
{
    public static Settings main;

    bool needRestart = false; //Нужна ли перезагрузка чтобы применить изменения

    [SerializeField]
    string language = "English";

    [SerializeField]
    float volumeAll = 1;
    [SerializeField]
    float volumeMusic = 1;
    [SerializeField]
    float volumeEffects = 1;

    static public float VolumeAll {
        get {
            return main.volumeAll;
        }
        set {
            main.volumeAll = value;
            PlayerPrefs.SetFloat(WindowAudio.keyVolumeAll, value);
        }
    }
    static public float VolumeMusic
    {
        get
        {
            return main.volumeMusic;
        }
        set
        {
            main.volumeMusic = value;
            PlayerPrefs.SetFloat(WindowAudio.keyVolumeMusic, value);
        }
    }
    static public float VolumeEffects
    {
        get
        {
            return main.volumeEffects;
        }
        set
        {
            main.volumeEffects = value;
            PlayerPrefs.SetFloat(WindowAudio.keyVolumeEffect, value);
        }
    }


    [SerializeField]
    public int distance = 3; //Дальность прорисовки


    // Start is called before the first frame update
    void Start()
    {
        main = this;
        Language.LoadLanguage("English");

        Load();
    }

    void Load() {

        //Язык
        language = PlayerPrefs.GetString(Language.directive, language);

        //Звук
        volumeAll = PlayerPrefs.GetFloat(WindowAudio.keyVolumeAll, volumeAll);
        volumeMusic = PlayerPrefs.GetFloat(WindowAudio.keyVolumeMusic, volumeMusic);
        volumeEffects = PlayerPrefs.GetFloat(WindowAudio.keyVolumeEffect, volumeEffects);

        
    }

    public void SaveLanguage(string language) {
        if (this.language == language) 
            return;

        needRestart = true;
        this.language = language;
        PlayerPrefs.SetString(Language.directive, language);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static float timeLastUnloadRam = 0;
    static public void TestUnloadRam() {
        if (timeLastUnloadRam + 600 > Time.unscaledTime)
            return;

        timeLastUnloadRam = Time.unscaledTime;
        Resources.UnloadUnusedAssets();
    }
}
