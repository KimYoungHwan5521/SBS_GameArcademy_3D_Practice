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
        // ��ġ����ŷ ��û�� ���� ����
        Backend.Match.OnMatchMakingResponse = (args) =>
        {
            UIManager.ClaimError(args.ErrInfo.ToString(), args.Reason, "Ȯ��", null);
        };
    }

    static void MatchPoll(float deltaTime)
    {
        Backend.Match.Poll();
    }
}
