using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInCanvas : MonoBehaviour
{
    [SerializeField] GameObject LogInWindow;
    [SerializeField] TMP_InputField inputID;
    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] Button buttonLogIn;

    public void LogIn()
    {
        Debug.Log($"{inputID.text}, {inputPassword.text}");
        StartCoroutine(NetworkManager.LogIn(inputID.text, inputPassword.text));
    }

    public void OpenLogInWindow()
    {
        LogInWindow.SetActive(true);
    }

    public void CloseLogInWindow()
    {
        LogInWindow.SetActive(false);
    }
}
