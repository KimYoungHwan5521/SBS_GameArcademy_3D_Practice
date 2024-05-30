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
    public void Attack(ref NetworkManager.Attack_Message message)
    {
        DoAttack?.Invoke(new Vector3(message.pos_x, message.pos_y, message.pos_z), new Vector3(message.rot_x, message.rot_y, message.rot_z), new Vector3(message.scale_x, message.scale_y, message.scale_z), message.duration, message.damage);
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
