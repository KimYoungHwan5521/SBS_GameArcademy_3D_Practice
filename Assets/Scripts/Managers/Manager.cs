using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager
{
    // �ڷ�ƾ
    // �ڷ�ƾ ���� �����Ҵ� ���� -> �ҿ�ð��� �ʹ� ���� ��
    // �ҿ�ð��� ���� ����� ���� -> ��� �ڷ�ƾ���� �Դٰ��� �ؾ��ؼ�
    // �ʱ�ȭ �߿��� �������� ���ÿ� �������Ѵ�
    // �ε��� : �÷��̾��� �Է��� �޾ƾ� �Ѵ� (�ȹ����� ������� -> ��������)
    public virtual IEnumerator Initiate()
    {
        yield return null;
    }
    
    public virtual void Start()
    {

    }

    public virtual void ManagerUpdate(float deltaTime)
    {

    }
}
