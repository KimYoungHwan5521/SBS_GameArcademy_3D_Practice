using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshCharacter : CustomCharacter
{
    [SerializeField] NavMeshAgent agent;

    Vector3 lastFramePosition;

    protected override void MyUpdate(float deltaTime)
    {
        base.MyUpdate(deltaTime);
        Vector3 positionDiff = transform.position - lastFramePosition;
        AnimFloat?.Invoke("HorizontalSpeed", positionDiff.magnitude / deltaTime);
        lastFramePosition= transform.position;

        Vector3 localDirection = transform.InverseTransformDirection(positionDiff.normalized);
        AnimFloat?.Invoke("MoveFoward", localDirection.z);
        AnimFloat?.Invoke("MoveRight", localDirection.x);
    }

    protected override void RegistrationFunction(CustomController targetController)
    {
        targetController.DoWalk -= Walk; 
        targetController.DoWalk += Walk;
        targetController.DoTeleport -= Teleport; 
        targetController.DoTeleport += Teleport;
        targetController.DoLookAt -= LookAt; 
        targetController.DoLookAt += LookAt;
        targetController.DoAttack -= Attack; 
        targetController.DoAttack += Attack;
    }

    protected override void UnRegistrationFunction(CustomController targetController)
    {
        targetController.DoWalk -= Walk;
        targetController.DoTeleport -= Teleport;
        targetController.DoLookAt -= LookAt;
        targetController.DoAttack -= Attack;
    }

    public virtual void Walk(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public virtual void Teleport(Vector3 destination)
    {
        agent.transform.position = destination;
        agent.SetDestination(destination);
    }

    public virtual void LookAt(Vector3 destination)
    {
        destination.y = transform.position.y;
        agent.transform.LookAt(destination);
    }

    public virtual void Attack(Vector3 position, Vector3 rotation, Vector3 scale, float duration, float damage)
    {
        AnimTrigger?.Invoke("Attack");
        GameObject hitBox = PoolManager.Instantiate(ResourceEnum.Prefab.HitBox, position, rotation);
        hitBox.transform.localScale = scale;
        Debug.Log($"damage : {damage}");
        Destroy(hitBox, duration);

    }
}
