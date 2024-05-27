using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.SceneManagement;
using System.ComponentModel.Design;
using BackEnd.Tcp;

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
                UIManager.ClaimError("매치 메이킹 실패", args.ErrInfo.Detail.ToString(), "확인", null);
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
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
                ClaimJoinRoom(args.MatchCardIndate, args.RoomInfo);
            }
            else if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_MatchMakingCanceled)
            {
                UIManager.ClaimError("취소", "매칭이 취소되었습니다.", "확인", null);

            }
            else if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_InProgress)
            {
                GameManager.Instance.NetworkManager.currentState = NetworkState.Matching;
                UIManager.ClaimError("매칭 시작", "매칭이 시작되었습니다.", "확인", null);

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

        //////////// 인게임 ///////////
        Backend.Match.OnSessionJoinInServer = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorInfo.Success)
            {
                currentState = NetworkState.OnGameServer;
                UIManager.ClaimError("성공", "인게임 서버 접속 성공", "확인", null);
            }
            else
            {
                UIManager.ClaimError("오류", args.ErrInfo.Detail.ToString(), "확인", null);
            }
        };

        // 내가 입장할 때 한 번
        Backend.Match.OnSessionListInServer = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
                currentState = NetworkState.OnGameRoom;
                // 유저들을 저장할 공간을 마련
                inGameUserInfoDictionary = new();
                foreach(MatchUserGameRecord gameRecord in args.GameRecords)
                {
                    // 유저 인포 딕셔너리에 새로운 유저를 추가하는 함수를 만들어서 여기서 돌리기
                    // 추가하는 함수에는 hostGameRecord와 myGameRecord가 분류 되어야 함.
                    AddUserInfoToDictionary(gameRecord);
                }
                SceneManager.LoadScene("InGameScene");
            }
            else
            {
                UIManager.ClaimError("오류", args.ErrInfo.ToString(), "확인", null);

            }
        };

        // 누군가 방에 입장할 때 마다
        Backend.Match.OnMatchInGameAccess = (args) =>
        {
            if (args.ErrInfo == ErrorCode.Success)
            {
                AddUserInfoToDictionary(args.GameRecord);
            }
            else
            {
                UIManager.ClaimError("오류", args.ErrInfo.ToString(), "확인", null);

            }
        };

        RegistrationCustomCallbacks();
    }

    static void MatchPoll(float deltaTime)
    {
        Backend.Match.Poll();
    }
}
