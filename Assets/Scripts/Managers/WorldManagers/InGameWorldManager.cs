using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameWorldManager : WorldManager
{
    protected override IEnumerator Initiate()
    {
        yield return base.Initiate();
        // ������ �⺻��Ҹ� �ε��ߴµ� �ε�â�� ������ �ʴ� ���
        // : ��� �÷��̾ �ε尡 ������ �ʾ��� ��
        NetworkManager.SendMessage(NetworkManager.MessageType.LoadComplete, new NetworkManager.LoadComplete_Message() { max = 2, current = 2 });
        GameManager.ClaimLoadInfo("Waiting for other players");
        yield return new WaitUntil(() =>
        {
            foreach(NetworkManager.PlayerInfo currentPlayer in NetworkManager.GetAllUser())
            {
                if (currentPlayer.isLoaded == false) return false;
                else if(currentPlayer.controller == null)
                {
                    // ������Ʈ�� ������ ���ӿ�����Ʈ �ȿ� �־�� �Ѵ�.
                    var inst = new GameObject($"{currentPlayer.record.m_nickname}'s controller", NetworkManager.myGameRecord == currentPlayer ? typeof(LocalPlayerController) : typeof(NetworkPlayerController));
                    currentPlayer.controller = inst.GetComponent<CustomController>();
                }
            }
            return true;
        });
        GameManager.CloseLoadInfo();

        // ���� �޽����� ����,
        // �� ��ġ�� �����ϱ�
        Vector3 spawnPos = new Vector3(Random.value, 1, Random.value);
        NetworkManager.SendMessage(NetworkManager.MessageType.Spawn, new NetworkManager.Spawn_Message() { pos_x = spawnPos.x, pos_y = spawnPos.y, pos_z = spawnPos.z });
        
    }
}
