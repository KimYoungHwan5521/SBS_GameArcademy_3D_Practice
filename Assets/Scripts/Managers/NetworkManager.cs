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

    public class PlayerInfo
    {
        public MatchUserGameRecord record;
        public bool isLoaded;
        public GameObject testObject;

        public PlayerInfo(MatchUserGameRecord record, bool isLoaded = false)
        {
            this.record = record;
            this.isLoaded = isLoaded;
        }
    }
    
    Dictionary<SessionId, PlayerInfo> inGameUserInfoDictionary;
    public static PlayerInfo myGameRecord;
    public static PlayerInfo hostGameRecord;

    public static PlayerInfo GetUser(SessionId wantSessionId)
    {
        var targetDictionary = GameManager.Instance.NetworkManager.inGameUserInfoDictionary;
        if (targetDictionary == null) return null;

        if(targetDictionary.TryGetValue(wantSessionId, out PlayerInfo result)) return result;
        else
        {
            UIManager.ClaimError("����", "���� ������ �ҷ����� ���߽��ϴ�.", "Ȯ��", null);
            return null;
        }
    }

    public static PlayerInfo[] GetAllUser()
    {
        var targetDictionary = GameManager.Instance.NetworkManager.inGameUserInfoDictionary;
        List<PlayerInfo> result = new();
        foreach(var info in targetDictionary)
        {
            result.Add(info.Value);
        }
        return result.ToArray();
    }

    void AddUserInfoToDictionary(MatchUserGameRecord wantUserInfo)
    {
        PlayerInfo info = new(wantUserInfo);
        if(wantUserInfo == null) return;
        var targetDictionary = GameManager.Instance.NetworkManager.inGameUserInfoDictionary;
        if (targetDictionary.TryAdd(wantUserInfo.m_sessionId, info))
        {
            Debug.Log($"{wantUserInfo.m_nickname + (wantUserInfo.m_isSuperGamer? "(Host)" : "")} is in this room!");

        }
        else
        {
            targetDictionary[wantUserInfo.m_sessionId] = info;
        }
        if (wantUserInfo.m_isSuperGamer) hostGameRecord = info;
        if (wantUserInfo.m_nickname == GameManager.Instance.NetworkManager.MyNickname) myGameRecord = info;
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
