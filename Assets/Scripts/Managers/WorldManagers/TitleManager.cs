using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : WorldManager
{
    protected override void Start()
    {
        // 로그인을 위해 있는 페이지
        // 로그인 요청을 언제 할까요? : 로딩이 모두 끝난 뒤 && 네트워크매니저가 정상적으로 실행되었을 때
        GameManager.ManagerStarts += () => {
            GameManager.WorldChange(this);
            if(GameManager.Instance.NetworkManager != null && GameManager.Instance.NetworkManager.CurrentState == NetworkManager.NetworkState.Connected)
            {
                UIManager.Open(UIManager.UIType.LogIn);
            }
        };
    }
}
