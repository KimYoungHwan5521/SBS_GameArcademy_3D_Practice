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
