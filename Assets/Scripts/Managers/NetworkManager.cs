using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class NetworkManager : Manager
{
    public enum NetworkState
    {                                                   // 멀티캐릭터면 캐릭터까지 모두 접속해야 접속한 판정
        Offline, Initiating, HandShake, Connected, SignIn, WorldJoin, Disconnected
    }                                            //접속할 때 데이터베이스 "단 한 번"은 확인 가능

    NetworkState currentState = NetworkState.Offline;
    public NetworkState CurrentState => currentState;


    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");
        yield return null;

        BackendReturnObject initializer = Backend.Initialize(true);

        // 이 함수 끝날때까지 기다려
        // yield return new WaitForFunction(() => { initializer = Backend.Initialize(true); });
        if(initializer.IsSuccess())
        {
            currentState++;
            // 여기 단계에서 네트워크에 연결됨
            // 로그인창을 열 준비 : 게임이 본격적으로 시작될 때
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
                // WaitForFuction은 멀티쓰레드를 사용하고 있다.
                // SetActive()는 메인쓰레드가 아닌 곳에서 호출되면 UnityException에러가 발생하며 호출이 무시된다.
                GameManager.ManagerStarts += () => UIManager.ClaimError("로그인 성공!", "로그인 성공!", "확인", null);
            }
            else
            {
                Debug.Log($"Log in Failed : {loginObject.GetMessage()}");
                GameManager.ManagerStarts += () => UIManager.ClaimError("오류", errorMessage, "확인", null);
                errorMessage = $"Log in Failed : {loginObject.GetMessage()}";
            }
        }));
        /*
        if(loginSuccess)
        {
            UIManager.ClaimError("로그인 성공!", "로그인 성공!", "확인", null);
            UIManager.Close(UIManager.UIType.LogIn);

        }
        else
        {
            UIManager.ClaimError("오류", errorMessage, "확인", null);

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
                GameManager.ManagerStarts += () => UIManager.ClaimError("회원 가입 성공!", "회원 가입에 성공했습니다.", "확인", null);
            }
            else
            {
                Debug.Log($"Sign up failed : {signUpObject.GetMessage()}");
                errorMessage = $"Sign up failed : {signUpObject.GetMessage()}";
                GameManager.ManagerStarts += () => UIManager.ClaimError("오류", errorMessage, "확인", null);
            }
        }));
        /*
        if (signUpSuccess)
        {
            UIManager.ClaimError("회원 가입 성공!", "회원 가입에 성공했습니다.", "확인", null);

        }
        else
        {
            UIManager.ClaimError("오류", errorMessage, "확인", null);

        }
         */
    }
}
