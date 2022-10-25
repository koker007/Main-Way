using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� �� ��������������� ������
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

    //�������� ���������
    void UpdateListener() {
        //���� ��������� ����
        if (listener)
        {
            //���� ����� ���� ���� ������� ��������� //�� ��������� - �����
            if (PlayerCTRL.local)
                Destroy(listener);
        }
        //���� ��������� �����������
        else {
            //���� ������ ��� ���� ������� ���������
            if (!PlayerCTRL.local)
                listener = gameObject.AddComponent<AudioListener>();
        }
    }

    void UpdateMusic() {
        //���� ���� ��������
        if (Gameplay.main != null && Gameplay.main.gameObject.activeSelf)
        {
            //���� ������ ������ �������� ����
            if (music != null && music.source != null && music.source.clip == MainTheme) {
                music.parameters.needDelete = true;
                music.parameters.speedDelete = 1/5f;
            }
        }
        //���� �������� �� �������
        else {
            //� ������ �� ������
            if (music == null || music.source == null) {
                music = AudioSystem.PlaySound(MainTheme);
            }
        }
    }
}
