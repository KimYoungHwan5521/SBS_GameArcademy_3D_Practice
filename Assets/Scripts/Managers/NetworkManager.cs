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
            // ���� �ܰ迡�� ��Ʈ��ũ�� �����
            // �α���â�� �� �غ� : ������ ���������� ���۵� ��
            GameManager.ManagerStarts += () => { UIManager.Open(UIManager.UIType.LogIn); };
        }
        else
        {
            currentState = NetworkState.Disconnected;
            Debug.LogError($"Connection Failed : {initializer}");
        }
        yield return null;

    }

    public static void ClaimSignIn(string inputID, string inputPassword)
    {
        //bool loginSuccess = false;
        string errorMessage = "";
        GameManager.Instance.StartCoroutine(new WaitForFunction(() => {
            BackendReturnObject loginObject = null;
            loginObject = Backend.BMember.CustomLogin(inputID, inputPassword);
            if(loginObject.IsSuccess())
            {

                Debug.Log($"Log in Successed : {loginObject}");
                //loginSuccess = true;
                // WaitForFuction�� ��Ƽ�����带 ����ϰ� �ִ�.
                // SetActive()�� ���ξ����尡 �ƴ� ������ ȣ��Ǹ� UnityException������ �߻��ϸ� ȣ���� ���õȴ�.
                GameManager.ManagerStarts += () => UIManager.ClaimError("�α��� ����!", "�α��� ����!", "Ȯ��", null);
            }
            else
            {
                Debug.Log($"Log in Failed : {loginObject.GetMessage()}");
                GameManager.ManagerStarts += () => UIManager.ClaimError("����", errorMessage, "Ȯ��", null);
                errorMessage = $"Log in Failed : {loginObject.GetMessage()}";
            }
        }));
        /*
        if(loginSuccess)
        {
            UIManager.ClaimError("�α��� ����!", "�α��� ����!", "Ȯ��", null);
            UIManager.Close(UIManager.UIType.LogIn);

        }
        else
        {
            UIManager.ClaimError("����", errorMessage, "Ȯ��", null);

        }
         */
    }

    public static void ClaimSignUp(string inputID, string inputPassword)
    {
        // bool signUpSuccess = false;
        string errorMessage = "";
        GameManager.Instance.StartCoroutine(new WaitForFunction(() =>
        {
            BackendReturnObject signUpObject = null;
            signUpObject = Backend.BMember.CustomSignUp(inputID, inputPassword);
            if(signUpObject.IsSuccess())
            {
                //signUpSuccess = true;
                GameManager.ManagerStarts += () => UIManager.ClaimError("ȸ�� ���� ����!", "ȸ�� ���Կ� �����߽��ϴ�.", "Ȯ��", null);
            }
            else
            {
                Debug.Log($"Sign up failed : {signUpObject.GetMessage()}");
                errorMessage = $"Sign up failed : {signUpObject.GetMessage()}";
                GameManager.ManagerStarts += () => UIManager.ClaimError("����", errorMessage, "Ȯ��", null);
            }
        }));
        /*
        if (signUpSuccess)
        {
            UIManager.ClaimError("ȸ�� ���� ����!", "ȸ�� ���Կ� �����߽��ϴ�.", "Ȯ��", null);

        }
        else
        {
            UIManager.ClaimError("����", errorMessage, "Ȯ��", null);

        }
         */
    }
}
