using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void WalkDelegate(Vector3 wantLocation);
public delegate void TeleportDelegate(Vector3 wantLocation);
public delegate void AttackDelegate(Vector3 wantLocation, Vector3 wantRotationEuler, Vector3 wantScale, float duration, float damage);
public delegate void LookAtDelegate(Vector3 wantLocation);

public class CustomController : MyComponent
{
    public WalkDelegate DoWalk;
    public TeleportDelegate DoTeleport;
    public AttackDelegate DoAttack;
    public LookAtDelegate DoLookAt;

    // 걷기
    public void Walk(float dest_x, float dest_y, float dest_z)
    {
        DoWalk?.Invoke(new Vector3(dest_x, dest_y, dest_z));
    }   
    
    public void Walk(Vector3 destination)
    {
        DoWalk?.Invoke(destination);
    }

    // 텔포
    public void Teleport(float dest_x, float dest_y, float dest_z)
    {
        DoTeleport?.Invoke(new Vector3(dest_x, dest_y, dest_z));
    }   
    
    public void Teleport(Vector3 destination)
    {
        DoTeleport?.Invoke(destination);
    }

    // 보는 방향
    public void LookAt(float dest_x, float dest_y, float dest_z)
    {
        DoLookAt?.Invoke(new Vector3(dest_x, dest_y, dest_z));
    }   
    
    public void LookAt(Vector3 destination)
    {
        DoLookAt?.Invoke(destination);
    }

    // 공격
    public void Attack(ref NetworkManager.Attack_Message message)
    {
        DoAttack?.Invoke(new Vector3(message.pos_x, message.pos_y, message.pos_z), new Vector3(message.rot_x, message.rot_y, message.rot_z), new Vector3(message.scale_x, message.scale_y, message.scale_z), message.duration, message.damage);
    }

    // 이 게임은 캐릭터 하나만 쓰니까
    protected CustomCharacter _controlledCharacter;
    public CustomCharacter ControlledCharacter => _controlledCharacter;
    public void Spawn(float dest_x, float dest_y, float dest_z)
    {
        if(_controlledCharacter)
        {
            // 있으면 텔포
            _controlledCharacter.transform.position = new Vector3(dest_x, dest_y, dest_z);
            Walk(new Vector3(dest_x, dest_y, dest_z));
        }
        else
        {
            // 없으면 생성
            GameObject inst = PoolManager.Instantiate(ResourceEnum.Prefab.Player, new Vector3(dest_x, dest_y, dest_z));
            _controlledCharacter = inst.GetComponent<CustomCharacter>();
            // 생성 후 빙의 해야함
            _controlledCharacter.Possesion(this);
        }
    }

}
