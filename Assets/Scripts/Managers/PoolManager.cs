using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Manager
{
    // ��ųʸ��� �ڷ����� �� �� �ʿ�
    // ������ ����, �������� ������ ����
    // �������� ���� : �迭, ����Ʈ, ť, ����, ��ųʸ�, Ʈ��, �׷���
    // "���� �� ģ��"�� ���� ������ ���ڴ� : ť
    // ���� ������ �ֵ��� ������ ��ųʸ�
    Dictionary<ResourceEnum.Prefab, Queue<GameObject>> prefabDictionary = new Dictionary<ResourceEnum.Prefab, Queue<GameObject>>();

    // �׶��׶� ���� ������Ʈ�� ����� ���� �ð��� �����ɸ��� �۾�
    // �̸� ���ӿ�����Ʈ�� ��������, ���� �״��ϱ�
    // �� ���� ����� ���;��ϴ� ���� ������ ���
    public IEnumerator ClaimPool(Dictionary<ResourceEnum.Prefab, int> input, int NumbersOnAFrame = 7)
    {
        if (NumbersOnAFrame < 1) NumbersOnAFrame = 1;
        int count = 0;
        // ��ųʸ��� ���빰�� KeyValuePair�� ����
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

    

    // �� �ϳ��� ������Ʈ�� �����ϰ� ����ϴ� ����
    void ReadyStock(ResourceEnum.Prefab target)
    {
        GameObject inst = GameObject.Instantiate(ResourceManager.GetPrefab(target));
        // �������� ������
        inst.AddComponent<PoolingInfo>().SetInfo(target);
        inst.SetActive(false);

        // ������ ������Ʈ�� Find�� ã���� ����.
        // �׷��� �̸� ���� �صξ�� �Ѵ�.
        // ������ Dictionary���� üũ�ؾ��Ұ� : �� Ű�� ������ ������ ���� ���� �ִ���
        if(!prefabDictionary.TryGetValue(target, out Queue<GameObject> result))
        {
            // ������ �����ٸ�
            // ���
            // Queue�� ���� ����
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

        // ���� �����ϴ� ������ �ƴ�
        // Ǯ���� �����ִ°� ���ֱ�
        // �����ִ� ����� ã�ƺ���
        if(!poolManager.prefabDictionary.TryGetValue(target, out Queue<GameObject> resultQueue))
        {
            poolManager.ReadyStock(target);
            poolManager.prefabDictionary.TryGetValue(target, out resultQueue);
        }
        else if(resultQueue.Count == 0)
        {
            // resultQueue�� �ֱ� �ִµ� ����ִ� ���
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
