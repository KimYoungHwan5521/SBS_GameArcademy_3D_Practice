using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class InGameWorldManager : WorldManager
{
    protected override IEnumerator Initiate()
    {
        yield return base.Initiate();
        // 월드의 기본요소를 로드했는데 로딩창이 끝나지 않는 경우
        // : 모든 플레이어가 로드가 끝나지 않았을 때
        Backend.Match.SendDataToInGameRoom("asbsdfsdjfiselfsjll".ToByteArray_UTF8());
    }
}
