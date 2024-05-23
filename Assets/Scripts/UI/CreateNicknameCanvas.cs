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
            UIManager.ClaimError("����", "�г����� 2���� �̻� 8���� ���Ͽ��� �մϴ�.", "Ȯ��", null);
        }
        else
        {
            NetworkManager.ClaimSetNickname(inputNickname.text);
        }

    }
}
