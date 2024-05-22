using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.SceneManagement;

// NetworkClaim
public partial class NetworkManager : Manager
{

    public static void ClaimSignIn(string inputID, string inputPassword)
    {
        string errorMessage = "";
        GameManager.Instance.StartCoroutine(new WaitForFunction(() => {
            BackendReturnObject loginObject = null;
            loginObject = Backend.BMember.CustomLogin(inputID, inputPassword);
            if (loginObject.IsSuccess())
            {

                Debug.Log($"Log in Successed : {loginObject}");
                // WaitForFuction은 멀티쓰레드를 사용하고 있다.
                // SetActive() 등은 메인쓰레드가 아닌 곳에서 호출되면 UnityException에러가 발생하며 호출이 무시된다.
                // GameManager.ManagerStarts += () => UIManager.ClaimError("로그인 성공!", "로그인 성공!", "확인", null);
                GameManager.ManagerStarts += () => SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                Debug.Log($"Log in Failed : {loginObject.GetMessage()}");
                GameManager.ManagerStarts += () => UIManager.ClaimError("오류", errorMessage, "확인", null);
                errorMessage = $"Log in Failed : {loginObject.GetMessage()}";
            }
        }));
    }

    public static void ClaimSignUp(string inputID, string inputPassword)
    {
        string errorMessage = "";
        GameManager.Instance.StartCoroutine(new WaitForFunction(() =>
        {
            BackendReturnObject signUpObject = null;
            signUpObject = Backend.BMember.CustomSignUp(inputID, inputPassword);
            if (signUpObject.IsSuccess())
            {
                GameManager.ManagerStarts += () => UIManager.ClaimError("회원 가입 성공!", "회원 가입에 성공했습니다.", "확인", null);
            }
            else
            {
                Debug.Log($"Sign up failed : {signUpObject.GetMessage()}");
                errorMessage = $"Sign up failed : {signUpObject.GetMessage()}";
                GameManager.ManagerStarts += () => UIManager.ClaimError("오류", errorMessage, "확인", null);
            }
        }));
    }

    public static void ClaimMatchMackingServer()
    {
        GameManager.Instance.StartCoroutine(MatchMakingServer());
    }

    public static IEnumerator MatchMakingServer()
    {
        GameManager.ClaimLoadInfo("join to MatchMakingServer");

        bool result = false;
        BackEnd.Tcp.ErrorInfo errorInfo = null;
        
        yield return new WaitForFunction(() => {
            result = Backend.Match.JoinMatchMakingServer(out errorInfo);
        });
        if (!result)  UIManager.ClaimError("오류", errorInfo.ToString(), "확인", ()=> UnityEngine.SceneManagement.SceneManager.LoadScene(0));

        BackendReturnObject response = null;
        yield return new WaitForFunction(() => { response = Backend.Match.GetMatchList(); });
        if(!response.IsSuccess())
        {
            UIManager.ClaimError("오류", response.GetMessage(), "확인", () => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
        }

        // response는 다양한 요청의 결과물이지만, 안에 들어있는것은 항상 다름
        List<MatchCard> newCardList = new();
        // 받은 정보를 JSON으로 저장
        LitJson.JsonData responseJSON = response.FlattenRows();

        foreach(LitJson.JsonData currentRow in responseJSON)
        {
            MatchCard currentCard = new();
            currentCard.inDate = currentRow["inDate"].ToString();
            System.Enum.TryParse(currentRow["matchType"].ToString(), out currentCard.matchType);
            System.Enum.TryParse(currentRow["matchModeType"].ToString(), out currentCard.modeType);
            newCardList.Add(currentCard);
        }
        GameManager.Instance.NetworkManager.matchCardArray = newCardList.ToArray();

        // 이제부터 "매치 서버의 메시지"를 받도록 하겠다.
        // 메시지는 "언제" 받나 : 계속
        // poll : 지금까지 들어온 메시지가 있는지 확인
        GameManager.ManagerUpdates -= MatchPoll;
        GameManager.ManagerUpdates += MatchPoll;


        GameManager.CloseLoadInfo();    
    }

    public static void ClaimMatch(int index = 0)
    {
        if(GameManager.Instance?.NetworkManager?.matchCardArray == null || GameManager.Instance.NetworkManager.matchCardArray.Length == 0 || GameManager.Instance.NetworkManager.matchCardArray.Length <= index)
        {
            UIManager.ClaimError("오류", "매치메이킹을 받는데 실패했습니다.", "확인", () => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
            return;
        }
        MatchCard wantCard = GameManager.Instance.NetworkManager.matchCardArray[index];

        Backend.Match.RequestMatchMaking(wantCard.matchType, wantCard.modeType, wantCard.inDate);
    }
}
