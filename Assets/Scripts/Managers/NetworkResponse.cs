using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.SceneManagement;

// NetworkResponse
public partial class NetworkManager : Manager
{
    // ���� ��ġ����ŷ�� ��� �Ǿ����� ������ ����ٸ�
    // �鿣�尡 �������� "�̷����� �Ͼ��"�� ���� �˷��ش� : "�ݹ�" �Լ��� ���
    void RegistCallBackFunction()
    {
        // ��ġ����ŷ ������ �����Ϸ��� �õ��� ���� ����
        Backend.Match.OnJoinMatchMakingServer = (args) =>
        {
            if(args.ErrInfo.Category != BackEnd.Tcp.ErrorCode.Success)
            {
                UIManager.ClaimError("��ġ ����ŷ ����", args.ErrInfo.Detail.ToString(), "Ȯ��", () => { UIManager.Close(UIManager.UIType.ErrorWindow); });
            }
        };

        // ��ġ����ŷ �뿡 �� ����
        Backend.Match.OnMatchMakingRoomJoin = (args) =>
        {
            // ���� ��ġ����ŷ ������ ���� �ִ� ���¶�� -> ��ġ����ŷ ������
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
                Debug.Log("��Ī ���� ���������� �����Ǿ����ϴ�.");
            }
            else
            {
                UIManager.ClaimError("��ġ ����ŷ ����", args.ErrInfo.ToString(), "Ȯ��", null);
            }
        };

        // ��ġ����ŷ ��û�� ���� ����
        Backend.Match.OnMatchMakingResponse = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_InProgress)
            {
                GameManager.Instance.NetworkManager.currentState = NetworkState.Matching;

            }
            else
            {
                UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason, "Ȯ��", null);

            }
        };

        Backend.Match.OnMatchResult = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
            }
            else
            {
                UIManager.ClaimError("��Ī ����", args.ErrInfo.ToString(), "Ȯ��", null);

            }
        };
    }

    static void MatchPoll(float deltaTime)
    {
        Backend.Match.Poll();
    }
}
