using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    // 원본 프리팹
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
            // 누군가 10년 뒤에 죽으라고 했다
            // 그런데 다른 애가 200년 뒤에 죽으라고 했다
            if (lifespan == -1) lifespan = value;
            else if(value >= 0) lifespan = Mathf.Min(Lifespan, value);
        }
    }

    protected override void MyDestroy()
    {
        // 파괴된 후에 초기화
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
