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

    // �ȱ�
    public void Walk(float dest_x, float dest_y, float dest_z)
    {
        DoWalk?.Invoke(new Vector3(dest_x, dest_y, dest_z));
    }   
    
    public void Walk(Vector3 destination)
    {
        DoWalk?.Invoke(destination);
    }

    // ����
    public void Teleport(float dest_x, float dest_y, float dest_z)
    {
        DoTeleport?.Invoke(new Vector3(dest_x, dest_y, dest_z));
    }   
    
    public void Teleport(Vector3 destination)
    {
        DoTeleport?.Invoke(destination);
    }

    // ���� ����
    public void LookAt(float dest_x, float dest_y, float dest_z)
    {
        DoLookAt?.Invoke(new Vector3(dest_x, dest_y, dest_z));
    }   
    
    public void LookAt(Vector3 destination)
    {
        DoLookAt?.Invoke(destination);
    }

    // ����
    public void Attack(float pos_x, float pos_y, float pos_z, float rot_x, float rot_y, float rot_z, float scale_x, float scale_y, float scale_z, float duration, float damage)
    {
        DoAttack?.Invoke(new Vector3(pos_x, pos_y, pos_z), new Vector3(rot_x, rot_y, rot_z), new Vector3(scale_x, scale_y, scale_z), duration, damage);
    }

    // �� ������ ĳ���� �ϳ��� ���ϱ�
    protected CustomCharacter _controlledCharacter;
    public CustomCharacter ControlledCharacter => _controlledCharacter;
    public void Spawn(float dest_x, float dest_y, float dest_z)
    {
        if(_controlledCharacter)
        {
            // ������ ����
            _controlledCharacter.transform.position = new Vector3(dest_x, dest_y, dest_z);
            Walk(new Vector3(dest_x, dest_y, dest_z));
        }
        else
        {
            // ������ ����
            GameObject inst = PoolManager.Instantiate(ResourceEnum.Prefab.Player, new Vector3(dest_x, dest_y, dest_z));
            _controlledCharacter = inst.GetComponent<CustomCharacter>();
            // ���� �� ���� �ؾ���
            _controlledCharacter.Possesion(this);
        }
    }

}
