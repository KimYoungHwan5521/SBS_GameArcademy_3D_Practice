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
    // ���� ��ġ����ŷ�� ��� �Ǿ����� ������ ����ٸ�
    // �鿣�尡 �������� "�̷����� �Ͼ��"�� ���� �˷��ش� : "�ݹ�" �Լ��� ���
    void RegistCallBackFunction()
    {
        // ��ġ����ŷ ������ �����Ϸ��� �õ��� ���� ����
        Backend.Match.OnJoinMatchMakingServer = (args) =>
        {
            if(args.ErrInfo.Category != BackEnd.Tcp.ErrorCode.Success)
            {
                UIManager.ClaimError("��ġ ����ŷ ����", args.ErrInfo.Detail.ToString(), "Ȯ��", null);
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
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
                ClaimJoinRoom(args.MatchCardIndate, args.RoomInfo);
            }
            else if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_MatchMakingCanceled)
            {
                UIManager.ClaimError("���", "��Ī�� ��ҵǾ����ϴ�.", "Ȯ��", null);

            }
            else if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_InProgress)
            {
                GameManager.Instance.NetworkManager.currentState = NetworkState.Matching;
                UIManager.ClaimError("��Ī ����", "��Ī�� ���۵Ǿ����ϴ�.", "Ȯ��", null);

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

        //////////// �ΰ��� ///////////
        Backend.Match.OnSessionJoinInServer = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorInfo.Success)
            {
                currentState = NetworkState.OnGameServer;
                UIManager.ClaimError("����", "�ΰ��� ���� ���� ����", "Ȯ��", null);
            }
            else
            {
                UIManager.ClaimError("����", args.ErrInfo.Detail.ToString(), "Ȯ��", null);
            }
        };

        // ���� ������ �� �� ��
        Backend.Match.OnSessionListInServer = (args) =>
        {
            if(args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
                currentState = NetworkState.OnGameRoom;
                // �������� ������ ������ ����
                inGameUserInfoDictionary = new();
                foreach(MatchUserGameRecord gameRecord in args.GameRecords)
                {
                    // ���� ���� ��ųʸ��� ���ο� ������ �߰��ϴ� �Լ��� ���� ���⼭ ������
                    // �߰��ϴ� �Լ����� hostGameRecord�� myGameRecord�� �з� �Ǿ�� ��.
                    AddUserInfoToDictionary(gameRecord);
                }
                SceneManager.LoadScene("InGameScene");
            }
            else
            {
                UIManager.ClaimError("����", args.ErrInfo.ToString(), "Ȯ��", null);

            }
        };

        // ������ �濡 ������ �� ����
        Backend.Match.OnMatchInGameAccess = (args) =>
        {
            if (args.ErrInfo == ErrorCode.Success)
            {
                AddUserInfoToDictionary(args.GameRecord);
            }
            else
            {
                UIManager.ClaimError("����", args.ErrInfo.ToString(), "Ȯ��", null);

            }
        };

        RegistrationCustomCallbacks();
    }

    static void MatchPoll(float deltaTime)
    {
        Backend.Match.Poll();
    }
}
