using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : WorldManager
{
    protected override void Start()
    {
        // �α����� ���� �ִ� ������
        // �α��� ��û�� ���� �ұ��? : �ε��� ��� ���� �� && ��Ʈ��ũ�Ŵ����� ���������� ����Ǿ��� ��
        GameManager.ManagerStarts += () => {
            GameManager.WorldChange(this);
            if(GameManager.Instance.NetworkManager != null && GameManager.Instance.NetworkManager.CurrentState == NetworkManager.NetworkState.Connected)
            {
                UIManager.Open(UIManager.UIType.LogIn);
            }
        };
    }
}
