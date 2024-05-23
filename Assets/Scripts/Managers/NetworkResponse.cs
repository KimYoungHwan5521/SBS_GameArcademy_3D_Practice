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
        // 매치메이킹 서버에 접속하려는 시도에 대한 응답
        Backend.Match.OnJoinMatchMakingServer = (args) =>
        {
            if(args.ErrInfo.Category != BackEnd.Tcp.ErrorCode.Success)
            {
                UIManager.ClaimError("매치 메이킹 실패", args.ErrInfo.Detail.ToString(), "확인", () => { UIManager.Close(UIManager.UIType.ErrorWindow); });
            }
        };

        // 매치메이킹 룸에 들어간 순간
        Backend.Match.OnMatchMakingRoomJoin = (args) =>
        {
            // 아직 매치메이킹 서버에 들어가만 있는 상태라면 -> 매치메이킹 룸으로
            if(GameManager.Instance.NetworkManager.currentState <= NetworkState.OnMatchRoom)
            {
                GameManager.Instance.NetworkManager.currentState = NetworkState.OnMatchRoom;
            }
        };

        Backend.Match.OnMatchMakingRoomCreate = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
                if (GameManager.Instance.NetworkManager.currentState <= NetworkState.OnMatchRoom)
                {
                    GameManager.Instance.NetworkManager.currentState = NetworkState.OnMatchRoom;
                }
                Debug.Log("매칭 룸이 정상적으로 생성되었습니다.");
            }
            else
            {
                UIManager.ClaimError("매치 메이킹 오류", args.ErrInfo.ToString(), "확인", null);
            }
        };

        // 매치메이킹 요청에 대한 응답
        Backend.Match.OnMatchMakingResponse = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_InProgress)
            {
                GameManager.Instance.NetworkManager.currentState = NetworkState.Matching;

            }
            else
            {
                UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason, "확인", null);

            }
        };

        Backend.Match.OnMatchResult = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
            }
            else
            {
                UIManager.ClaimError("매칭 오류", args.ErrInfo.ToString(), "확인", null);

            }
        };
    }

    static void MatchPoll(float deltaTime)
    {
        Backend.Match.Poll();
    }
}
