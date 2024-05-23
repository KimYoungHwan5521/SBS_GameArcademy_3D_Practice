using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

// NetworkClaim
public partial class NetworkManager : Manager
{

    public static void ClaimSignIn(string inputID, string inputPassword)
    {
        GameManager.Instance.StartCoroutine(SignInStart(inputID, inputPassword));
    }

    public static IEnumerator SignInStart(string inputID, string inputPassword)
    {
        GameManager.ClaimLoadInfo("sign in");
        string errorMessage = "";
        BackendReturnObject loginObject = null;

        yield return new WaitForFunction(()=> { loginObject = Backend.BMember.CustomLogin(inputID, inputPassword); });
        if (loginObject.IsSuccess())
        {
            Debug.Log($"Log in Successed : {loginObject}");
            BackendReturnObject userInfoResult = null;
            yield return new WaitForFunction(() => { userInfoResult = Backend.BMember.GetUserInfo(); });
            if(userInfoResult.IsSuccess())
            {
                LitJson.JsonData userJson = userInfoResult.GetReturnValuetoJSON()["row"];
                GameManager.CloseLoadInfo();
                UserInfo newInfo = new();
                newInfo.gamerId = userJson["gamerId"].ToString();
                newInfo.countryCode = userJson["countryCode"]?.ToString();
                newInfo.nickname = userJson["nickname"]?.ToString();
                GameManager.Instance.NetworkManager.myInfo = newInfo;
                GameManager.Instance.NetworkManager.currentState = NetworkState.SignIn;
                
                GameManager.ManagerStarts += () => SceneManager.LoadScene("LobbyScene");

            }
            else
            {
                errorMessage = $"Failed to get user info : {userInfoResult.GetMessage()}";
                GameManager.ManagerStarts += () => UIManager.ClaimError("오류", errorMessage, "확인", null);

            }

        }
        else
        {
            Debug.Log($"Log in Failed : {loginObject.GetMessage()}");
            errorMessage = $"Log in Failed : {loginObject.GetMessage()}"; 
            GameManager.ManagerStarts += () => UIManager.ClaimError("오류", errorMessage, "확인", null);
        }
        GameManager.CloseLoadInfo();


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
            GameManager.CloseLoadInfo();
            yield break;
        }

        // response는 다양한 요청의 결과물이지만, 안에 들어있는것은 항상 다름
        List<MatchCard> newCardList = new();
        // 받은 정보를 JSON으로 저장
        LitJson.JsonData responseJSON = response.FlattenRows();

        foreach(LitJson.JsonData currentRow in responseJSON)
        {
            MatchCard currentCard = new();
            currentCard.inDate = currentRow["inDate"].ToString();
            System.Enum.TryParse(currentRow["matchType"].ToString().FirstCharacterToUpper(), out currentCard.matchType);
            System.Enum.TryParse(currentRow["matchModeType"].ToString(), out currentCard.modeType);
            newCardList.Add(currentCard);
        }
        GameManager.Instance.NetworkManager.matchCardArray = newCardList.ToArray();

        // 매치서버에 들어옴
        GameManager.Instance.NetworkManager.currentState = NetworkState.OnMatchingServer;

        // 이제부터 "매치 서버의 메시지"를 받도록 하겠다.
        // 메시지는 "언제" 받나 : 계속
        // poll : 지금까지 들어온 메시지가 있는지 확인
        GameManager.ManagerUpdates -= MatchPoll;
        GameManager.ManagerUpdates += MatchPoll;


        GameManager.CloseLoadInfo();    
    }

    public static void ClaimMatch(int index = 0)
    {
        GameManager.Instance.StartCoroutine(MatchStart(index));
    }

    public static IEnumerator MatchStart(int index)
    {
        if(GameManager.Instance?.NetworkManager?.matchCardArray == null || GameManager.Instance.NetworkManager.matchCardArray.Length == 0 || GameManager.Instance.NetworkManager.matchCardArray.Length <= index)
        {
            UIManager.ClaimError("오류", "매치메이킹을 받는데 실패했습니다.", "확인", () => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
            yield break;
        }

        // 매치룸에 들어가 있는지 확인 해야한다.
        if(GameManager.Instance.NetworkManager.currentState < NetworkState.OnMatchRoom)
        {
            // 안들어가있으면 매치룸 만들기
            yield return new WaitForFunction(()=> Backend.Match.CreateMatchRoom());

            // 매칭룸에 들어갈 때까지 대기
            yield return new WaitWhile(() => GameManager.Instance.NetworkManager.currentState < NetworkState.OnMatchRoom);
        }
        MatchCard wantCard = GameManager.Instance.NetworkManager.matchCardArray[index];

        Backend.Match.RequestMatchMaking(wantCard.matchType, wantCard.modeType, wantCard.inDate);

    }

    public static void ClaimSetNickname(string wantNickname)
    {
        GameManager.Instance.StartCoroutine(SetNicknameStart(wantNickname)); 
        
    }

    public static IEnumerator SetNicknameStart(string wantNickname)
    {
        BackendReturnObject result = null;
        yield return new WaitForFunction(() => { 
            result = Backend.BMember.UpdateNickname(wantNickname);
        });
        if(result.IsSuccess())
        {
            UIManager.Close(UIManager.UIType.CreateNickname);
            UIManager.ClaimError("성공", "닉네임 변경에 성공했습니다.", "확인", null);
        }
        else
        {
            UIManager.ClaimError("오류", result.GetMessage(), "확인", null);
        }

    }
}
