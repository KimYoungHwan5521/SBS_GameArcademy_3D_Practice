using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.SceneManagement;

// NetworkResponse
public partial class NetworkManager : Manager
{
    // 만약 매치메이킹이 어떻게 되었는지 정보를 들었다면
    // 백엔드가 저희한테 "이런일이 일어났다"는 것을 알려준다 : "콜백" 함수를 등록
    void RegistCallBackFunction()
    {
        // 매치메이킹 요청에 대한 응답
        Backend.Match.OnMatchMakingResponse = (args) =>
        {
            UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason, "확인", null);
        };
    }

    static void MatchPoll(float deltaTime)
    {
        Backend.Match.Poll();
    }
}
