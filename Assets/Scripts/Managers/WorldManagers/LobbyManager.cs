using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : WorldManager
{
    [SerializeField] GameObject matching;
    [SerializeField] TextMeshProUGUI matchingTimeText;
    [SerializeField] TextMeshProUGUI gameStartText;

    float matchingTime;
    bool isMatching;

    protected override IEnumerator Initiate()
    {
        NetworkManager networkManager = GameManager.Instance.NetworkManager;
        if (networkManager.MyNickname == null)
        UIManager.Open(UIManager.UIType.CreateNickname);
        yield return new WaitWhile(() => networkManager.MyNickname == null);

        yield return NetworkManager.MatchMakingServer();
        yield return base.Initiate();
    }

    private void Update()
    {
        if (!isMatching) return;
        matchingTime += Time.deltaTime;
        matchingTimeText.text = $"{(int)matchingTime/60:00} : {(int)matchingTime%60:00}";
    }

    public void OnClickGameStart()
    {
        if (isMatching)
        {
            isMatching= false;
            gameStartText.text = "게임시작";
            matching.SetActive(false);
        }
        else
        {
            isMatching = true;
            matchingTime = 0;
            gameStartText.text = "매칭취소";
            matching.SetActive(true);
        }
    }
}
