using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BackEnd;
using BackEnd.Tcp;

// 파셜을 나눠 놓으면 협업할때도 충돌을 피하기 좋다.
// NetworkManager
public partial class NetworkManager : Manager
{
    public enum NetworkState
    {
        // 멀티캐릭터면 캐릭터까지 모두 접속해야 접속한 판정
        //접속할 때 데이터베이스 "단 한 번"은 확인 가능
        Offline, Initiating, Connected, SignIn,
        OnMatchingServer, OnMatchRoom, Matching, 
        JoinGameServer, OnGameServer, OnGameRoom, 
        WorldJoin, Disconnected
    }

    NetworkState currentState = NetworkState.Offline;
    public NetworkState CurrentState => currentState;

    public class UserInfo
    {
        public string gamerId, countryCode, nickname;
    }

    UserInfo myInfo;
    public string MyNickname{ 
        get => myInfo?.nickname;
        set
        {
            if (myInfo != null) return;
            myInfo.nickname = value;
        }
    }
    
    public class MatchCard
    {
        public MatchType matchType;
        public MatchModeType modeType;
        public string inDate;
    }

    public MatchCard[] matchCardArray;

    
    Dictionary<SessionId, MatchUserGameRecord> inGameUserInfoDictionary;
    MatchUserGameRecord myGameRecord;
    MatchUserGameRecord hostGameRecord;

    public static MatchUserGameRecord GetUser(SessionId wantSessionId)
    {
        var targetDictionary = GameManager.Instance.NetworkManager.inGameUserInfoDictionary;
        if (targetDictionary == null) return null;

        if(targetDictionary.TryGetValue(wantSessionId, out MatchUserGameRecord result)) return result;
        else
        {
            UIManager.ClaimError("오류", "유저 정보를 불러오지 못했습니다.", "확인", null);
            return null;
        }
    }

    void AddUserInfoToDictionary(MatchUserGameRecord wantUserInfo)
    {
        if(wantUserInfo == null) return;
        var targetDictionary = GameManager.Instance.NetworkManager.inGameUserInfoDictionary;
        if (targetDictionary.TryAdd(wantUserInfo.m_sessionId, wantUserInfo))
        {
            Debug.Log($"{wantUserInfo.m_nickname + (wantUserInfo.m_isSuperGamer? "(Host)" : "")} is in this room!");

        }
        else
        {
            targetDictionary[wantUserInfo.m_sessionId] = wantUserInfo;
        }
        if (wantUserInfo.m_isSuperGamer) hostGameRecord = wantUserInfo;
        if (wantUserInfo.m_nickname == GameManager.Instance.NetworkManager.MyNickname) myGameRecord = wantUserInfo;
    }

    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");
        yield return null;

        currentState = NetworkState.Initiating;
        BackendReturnObject initializer = Backend.Initialize(true);

        // 이 함수 끝날때까지 기다려
        // yield return new WaitForFunction(() => { initializer = Backend.Initialize(true); });
        if(initializer.IsSuccess())
        {
            currentState = NetworkState.Connected;
            // 여기 단계에서 네트워크에 연결됨
            // 로그인창을 열 준비 : 게임이 본격적으로 시작될 때
            // GameManager.ManagerStarts += () => { UIManager.Open(UIManager.UIType.LogIn); };
            RegistCallBackFunction();
        }
        else
        {
            currentState = NetworkState.Disconnected;
            Debug.LogError($"Connection Failed : {initializer}");
        }
        yield return null;

    }

}
