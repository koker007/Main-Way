using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� �� ��������������� ������ � ������ ��� 2� ���������� �����
public class AudioSystem : MonoBehaviour
{
    static AudioSystem main;

    //������ ����������
    List<Audiopack> audioPacks = new List<Audiopack>();

    //���������
    public class Parameters{

        public float timeToDelete;
        public bool needDelete;
        public float volume;
        public float speedDelete; //�������� �������� \ ��������� �����
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

    //��������� ��� ���������
    void UpdateSources() {
        List<Audiopack> audiopacksNew = new List<Audiopack>();

        foreach (Audiopack pack in audioPacks) {
            if (pack == null)
                continue;

            if (pack.source == null)
                continue;

            //���� ����� ������� ������, �������� ���������
            if (pack.parameters.needDelete)
                pack.parameters.volume -= pack.parameters.speedDelete * Time.unscaledDeltaTime;

            //������� ���� ���� �������, ���� ����� �����
            if (pack.parameters.timeToDelete <= Time.time ||
                (pack.parameters.needDelete && pack.parameters.volume <= 0))
                Destroy(pack.source);

            //�������� ���������
            pack.source.volume = pack.parameters.volume * Settings.VolumeAll * Settings.VolumeMusic;

            audiopacksNew.Add(pack);
        }

        audioPacks = audiopacksNew;


    }








    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////�����������
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //������� ����� ��������
    static Audiopack AddSource(AudioClip clip, Parameters parameters) {

        //������� ��� �����
        Audiopack audiopack = new Audiopack();
        //������� ��������� ��������� � �������� �������
        audiopack.source = main.gameObject.AddComponent<AudioSource>();
        //��������� ���������
        audiopack.parameters = parameters;

        //������������� ����
        audiopack.source.clip = clip;
        audiopack.source.Play();

        main.audioPacks.Add(audiopack);
        return audiopack;
    }

    //������������� ����
    static public Audiopack PlaySound(AudioClip clip) {
        Parameters parameters = new Parameters();

        parameters.volume = 1;
        parameters.timeToDelete = Time.time + clip.length;
        parameters.needDelete = false;

        //��������� �������� �����, ���������� ���������
        return AddSource(clip, parameters);
    }
}
