using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NicknameValidator", menuName = "TextMeshPro/Validates/nickName")]
public class NicknameValidator : TMP_InputValidator
{
    // text : ��ǲ�ʵ� ���ڿ�
    // pos : Ŀ�� ��ġ
    // ch : �Է¹��� ����
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // ���� ��
        if (text.Length > 8) return '\0';

        if(ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch >= 0xAC00 && ch <= 0xD7AF || ch <= 0x3131 && ch >= 3163)
        {
            text = text.Insert(pos, $"{ch}");
            pos++;
            return ch;
        }
        return '\0';
    }

}
