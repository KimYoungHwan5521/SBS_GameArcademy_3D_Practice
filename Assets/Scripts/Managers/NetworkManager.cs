using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class NetworkManager : Manager
{
    public enum NetworkState
    {                                                   // ��Ƽĳ���͸� ĳ���ͱ��� ��� �����ؾ� ������ ����
        Offline, Initiating, HandShake, Connected, SignIn, WorldJoin, Disconnected
    }                                            //������ �� �����ͺ��̽� "�� �� ��"�� Ȯ�� ����

    NetworkState currentState = NetworkState.Offline;
    public NetworkState CurrentState => currentState;


    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");
        yield return null;

        BackendReturnObject initializer = Backend.Initialize(true);

        // �� �Լ� ���������� ��ٷ�
        // yield return new WaitForFunction(() => { initializer = Backend.Initialize(true); });
        if(initializer.IsSuccess())
        {
            currentState++;
        }
        else
        {
            currentState = NetworkState.Disconnected;
            Debug.LogError($"Connection Failed : {initializer}");
        }
        yield return null;

    }

    public static IEnumerator LogIn(string inputID, string inputPassword)
    {
        BackendReturnObject loginObject = null;
        yield return new WaitForFunction(() => { loginObject = Backend.BMember.CustomLogin(inputID, inputPassword); });
        if(loginObject.IsSuccess())
        {

            Debug.Log($"Log in Successed : {loginObject}");
            GameManager.Instance.loginCanvas.CloseLogInWindow();
        }
        else
        {
            Debug.Log($"Log in Failed : {loginObject}");
        }
        yield return null;
    }
}
