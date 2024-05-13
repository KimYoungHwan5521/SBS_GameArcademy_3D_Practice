using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    // 로딩 내용을 보여줄 텍스트 오브젝트
    [SerializeField] protected TextMeshProUGUI loadingText;
    [SerializeField] protected Image loadingBar;
    [SerializeField] protected TextMeshProUGUI loadingProgressText;

    // 이것 가지고 로딩 정보를 입력할 수 있도록
    public void SetLoadInfo(string currentInfo)
    {
        loadingText.text = $"Now Loading..<br>{currentInfo}";
        loadingBar.fillAmount = ResourceManager.resourceLoadCompleted / ResourceManager.resourceAmount;
        loadingProgressText.text = $"{ ResourceManager.resourceLoadCompleted / ResourceManager.resourceAmount * 100}%";
    }
}
