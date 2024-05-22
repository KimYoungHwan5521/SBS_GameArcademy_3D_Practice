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
    {                                                   // ��Ƽĳ���͸� ĳ���ͱ��� ��� �����ؾ� ������ ����
        Offline, Initiating, Connected, SignIn, WorldJoin, Disconnected
    }                                            //������ �� �����ͺ��̽� "�� �� ��"�� Ȯ�� ����

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

        // �� �Լ� ���������� ��ٷ�
        // yield return new WaitForFunction(() => { initializer = Backend.Initialize(true); });
        if(initializer.IsSuccess())
        {
            currentState = NetworkState.Connected;
            // ���� �ܰ迡�� ��Ʈ��ũ�� �����
            // �α���â�� �� �غ� : ������ ���������� ���۵� ��
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
