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
                GameManager.ManagerStarts += () => UIManager.ClaimError("����", errorMessage, "Ȯ��", null);

            }

        }
        else
        {
            Debug.Log($"Log in Failed : {loginObject.GetMessage()}");
            errorMessage = $"Log in Failed : {loginObject.GetMessage()}"; 
            GameManager.ManagerStarts += () => UIManager.ClaimError("����", errorMessage, "Ȯ��", null);
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
                GameManager.ManagerStarts += () => UIManager.ClaimError("ȸ�� ���� ����!", "ȸ�� ���Կ� �����߽��ϴ�.", "Ȯ��", null);
            }
            else
            {
                Debug.Log($"Sign up failed : {signUpObject.GetMessage()}");
                errorMessage = $"Sign up failed : {signUpObject.GetMessage()}";
                GameManager.ManagerStarts += () => UIManager.ClaimError("����", errorMessage, "Ȯ��", null);
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
        if (!result)  UIManager.ClaimError("����", errorInfo.ToString(), "Ȯ��", ()=> UnityEngine.SceneManagement.SceneManager.LoadScene(0));

        BackendReturnObject response = null;
        yield return new WaitForFunction(() => { response = Backend.Match.GetMatchList(); });
        if(!response.IsSuccess())
        {
            UIManager.ClaimError("����", response.GetMessage(), "Ȯ��", () => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
            GameManager.CloseLoadInfo();
            yield break;
        }

        // response�� �پ��� ��û�� �����������, �ȿ� ����ִ°��� �׻� �ٸ�
        List<MatchCard> newCardList = new();
        // ���� ������ JSON���� ����
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

        // ��ġ������ ����
        GameManager.Instance.NetworkManager.currentState = NetworkState.OnMatchingServer;

        // �������� "��ġ ������ �޽���"�� �޵��� �ϰڴ�.
        // �޽����� "����" �޳� : ���
        // poll : ���ݱ��� ���� �޽����� �ִ��� Ȯ��
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
            UIManager.ClaimError("����", "��ġ����ŷ�� �޴µ� �����߽��ϴ�.", "Ȯ��", () => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
            yield break;
        }

        // ��ġ�뿡 �� �ִ��� Ȯ�� �ؾ��Ѵ�.
        if(GameManager.Instance.NetworkManager.currentState < NetworkState.OnMatchRoom)
        {
            // �ȵ������� ��ġ�� �����
            yield return new WaitForFunction(()=> Backend.Match.CreateMatchRoom());

            // ��Ī�뿡 �� ������ ���
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
            UIManager.ClaimError("����", "�г��� ���濡 �����߽��ϴ�.", "Ȯ��", null);
        }
        else
        {
            UIManager.ClaimError("����", result.GetMessage(), "Ȯ��", null);
        }

    }
}
