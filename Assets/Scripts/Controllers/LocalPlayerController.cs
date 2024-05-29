using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerController : CustomController
{
    const float wantNetworkUpdateTime = 0.05f;
    float lastUpdateTime;

    protected override void MyUpdate(float deltaTime)
    {
        base.MyUpdate(deltaTime);

        // 마우스위치에서 선을 발사
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = LayerMask.GetMask("MouseChecker");
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.yellow);
        //                                                      ~ : 비트 반전
        if (Physics.Raycast(cameraRay, out RaycastHit hit, 100, ~mask))
        {
            // Walk
            if (Input.GetMouseButtonDown(1))
            {
                NetworkManager.SendMessage(NetworkManager.MessageType.Walk, new NetworkManager.Walk_Message() { pos_x = hit.point.x, pos_y = hit.point.y, pos_z = hit.point.z });
            }
        }

        // Attack
        if(Input.GetKeyDown(KeyCode.A))
        {
            NetworkManager.SendMessage(NetworkManager.MessageType.Attack, new NetworkManager.Attack_Message()
            {
                pos_x = 0,
                pos_y = 0,
                pos_z = 0,
                rot_x = 0,
                rot_y = 0,
                rot_z = 0,
                scale_x = 1,
                scale_y = 1,
                scale_z = 4,
                duration = 1f,
                damage = 10
            });
        }

        if(Time.time - lastUpdateTime >= wantNetworkUpdateTime)
        {
            lastUpdateTime= Time.time;
            if (Physics.Raycast(cameraRay, out RaycastHit hit2, 100, mask))
            {
                // LookAt
                NetworkManager.SendMessage(NetworkManager.MessageType.LookAt, new NetworkManager.LookAt_Message() { pos_x = hit2.point.x, pos_y = hit2.point.y, pos_z = hit2.point.z });

            }


        }


    }
}
