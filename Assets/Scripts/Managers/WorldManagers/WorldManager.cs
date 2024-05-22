using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    CameraManager cameraManager;
    public CameraManager CameraManager => cameraManager;
    CharacterManager characterManager;
    public CharacterManager CharacterManager => characterManager;
    PoolManager poolmanager;
    public PoolManager PoolManager => poolmanager;

    [SerializeField] List<ResourceEnum.Prefab> prefabs;
    [SerializeField] List<int> prefabCounts;

    protected virtual void Start()
    {
        if(GameManager.Instance == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }
        GameManager.ManagerStarts += ()=> { GameManager.WorldChange(this); };
    }

    protected virtual IEnumerator Initiate()
    {
        // 일단 로딩 시작
        GameManager.ClaimLoadInfo("New World");

        Dictionary<ResourceEnum.Prefab, int> prefabCountDictionary = new();
        // 딕셔너리 안에 리스트자료들을 넣어주기
        if(prefabCounts.Count != prefabs.Count)
        {
            Debug.LogWarning("Prefabs\' and prefabCounts\' number not match!");
        }
        for(int i=0;i<prefabCounts.Count;i++)
        {
            ResourceEnum.Prefab currentPrefab = prefabs[i];

            if(prefabCountDictionary.ContainsKey(currentPrefab))
            {
                if(i < prefabCounts.Count)
                {
                    prefabCountDictionary[currentPrefab] = Mathf.Max(prefabCountDictionary[currentPrefab], prefabCounts[i]);
                }
            }
            else prefabCountDictionary.Add(currentPrefab, i < prefabCounts.Count ?  prefabCounts[i] : 1);
        }

        poolmanager= new PoolManager();
        yield return poolmanager.Initiate();
        yield return poolmanager.ClaimPool(prefabCountDictionary);

        characterManager= new CharacterManager();
        yield return characterManager.Initiate();
        cameraManager= new CameraManager();
        yield return cameraManager.Initiate();

        GameManager.CloseLoadInfo();
        yield return null;
    }

    public void Delete()
    {

    }

    public void Create()
    {
        StartCoroutine(Initiate());
    }


}
