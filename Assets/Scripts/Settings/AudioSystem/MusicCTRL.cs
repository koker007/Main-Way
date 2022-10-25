using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Отвечает за воспроизведение музыки
public class MusicCTRL : MonoBehaviour
{
    AudioListener listener;

    [SerializeField]
    AudioClip MainTheme;

    AudioSystem.Audiopack music;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMusic();
        UpdateListener();
    }

    //проверка слушателя
    void UpdateListener() {
        //Если слушатель есть
        if (listener)
        {
            //Если игрок есть надо удалить слушателя //ТК слушатель - игрок
            if (PlayerCTRL.local)
                Destroy(listener);
        }
        //Если слушатель отсутствует
        else {
            //Если игрока нет надо создать слушателя
            if (!PlayerCTRL.local)
                listener = gameObject.AddComponent<AudioListener>();
        }
    }

    void UpdateMusic() {
        //Если игра запущена
        if (Gameplay.main != null && Gameplay.main.gameObject.activeSelf)
        {
            //Если играет музыка главного меню
            if (music != null && music.source != null && music.source.clip == MainTheme) {
                music.parameters.needDelete = true;
                music.parameters.speedDelete = 1/5f;
            }
        }
        //Если геймплей не активен
        else {
            //и музыка не играет
            if (music == null || music.source == null) {
                music = AudioSystem.PlaySound(MainTheme);
            }
        }
    }
}
