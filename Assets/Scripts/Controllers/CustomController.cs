using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void WalkDelegate(Vector3 wantLocation);
public delegate void TeleportDelegate(Vector3 wantLocation);
public delegate void AttackDelegate(Vector3 wantLocation, Vector3 wantRotation, Vector3 wantScale, float duration, float damage);
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
    public void Attack(float pos_x, float pos_y, float pos_z, float rot_x, float rot_y, float rot_z, float scale_x, float scale_y, float scale_z, float duration, float damage)
    {
        DoAttack?.Invoke(new Vector3(pos_x, pos_y, pos_z), new Vector3(rot_x, rot_y, rot_z), new Vector3(scale_x, scale_y, scale_z), duration, damage);
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
