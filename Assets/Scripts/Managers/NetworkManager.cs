using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BackEnd;
using BackEnd.Tcp;

// �ļ��� ���� ������ �����Ҷ��� �浹�� ���ϱ� ����.
// NetworkManager
public partial class NetworkManager : Manager
{
    public enum NetworkState
    {
        // ��Ƽĳ���͸� ĳ���ͱ��� ��� �����ؾ� ������ ����
        //������ �� �����ͺ��̽� "�� �� ��"�� Ȯ�� ����
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
            UIManager.ClaimError("����", "���� ������ �ҷ����� ���߽��ϴ�.", "Ȯ��", null);
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

        // �� �Լ� ���������� ��ٷ�
        // yield return new WaitForFunction(() => { initializer = Backend.Initialize(true); });
        if(initializer.IsSuccess())
        {
            currentState = NetworkState.Connected;
            // ���� �ܰ迡�� ��Ʈ��ũ�� �����
            // �α���â�� �� �غ� : ������ ���������� ���۵� ��
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
