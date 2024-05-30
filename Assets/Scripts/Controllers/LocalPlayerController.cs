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

            // Attack
            if(Input.GetKeyDown(KeyCode.A) && ControlledCharacter)
            {
                Vector3 attackDirection = hit.point - ControlledCharacter.transform.position;
                attackDirection.y = 0;
                attackDirection.Normalize();
                //attackDirection = attackDirection.GetVerticalRotate(-30).normalized;
                Vector3 attackRotation = (Quaternion.LookRotation(attackDirection)).eulerAngles;
                NetworkManager.SendMessage(NetworkManager.MessageType.Attack, new NetworkManager.Attack_Message()
                {
                    pos_x = ControlledCharacter.transform.position.x + attackDirection.x,
                    pos_y = ControlledCharacter.transform.position.y,
                    pos_z = ControlledCharacter.transform.position.z + attackDirection.z,
                    rot_x = attackRotation.x,
                    rot_y = attackRotation.y,
                    rot_z = attackRotation.z,
                    scale_x = 1,
                    scale_y = 2,
                    scale_z = 1,
                    duration = 1f,
                    damage = 10
                });
                //StartCoroutine(Skill(ControlledCharacter, null, attackRotation));
            }
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

    IEnumerator Skill(CustomCharacter from, CustomCharacter to, Vector3 attackRotation)
    {
        for(int i=0; i<128; i++)
        {
            Vector3 direction = (i * 3f).GetAngledVector();
            Quaternion quaternion= Quaternion.LookRotation(direction);
            NetworkManager.SendMessage(NetworkManager.MessageType.Attack, new NetworkManager.Attack_Message()
            {
                pos_x = (from.transform.position + direction).x,
                pos_y = (from.transform.position + direction).y,
                pos_z = (from.transform.position + direction).z,
                rot_x = attackRotation.x,
                rot_y = attackRotation.y,
                rot_z = attackRotation.z,
                scale_x = 2,
                scale_y = 1,
                scale_z = 1,
                duration = 1f,
                damage = 10
            });
            yield return new WaitForSeconds(0.1f);
        }
    }

}
