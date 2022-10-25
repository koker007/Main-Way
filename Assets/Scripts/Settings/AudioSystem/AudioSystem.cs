using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Отвечает за воспроизведение звуков и музыки для 2д источников звука
public class AudioSystem : MonoBehaviour
{
    static AudioSystem main;

    //Список источников
    List<Audiopack> audioPacks = new List<Audiopack>();

    //Параметры
    public class Parameters{

        public float timeToDelete;
        public bool needDelete;
        public float volume;
        public float speedDelete; //Скорость удаления \ затухания звука
    }

    public class Audiopack{
        public AudioSource source;
        public Parameters parameters;
    }
    // Start is called before the first frame update

    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSources();
    }

    //проверяем все источники
    void UpdateSources() {
        List<Audiopack> audiopacksNew = new List<Audiopack>();

        foreach (Audiopack pack in audioPacks) {
            if (pack == null)
                continue;

            if (pack.source == null)
                continue;

            //Если нужно удалить плавно, отнимаем громкость
            if (pack.parameters.needDelete)
                pack.parameters.volume -= pack.parameters.speedDelete * Time.unscaledDeltaTime;

            //Удаляем если надо удалить, если время вышло
            if (pack.parameters.timeToDelete <= Time.time ||
                (pack.parameters.needDelete && pack.parameters.volume <= 0))
                Destroy(pack.source);

            //Изменяем громкость
            pack.source.volume = pack.parameters.volume * Settings.VolumeAll * Settings.VolumeMusic;

            audiopacksNew.Add(pack);
        }

        audioPacks = audiopacksNew;


    }








    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////Статические
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //Создаем новый источник
    static Audiopack AddSource(AudioClip clip, Parameters parameters) {

        //Создаем пак звука
        Audiopack audiopack = new Audiopack();
        //Создаем компонент источника у игрового объекта
        audiopack.source = main.gameObject.AddComponent<AudioSource>();
        //Добавляем параметры
        audiopack.parameters = parameters;

        //Воспроизводим звук
        audiopack.source.clip = clip;
        audiopack.source.Play();

        main.audioPacks.Add(audiopack);
        return audiopack;
    }

    //Воспроизвести звук
    static public Audiopack PlaySound(AudioClip clip) {
        Parameters parameters = new Parameters();

        parameters.volume = 1;
        parameters.timeToDelete = Time.time + clip.length;
        parameters.needDelete = false;

        //Добавляем источник звука, запихиваем параметры
        return AddSource(clip, parameters);
    }
}
