using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    // ���� ������
    ResourceEnum.Prefab origin;
    public ResourceEnum.Prefab Origin => origin;
    Queue<GameObject> queue;
    public Queue<GameObject> _Queue => queue;

    [SerializeField]float lifespan = -1;
    public float Lifespan
    {
        get => lifespan; 
        set
        {
            // ������ 10�� �ڿ� ������� �ߴ�
            // �׷��� �ٸ� �ְ� 200�� �ڿ� ������� �ߴ�
            if (lifespan == -1) lifespan = value;
            else if(value >= 0) lifespan = Mathf.Min(Lifespan, value);
        }
    }

    protected override void MyDestroy()
    {
        // �ı��� �Ŀ� �ʱ�ȭ
        lifespan = -1;
    }

    protected override void MyUpdate(float deltaTime)
    {
        if(lifespan >=0)
        {
            lifespan -= deltaTime;
            if(lifespan<= 0)
            {
                PoolManager.Destroy(this);
            }
        }
    }

    public void SetInfo(ResourceEnum.Prefab wantOrigin, Queue<GameObject> wantQueue)
    {
        origin= wantOrigin;
        queue= wantQueue;
    }



}
