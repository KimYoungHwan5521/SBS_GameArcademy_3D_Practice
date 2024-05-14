using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Manager
{
    // 반환값이 없는 함수
    // System.Action<float> AudioEffectUpdate;
    UpdateFunction AudioEffectUpdate;


    AudioMixer currentMixer;
    AudioMixerGroup AMGmaster;
    AudioMixerGroup AMGbgm;
    AudioMixerGroup AMGsfx;

    // 무엇이 "현재" 실행중인지
    AudioSource[] bgmArray = new AudioSource[2];
    const int SFXMaxNumber = 10;
    Queue<AudioSource> sfxQueue = new Queue<AudioSource>();

    public override IEnumerator Initiate()
    {
        // 리소스 매니저한테서 믹서를 가져온다
        currentMixer = ResourceManager.Mixer;
        if (!currentMixer) Debug.LogWarning("Audio Mixer has Not Found!");
        AMGmaster = currentMixer.FindMatchingGroups("Master")[0];
        AMGbgm = currentMixer.FindMatchingGroups("Master")[1];
        // 또는 AMGbgm = currentMixer.FindMatchingGroups("BGM")[0];
        AMGsfx = currentMixer.FindMatchingGroups("Master")[2];
        if (!AMGmaster) Debug.LogWarning("Audio Mixer Master has Not Found!");
        if (!AMGbgm) Debug.LogWarning("Audio Mixer BGM has Not Found!");
        if (!AMGsfx) Debug.LogWarning("Audio Mixer SFX has Not Found!");

        // 오디오 소스를 넣어 놓을 수 있는 오브젝트
        GameObject bgmCarrier = new GameObject("BGM Carrier", typeof(AudioSource), typeof(AudioSource));
        bgmCarrier.transform.SetParent(GameObject.Find("GameManager").transform);
        bgmArray = bgmCarrier.GetComponents<AudioSource>();

        // 싹다 돌면서 Audio Mixer Group을 준비
        for(int i = 0; i < bgmArray.Length; i++)
        {
            bgmArray[i].outputAudioMixerGroup = AMGbgm;
            bgmArray[i].loop= true;
            bgmArray[i].playOnAwake= false;
            bgmArray[i].maxDistance = float.MaxValue;
            bgmArray[i].minDistance = float.MaxValue;
        }
        // 사운드 이펙트
        // 각각의 사운드 오브젝트가 독립적으로 존재
        // 몇 개나 동시에 발생할 수 있을지
        for(int i = 0; i< SFXMaxNumber; i++)
        {
            GameObject sfxCarriers = new GameObject("SFX Carrier", typeof(AudioSource));
            // 그 사운드 이펙트용으로 만든 오디오소스도 Audio Mixer Group을 SFX로
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
        // 오디오 이펙트를 실행하고 싶어요
        AudioEffectUpdate?.Invoke(deltaTime);
    }


    // BGM이 0번이 1로 올라가고
    // 1번이 0으로 내려감
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
        // 0 : 플레이할 브금
        // 1 : 플레이중인 브금
        // 현재 플레이할 브금[0]을 [1]번으로
        SoundManager SM = GameManager.Instance.SoundManager;
        SM.bgmArray[1].clip = SM.bgmArray[0].clip;
        SM.bgmArray[1].volume = SM.bgmArray[0].volume;
        SM.bgmArray[1].time = SM.bgmArray[0].time;

        SM.bgmArray[0].clip = ResourceManager.GetBGM(wantBGM);
        // 새브금 볼륨, 재생시간은 0부터
        SM.bgmArray[0].volume = 0;
        SM.bgmArray[0].time = 0;
        SM.bgmArray[0].Play();

        // 오디오 이펙트에 브금을 섞어주는 이펙트
        // Action에서 먼저 빼고 넣는 이유 : 이미 있는것은 제거하고 하나만 하게
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
