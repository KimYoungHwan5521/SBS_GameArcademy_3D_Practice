using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Manager
{
    // ��ȯ���� ���� �Լ�
    // System.Action<float> AudioEffectUpdate;
    UpdateFunction AudioEffectUpdate;


    AudioMixer currentMixer;
    AudioMixerGroup AMGmaster;
    AudioMixerGroup AMGbgm;
    AudioMixerGroup AMGsfx;

    // ������ "����" ����������
    AudioSource[] bgmArray = new AudioSource[2];
    const int SFXMaxNumber = 10;
    Queue<AudioSource> sfxQueue = new Queue<AudioSource>();

    public override IEnumerator Initiate()
    {
        // ���ҽ� �Ŵ������׼� �ͼ��� �����´�
        currentMixer = ResourceManager.Mixer;
        if (!currentMixer) Debug.LogWarning("Audio Mixer has Not Found!");
        AMGmaster = currentMixer.FindMatchingGroups("Master")[0];
        AMGbgm = currentMixer.FindMatchingGroups("Master")[1];
        // �Ǵ� AMGbgm = currentMixer.FindMatchingGroups("BGM")[0];
        AMGsfx = currentMixer.FindMatchingGroups("Master")[2];
        if (!AMGmaster) Debug.LogWarning("Audio Mixer Master has Not Found!");
        if (!AMGbgm) Debug.LogWarning("Audio Mixer BGM has Not Found!");
        if (!AMGsfx) Debug.LogWarning("Audio Mixer SFX has Not Found!");

        // ����� �ҽ��� �־� ���� �� �ִ� ������Ʈ
        GameObject bgmCarrier = new GameObject("BGM Carrier", typeof(AudioSource), typeof(AudioSource));
        bgmCarrier.transform.SetParent(GameObject.Find("GameManager").transform);
        bgmArray = bgmCarrier.GetComponents<AudioSource>();

        // �ϴ� ���鼭 Audio Mixer Group�� �غ�
        for(int i = 0; i < bgmArray.Length; i++)
        {
            bgmArray[i].outputAudioMixerGroup = AMGbgm;
            bgmArray[i].loop= true;
            bgmArray[i].playOnAwake= false;
            bgmArray[i].maxDistance = float.MaxValue;
            bgmArray[i].minDistance = float.MaxValue;
        }
        // ���� ����Ʈ
        // ������ ���� ������Ʈ�� ���������� ����
        // �� ���� ���ÿ� �߻��� �� ������
        for(int i = 0; i< SFXMaxNumber; i++)
        {
            GameObject sfxCarriers = new GameObject("SFX Carrier", typeof(AudioSource));
            // �� ���� ����Ʈ������ ���� ������ҽ��� Audio Mixer Group�� SFX��
            AudioSource currentSource = sfxCarriers.GetComponent<AudioSource>();
            currentSource.outputAudioMixerGroup = AMGsfx;
            currentSource.playOnAwake= false;
            sfxQueue.Enqueue(currentSource);
        }

        yield return null;
    }

    public override void ManagerUpdate(float deltaTime)
    {
        // AudioEffectUpdate += (t) => { };
        // ����� ����Ʈ�� �����ϰ� �;��
        AudioEffectUpdate?.Invoke(deltaTime);
    }


    // BGM�� 0���� 1�� �ö󰡰�
    // 1���� 0���� ������
    public void UpdateBGMMixer(float deltaTime)
    {
        bgmArray[0].volume = Mathf.SmoothStep(bgmArray[0].volume, 1, deltaTime * 5);
        bgmArray[1].volume = Mathf.SmoothStep(bgmArray[1].volume, 0, deltaTime * 5);
        if (bgmArray[0].volume == 1)
        {
            AudioEffectUpdate -= UpdateBGMMixer;
        }
    }

    public static void Play(ResourceEnum.BGM wantBGM)
    {
        // 0 : �÷����� ���
        // 1 : �÷������� ���
        // ���� �÷����� ���[0]�� [1]������
        SoundManager SM = GameManager.Instance.SoundManager;
        SM.bgmArray[1].clip = SM.bgmArray[0].clip;
        SM.bgmArray[1].volume = SM.bgmArray[0].volume;
        SM.bgmArray[1].time = SM.bgmArray[0].time;

        SM.bgmArray[0].clip = ResourceManager.GetBGM(wantBGM);
        // ����� ����, ����ð��� 0����
        SM.bgmArray[0].volume = 0;
        SM.bgmArray[0].time = 0;
        SM.bgmArray[0].Play();

        // ����� ����Ʈ�� ����� �����ִ� ����Ʈ
        // Action���� ���� ���� �ִ� ���� : �̹� �ִ°��� �����ϰ� �ϳ��� �ϰ�
        SM.AudioEffectUpdate -= SM.UpdateBGMMixer;
        SM.AudioEffectUpdate += SM.UpdateBGMMixer;
    }

    public static void Play(ResourceEnum.SFX wantSFX, Vector3 soundOrigin)
    {
        AudioClip wantClip = ResourceManager.GetSFX(wantSFX);
        SoundManager SM = GameManager.Instance.SoundManager;
        if (SM == null) return;
        if (SM.sfxQueue.TryDequeue(out AudioSource curSource))
        {
            curSource.clip = wantClip;
            curSource.transform.position = soundOrigin;
            curSource.Play();
            SM.sfxQueue.Enqueue(curSource);
        }
    }

    public void SetSoundInfo(ref OptionManager.OptionData optionData)
    {
        currentMixer.SetFloat("Master", Mathf.Log10(optionData.volumeMaster * 10) * 20);
        currentMixer.SetFloat("BGM", Mathf.Log10(optionData.volumeBGM * 10) * 20);
        currentMixer.SetFloat("SFX", Mathf.Log10(optionData.volumeSFX * 10) * 20);
    }

}
