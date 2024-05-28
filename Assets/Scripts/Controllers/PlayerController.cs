using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.ControllerUpdates -= PlayerMove;
        GameManager.ControllerUpdates += PlayerMove;
    }

    void OnDisable()
    {
        GameManager.ControllerUpdates -= PlayerMove;
        
    }

    void PlayerMove(float deltaTime)
    {
        // 마우스위치에서 선을 발사
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 100, Color.yellow);
        if(Physics.Raycast(mouseRay, out RaycastHit hit, 100))
        {
            if(Input.GetMouseButtonDown(1))
            {
                NetworkManager.SendMessage(NetworkManager.MessageType.Move, new NetworkManager.Move_Message() { pos_x = hit.point.x, pos_y = hit.point.y, pos_z = hit.point.z });
            }
        }
        
    }
}
