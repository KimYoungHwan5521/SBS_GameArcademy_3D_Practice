using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ResourceManager : Manager
{
    // prefab�̸��� �Ѱ��ָ� GameObject�� �޾ƿ� �� �ְ� : ��ųʸ�
    static Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;
    static Dictionary<ResourceEnum.Sprite, Sprite> spriteDictionary;
    static Dictionary<ResourceEnum.BGM, AudioClip> bgmDictionary;
    static Dictionary<ResourceEnum.SFX, AudioClip> sfxDictionary;
    static Dictionary<ResourceEnum.Animation, AnimationClip> animDictionary;

    static AudioMixer audioMixer;
    public static AudioMixer Mixer => audioMixer;

    public static int resourceAmount = 0;
    public static int resourceLoadCompleted = 0;

    public override IEnumerator Initiate()
    {
        // Dictionary�� �����̴ϱ� ������ ���縦 �Ϸ��� ������ �־����
        // ����� ���� �ִ��� üũ : ���� �̹� �ִ� => �Լ��� ����
        // yield break : �ڷ�ƾ ����(�Լ��� return)
        if (prefabDictionary != null) yield break;

        // �ʱ�ȭ
        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject>();
        spriteDictionary = new Dictionary<ResourceEnum.Sprite, Sprite>();
        bgmDictionary = new Dictionary<ResourceEnum.BGM, AudioClip>();
        sfxDictionary = new Dictionary<ResourceEnum.SFX, AudioClip>();
        animDictionary = new Dictionary<ResourceEnum.Animation, AnimationClip>();

        resourceAmount = 0;
        resourceLoadCompleted = 0;
        // �ε��� ���α׷��� ǥ�ø� ���� ī��Ʈ
        resourceAmount += ResourcePath.prefabPathArray.Length;
        resourceAmount += ResourcePath.spritePathArray.Length;
        resourceAmount += ResourcePath.bgmPathArray.Length;
        resourceAmount += ResourcePath.sfxPathArray.Length;
        resourceAmount += ResourcePath.animPathArray.Length;

        // �ε�
        GameManager.ClaimLoadInfo("Audio Mixer");
        audioMixer = Load<AudioMixer>($"{ResourcePath.audioMixerPathArray}");

        // �̹� �ڷ�ƾ ���̱� ������ StartCoroutine�� ���ؼ� �θ� �ʿ䰡 ����.
        // ��� yield return
        // yield return �Ⱥ��̸� �Լ� ���� ����.
        yield return Load(prefabDictionary, ResourcePath.prefabPathArray, "Prefab");
        yield return Load(spriteDictionary, ResourcePath.spritePathArray, "Sprite");
        yield return Load(bgmDictionary, ResourcePath.bgmPathArray, "BGM");
        yield return Load(sfxDictionary, ResourcePath.sfxPathArray, "SFX");
        yield return Load(animDictionary, ResourcePath.animPathArray, "Animation");
        GameManager.ClaimLoadInfo($"Load Completed");

        yield return null;
    }

    // ���ҽ��� �ҷ��� �� �׳� Resources.Laod���� ���� ����ó��!
    // key�� ������ Enum�̴ϱ�, Enum���� ���������� ����� �������?
    // �ڷ����� �����Ӱ� �ޱ�������, ��¥ ��� ���� ���� �ʿ�� ����.               key  �ڷ����� ������ Enum��
    bool Load<key, value>(string path, Dictionary<key, value> dictionary) where key : Enum where value : UnityEngine.Object
    {
        try
        {
            string fileName = path.GetFileName();
            // Parse : ��ȯ  (���ιٲ���, ����ڸ�, �����)
            if (Enum.TryParse(typeof(key), fileName, out object fileKey))
            {
                value loadData = Load<value>(path);
                if (loadData == null) return false;
                // ������Ʈ �տ� (�ڷ���)��� : ����Ű¡
                // ����ȯ ���ٴ� ���� ���̱� ��
                dictionary.Add((key)fileKey, loadData);
                return true;
            }
            else
            {
                Debug.LogError($"�����̸� \"{fileName}\"�� key \"{typeof(key)}\" ��ġ���� ����!");
                // ���ο� Exception ����
                Exception currentException = new Exception("Enum Mismatch");
                throw currentException;
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {path}�� �ε��ϴµ� ����!");
            return false;
        }

    }

    IEnumerator Load<key, value>(Dictionary<key, value> dictionary, string[] pathArray, string resourceType) where key : Enum where value : UnityEngine.Object
    {
        for (int i = 0; i < pathArray.Length; i++)
        {
            // ���� �̷��� �ε� �ϴ� ���̾�
            GameManager.ClaimLoadInfo($"{resourceType} ({resourceLoadCompleted}/{resourceAmount})");
            if (Load(pathArray[i], dictionary))
            {
                resourceLoadCompleted++;
            }
            yield return null;
        }

    }


    // �ε带 �Ұǵ� path�� �ִ·ε�, ��ųʸ��� �ƴ϶� ��ȯ������ �����ִ� �ε�
    // �� ��ųʸ� ���� Load�� ���� ���� Load�� ������ ��ųʸ��� �ִ� ��ɸ�
    value Load<value>(string path) where value : UnityEngine.Object
    {
        // �ε带 �����ϸ� �������̴� �Լ��� �����.
        // try : �õ��ϴ� �����ϸ� -> catch ����
        try
        {
            value loadData = Resources.Load<value>(path);
            if (loadData == null) throw new Exception("File Not Found!");

            return loadData;

        }
        // ������ �־��� ��� ����
        catch(Exception e) 
        {
            Debug.LogError($"{e} : {path}�� �ε��ϴµ� ����!");
            return null;
        }
        // ������ �ֵ� ���� ����
        // finally { } 
        
    }

    public static GameObject GetPrefab(ResourceEnum.Prefab wantPrefab)
    {
        // ��ųʸ��� Add�ϴ� ���� ����
        // �ҷ����� ��?
        if (prefabDictionary.TryGetValue(wantPrefab, out GameObject prefab))
        {
            return prefab;
        }
        return null;
    }

    public static Sprite GetSprite(ResourceEnum.Sprite wantSprite)
    {
        if(spriteDictionary.TryGetValue(wantSprite, out Sprite sprite))
        {
            return sprite;
        }
        return null;
    }

    public static AudioClip GetBGM(ResourceEnum.BGM wantBGM)
    {
        if(bgmDictionary.TryGetValue(wantBGM, out AudioClip bgm))
        {
            return bgm;
        }
        return null;
    }

    public static AudioClip GetSFX(ResourceEnum.SFX wantSFX)
    {
        if(sfxDictionary.TryGetValue(wantSFX, out AudioClip sfx))
        {
            return sfx;
        }
        return null;
    }

    public static AnimationClip GetAnimation(ResourceEnum.Animation wantAnimation)
    {
        if(animDictionary.TryGetValue(wantAnimation, out AnimationClip animation))
        {
            return animation;
        }
        return null;
    }

}
