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
    {                                                   // 멀티캐릭터면 캐릭터까지 모두 접속해야 접속한 판정
        Offline, Initiating, Connected, SignIn, WorldJoin, Disconnected
    }                                            //접속할 때 데이터베이스 "단 한 번"은 확인 가능

    NetworkState currentState = NetworkState.Offline;
    public NetworkState CurrentState => currentState;

    public class MatchCard
    {
        public MatchType matchType;
        public MatchModeType modeType;
        public string inDate;
    }

    public MatchCard[] matchCardArray;

    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");
        yield return null;

        BackendReturnObject initializer = Backend.Initialize(true);

        // 이 함수 끝날때까지 기다려
        // yield return new WaitForFunction(() => { initializer = Backend.Initialize(true); });
        if(initializer.IsSuccess())
        {
            currentState = NetworkState.Connected;
            // 여기 단계에서 네트워크에 연결됨
            // 로그인창을 열 준비 : 게임이 본격적으로 시작될 때
            // GameManager.ManagerStarts += () => { UIManager.Open(UIManager.UIType.LogIn); };
        }
        else
        {
            currentState = NetworkState.Disconnected;
            Debug.LogError($"Connection Failed : {initializer}");
        }
        yield return null;

    }

}
