using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Appearance : MonoBehaviour
{
    [SerializeField] Animator anim;

    CustomCharacter owner;

    private void OnEnable()
    {
        ConnectCharacter(GetComponentInParent<CustomCharacter>());
    }

    private void OnDisable()
    {
        DisConnectCharacter();
    }

    public void ConnectCharacter(CustomCharacter target)
    {
        if (target == null) return;
        if( owner && owner != target) DisConnectCharacter();
        owner = target;
        target.AnimBool -= SetBool;
        target.AnimBool += SetBool;
        target.AnimFloat -= SetFloat;
        target.AnimFloat += SetFloat;
        target.AnimInt -= SetInt;
        target.AnimInt += SetInt;
        target.AnimTrigger -= SetTrigger;
        target.AnimTrigger += SetTrigger;
    }

    public void DisConnectCharacter()
    {
        if(owner == null) return;
        owner.AnimBool -= SetBool;
        owner.AnimFloat -= SetFloat;
        owner.AnimInt -= SetInt;
        owner.AnimTrigger -= SetTrigger;
        owner = null;
    }

    public void SetBool(string name, bool value) => anim?.SetBool(name, value);
    public void SetFloat(string name, float value) => anim?.SetFloat(name, value);
    public void SetInt(string name, int value) => anim?.SetInteger(name, value);
    public void SetTrigger(string name) => anim?.SetTrigger(name);

}
