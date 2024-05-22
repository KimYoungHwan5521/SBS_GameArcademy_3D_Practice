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
                // WaitForFuction�� ��Ƽ�����带 ����ϰ� �ִ�.
                // SetActive() ���� ���ξ����尡 �ƴ� ������ ȣ��Ǹ� UnityException������ �߻��ϸ� ȣ���� ���õȴ�.
                // GameManager.ManagerStarts += () => UIManager.ClaimError("�α��� ����!", "�α��� ����!", "Ȯ��", null);
                GameManager.ManagerStarts += () => SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                Debug.Log($"Log in Failed : {loginObject.GetMessage()}");
                GameManager.ManagerStarts += () => UIManager.ClaimError("����", errorMessage, "Ȯ��", null);
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
        }

        // response�� �پ��� ��û�� �����������, �ȿ� ����ִ°��� �׻� �ٸ�
        List<MatchCard> newCardList = new();
        // ���� ������ JSON���� ����
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

        // �������� "��ġ ������ �޽���"�� �޵��� �ϰڴ�.
        // �޽����� "����" �޳� : ���
        // poll : ���ݱ��� ���� �޽����� �ִ��� Ȯ��
        GameManager.ManagerUpdates -= MatchPoll;
        GameManager.ManagerUpdates += MatchPoll;


        GameManager.CloseLoadInfo();    
    }

    public static void ClaimMatch(int index = 0)
    {
        if(GameManager.Instance?.NetworkManager?.matchCardArray == null || GameManager.Instance.NetworkManager.matchCardArray.Length == 0 || GameManager.Instance.NetworkManager.matchCardArray.Length <= index)
        {
            UIManager.ClaimError("����", "��ġ����ŷ�� �޴µ� �����߽��ϴ�.", "Ȯ��", () => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
            return;
        }
        MatchCard wantCard = GameManager.Instance.NetworkManager.matchCardArray[index];

        Backend.Match.RequestMatchMaking(wantCard.matchType, wantCard.modeType, wantCard.inDate);
    }
}
