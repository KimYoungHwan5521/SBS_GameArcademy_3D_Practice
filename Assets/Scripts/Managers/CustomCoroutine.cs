using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//          �Լ��� ���� ������ ���
public class WaitForFunction : CustomYieldInstruction
{
    bool isWating;
    public override bool keepWaiting => isWating;

    public WaitForFunction(System.Action wantFunction)
    {
        isWating = true;

        //wantFunction?.Invoke();

        Run(wantFunction);
    }

    // �񵿱��Լ�
    async void Run(System.Action wantFunction)
    {
        // �ٸ� ������ -> �۾����� (�������� �ڵ尡 ���ÿ� ���ư� �� ����)
        // ���� ���� �ٻڴϱ� ��������� ���� ����ġ�� ��ٸ�
        await Task.Run(wantFunction);
        isWating= false; // ���� �ȱ�ٷ�����
    }
}
