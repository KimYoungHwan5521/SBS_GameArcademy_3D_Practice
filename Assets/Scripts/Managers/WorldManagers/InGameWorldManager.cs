using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class InGameWorldManager : WorldManager
{
    protected override IEnumerator Initiate()
    {
        yield return base.Initiate();
        // ������ �⺻��Ҹ� �ε��ߴµ� �ε�â�� ������ �ʴ� ���
        // : ��� �÷��̾ �ε尡 ������ �ʾ��� ��
        Backend.Match.SendDataToInGameRoom("asbsdfsdjfiselfsjll".ToByteArray_UTF8());
    }
}
