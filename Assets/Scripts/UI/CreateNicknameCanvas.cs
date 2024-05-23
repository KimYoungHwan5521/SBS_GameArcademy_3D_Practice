using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateNicknameCanvas : MonoBehaviour
{
    [SerializeField] TMP_InputField inputNickname;

    public void SetNickname()
    {
        if (inputNickname.text.Length < 2)
        {
            UIManager.ClaimError("오류", "닉네임은 2글자 이상 8글자 이하여야 합니다.", "확인", null);
        }
        else
        {
            NetworkManager.ClaimSetNickname(inputNickname.text);
        }

    }
}
