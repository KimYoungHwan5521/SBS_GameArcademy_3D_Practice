using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager
{
    // 코루틴
    // 코루틴 쓰면 안좋았던 이유 -> 소요시간이 너무 많이 듦
    // 소요시간이 많이 들었던 이유 -> 모든 코루틴들을 왔다갔다 해야해서
    // 초기화 중에는 여러개를 동시에 돌려야한다
    // 로딩중 : 플레이어의 입력을 받아야 한다 (안받으면 응답없음 -> 강제종료)
    public virtual IEnumerator Initiate()
    {
        yield return null;
    }
    
    public virtual void Start()
    {

    }

    public virtual void ManagerUpdate(float deltaTime)
    {

    }
}
