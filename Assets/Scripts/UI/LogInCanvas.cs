using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInCanvas : MonoBehaviour
{
    [SerializeField] GameObject LogInWindow;
    [SerializeField] TMP_InputField signInInputID;
    [SerializeField] TMP_InputField signInInputPassword;

    [SerializeField] TMP_InputField signUpInputID;
    [SerializeField] TMP_InputField signUpInputPassword;
    [SerializeField] TMP_InputField signUpInputPasswordConfirm;
    [SerializeField] GameObject signUpWindow;

    public void SignIn()
    {
        NetworkManager.ClaimSignIn(signInInputID.text, signInInputPassword.text);
    }

    public void SignUp()
    {
        if(string.Compare(signUpInputPassword.text, signUpInputPasswordConfirm.text) != 0)
        {
            Debug.Log("Password and password confirm are not the same");
        }
        else
        {
            NetworkManager.ClaimSignUp(signUpInputID.text, signUpInputPassword.text);
        }
    }

    public void OpenSignUpWindow()
    {
        signUpWindow.SetActive(true);
    }

    public void CloseSignUpWindow()
    {
        signUpWindow.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying= false;
#else
        Application.Quit();
#endif
    }
}
