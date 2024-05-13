using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    // �ε� ������ ������ �ؽ�Ʈ ������Ʈ
    [SerializeField] protected TextMeshProUGUI loadingText;
    [SerializeField] protected Image loadingBar;
    [SerializeField] protected TextMeshProUGUI loadingProgressText;

    // �̰� ������ �ε� ������ �Է��� �� �ֵ���
    public void SetLoadInfo(string currentInfo)
    {
        loadingText.text = $"Now Loading..<br>{currentInfo}";
        loadingBar.fillAmount = ResourceManager.resourceLoadCompleted / ResourceManager.resourceAmount;
        loadingProgressText.text = $"{ ResourceManager.resourceLoadCompleted / ResourceManager.resourceAmount * 100}%";
    }
}
