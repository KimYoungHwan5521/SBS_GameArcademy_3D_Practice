using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameWorldManager : WorldManager
{
    protected override IEnumerator Initiate()
    {
        yield return base.Initiate();
        // 월드의 기본요소를 로드했는데 로딩창이 끝나지 않는 경우
        // : 모든 플레이어가 로드가 끝나지 않았을 때
        NetworkManager.SendMessage(NetworkManager.MessageType.LoadComplete, new NetworkManager.LoadComplete_Message() { max = 2, current = 2 });
        GameManager.ClaimLoadInfo("Waiting for other players");
        yield return new WaitUntil(() =>
        {
            foreach(NetworkManager.PlayerInfo currentPlayer in NetworkManager.GetAllUser())
            {
                if (currentPlayer.isLoaded == false) return false;
                else if(currentPlayer.controller == null)
                {
                    // 컴포넌트는 무조건 게임오브젝트 안에 있어야 한다.
                    var inst = new GameObject($"{currentPlayer.record.m_nickname}'s controller", NetworkManager.myGameRecord == currentPlayer ? typeof(LocalPlayerController) : typeof(NetworkPlayerController));
                    currentPlayer.controller = inst.GetComponent<CustomController>();
                }
            }
            return true;
        });
        GameManager.CloseLoadInfo();

        // 스폰 메시지를 전달,
        // 그 위치에 스폰하기
        Vector3 spawnPos = new Vector3(Random.value, 1, Random.value);
        NetworkManager.SendMessage(NetworkManager.MessageType.Spawn, new NetworkManager.Spawn_Message() { pos_x = spawnPos.x, pos_y = spawnPos.y, pos_z = spawnPos.z });
        
    }
}
