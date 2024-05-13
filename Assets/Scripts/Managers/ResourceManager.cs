using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ResourceManager : Manager
{
    // prefab이름을 넘겨주면 GameObject를 받아올 수 있게 : 딕셔너리
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
        // Dictionary는 사전이니까 사전에 등재를 하려면 사전이 있어야함
        // 만들기 전에 있는지 체크 : 무언가 이미 있다 => 함수를 종료
        // yield break : 코루틴 종료(함수의 return)
        if (prefabDictionary != null) yield break;

        // 초기화
        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject>();
        spriteDictionary = new Dictionary<ResourceEnum.Sprite, Sprite>();
        bgmDictionary = new Dictionary<ResourceEnum.BGM, AudioClip>();
        sfxDictionary = new Dictionary<ResourceEnum.SFX, AudioClip>();
        animDictionary = new Dictionary<ResourceEnum.Animation, AnimationClip>();

        resourceAmount = 0;
        resourceLoadCompleted = 0;
        // 로딩바 프로그래스 표시를 위해 카운트
        resourceAmount += ResourcePath.prefabPathArray.Length;
        resourceAmount += ResourcePath.spritePathArray.Length;
        resourceAmount += ResourcePath.bgmPathArray.Length;
        resourceAmount += ResourcePath.sfxPathArray.Length;
        resourceAmount += ResourcePath.animPathArray.Length;

        // 로드
        GameManager.ClaimLoadInfo("Audio Mixer");
        audioMixer = Load<AudioMixer>($"{ResourcePath.audioMixerPathArray}");

        // 이미 코루틴 안이기 때문에 StartCoroutine을 통해서 부를 필요가 없다.
        // 대신 yield return
        // yield return 안붙이면 함수 실행 안함.
        yield return Load(prefabDictionary, ResourcePath.prefabPathArray, "Prefab");
        yield return Load(spriteDictionary, ResourcePath.spritePathArray, "Sprite");
        yield return Load(bgmDictionary, ResourcePath.bgmPathArray, "BGM");
        yield return Load(sfxDictionary, ResourcePath.sfxPathArray, "SFX");
        yield return Load(animDictionary, ResourcePath.animPathArray, "Animation");
        GameManager.ClaimLoadInfo($"Load Completed");

        yield return null;
    }

    // 리소스를 불러올 때 그냥 Resources.Laod하지 말고 예외처리!
    // key는 언제나 Enum이니까, Enum으로 한정시켜줄 방법은 없을까요?
    // 자료형을 자유롭게 받긴하지만, 진짜 모든 것을 받을 필요는 없다.               key  자료형은 무조건 Enum만
    bool Load<key, value>(string path, Dictionary<key, value> dictionary) where key : Enum where value : UnityEngine.Object
    {
        try
        {
            string fileName = path.GetFileName();
            // Parse : 변환  (뭘로바꿀지, 어떤문자를, 결과물)
            if (Enum.TryParse(typeof(key), fileName, out object fileKey))
            {
                value loadData = Load<value>(path);
                if (loadData == null) return false;
                // 오브젝트 앞에 (자료형)대상 : 언패키징
                // 형변환 보다는 조금 무겁긴 함
                dictionary.Add((key)fileKey, loadData);
                return true;
            }
            else
            {
                Debug.LogError($"파일이름 \"{fileName}\"과 key \"{typeof(key)}\" 매치되지 않음!");
                // 새로운 Exception 정의
                Exception currentException = new Exception("Enum Mismatch");
                throw currentException;
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {path}를 로드하는데 실패!");
            return false;
        }

    }

    IEnumerator Load<key, value>(Dictionary<key, value> dictionary, string[] pathArray, string resourceType) where key : Enum where value : UnityEngine.Object
    {
        for (int i = 0; i < pathArray.Length; i++)
        {
            // 나는 이런걸 로드 하는 중이야
            GameManager.ClaimLoadInfo($"{resourceType} ({resourceLoadCompleted}/{resourceAmount})");
            if (Load(pathArray[i], dictionary))
            {
                resourceLoadCompleted++;
            }
            yield return null;
        }

    }


    // 로드를 할건데 path만 있는로드, 딕셔너리가 아니라 반환값으로 보내주는 로드
    // 위 딕셔너리 쓰는 Load는 지금 만들 Load를 가지고 딕셔너리에 넣는 기능만
    value Load<value>(string path) where value : UnityEngine.Object
    {
        // 로드를 실패하면 진행중이던 함수가 멈춘다.
        // try : 시도하다 실패하면 -> catch 실행
        try
        {
            value loadData = Resources.Load<value>(path);
            if (loadData == null) throw new Exception("File Not Found!");

            return loadData;

        }
        // 오류가 있었을 경우 실행
        catch(Exception e) 
        {
            Debug.LogError($"{e} : {path}를 로드하는데 실패!");
            return null;
        }
        // 오류가 있든 없든 실행
        // finally { } 
        
    }

    public static GameObject GetPrefab(ResourceEnum.Prefab wantPrefab)
    {
        // 딕셔너리에 Add하는 것은 간단
        // 불러오는 건?
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
