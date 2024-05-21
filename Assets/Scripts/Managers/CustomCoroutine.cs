using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//          함수가 끝날 때까지 대기
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

    // 비동기함수
    async void Run(System.Action wantFunction)
    {
        // 다른 쓰레드 -> 작업라인 (여러개의 코드가 동시에 돌아갈 수 있음)
        // 내가 일이 바쁘니까 옆사람에게 일을 떠밀치고 기다림
        await Task.Run(wantFunction);
        isWating= false; // 이제 안기다려도됨
    }
}
