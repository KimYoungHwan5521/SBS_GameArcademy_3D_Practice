using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Manager
{
    // 딕셔너리는 자료형이 두 개 필요
    // 프리팹 종류, 프리팹을 저장한 공간
    // 여러개를 저장 : 배열, 리스트, 큐, 스택, 딕셔너리, 트리, 그래프
    // "먼저 온 친구"가 먼저 나오면 좋겠다 : 큐
    // 지금 꺼놓은 애들을 저장할 딕셔너리
    Dictionary<ResourceEnum.Prefab, Queue<GameObject>> prefabDictionary = new Dictionary<ResourceEnum.Prefab, Queue<GameObject>>();

    // 그때그때 게임 오브젝트를 만드는 것은 시간이 오래걸리는 작업
    // 미리 게임오브젝트를 만들어놓고, 껐다 켰다하기
    // 한 번에 제대로 나와야하는 몬스터 갯수만 기억
    public IEnumerator ClaimPool(Dictionary<ResourceEnum.Prefab, int> input, int NumbersOnAFrame = 7)
    {
        if (NumbersOnAFrame < 1) NumbersOnAFrame = 1;
        int count = 0;
        // 딕셔너리의 내용물은 KeyValuePair로 저장
        foreach(var keyValue in input)
        {
            for(int i=0; i<keyValue.Value; i++)
            {
                ReadyStock(keyValue.Key);
                count++;
                GameManager.ClaimLoadInfo($"{keyValue.Key} : {count} / {keyValue.Value}");
                if(count % NumbersOnAFrame == 0)
                {
                    yield return null;
                }
            }
        }
        yield return null;
    }

    

    // 단 하나의 오브젝트를 생성하고 등록하는 과정
    void ReadyStock(ResourceEnum.Prefab target)
    {
        GameObject inst = GameObject.Instantiate(ResourceManager.GetPrefab(target));
        // 만들어놓고 꺼놓기
        inst.AddComponent<PoolingInfo>().SetInfo(target);
        inst.SetActive(false);

        // 꺼놓은 오브젝트는 Find로 찾을수 없다.
        // 그래서 미리 저장 해두어야 한다.
        // 사전에 Dictionary에서 체크해야할것 : 이 키를 가지고 내용을 만든 적이 있는지
        if(!prefabDictionary.TryGetValue(target, out Queue<GameObject> result))
        {
            // 만든적 없었다면
            // 등록
            // Queue를 먼저 생성
            result = new Queue<GameObject>();
            prefabDictionary.Add(target, result);
        }
        else if(result == null)
        {
            result = new Queue<GameObject>();
            prefabDictionary[target] = result;
        }
        result.Enqueue(inst);
    }

    public static GameObject Instantiate(ResourceEnum.Prefab target)
    {
        if(!GameManager.Instance.WorldManager || GameManager.Instance.WorldManager.PoolManager == null)
        {
            Debug.LogWarning($"tried instantiate \"{target}\" before Manager Initiate");
            return null;
        }

        PoolManager poolManager = GameManager.Instance.WorldManager.PoolManager;

        // 새로 생성하는 과정은 아님
        // 풀에서 꺼져있는걸 켜주기
        // 꺼져있는 대상을 찾아보자
        if(!poolManager.prefabDictionary.TryGetValue(target, out Queue<GameObject> resultQueue))
        {
            poolManager.ReadyStock(target);
            poolManager.prefabDictionary.TryGetValue(target, out resultQueue);
        }
        else if(resultQueue.Count == 0)
        {
            // resultQueue가 있긴 있는데 비어있는 경우
            poolManager.ReadyStock(target);
            poolManager.prefabDictionary.TryGetValue(target, out resultQueue);
        }

        GameObject result = resultQueue.Dequeue();
        result.transform.eulerAngles = new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
        result.SetActive(true);
        return result;

    }

    public static void Destroy(GameObject target)
    {
        if(target.TryGetComponent(out PoolingInfo info))
        {
            if(GameManager.Instance.WorldManager.PoolManager.prefabDictionary.TryGetValue(info.Origin, out Queue<GameObject> resultQueue))
            {
                resultQueue.Enqueue(target);
                target.SetActive(false);
            }
            else
            {
                GameObject.Destroy(target);
            }
        }
        else
        {
            GameObject.Destroy(target);
        }
    }

}
