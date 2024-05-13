using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSomething : MonoBehaviour
{
    public ResourceEnum.Prefab wantPrefab;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            GameManager.Instance.SoundManager.Play(ResourceEnum.BGM.time_for_adventure);
        }
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            GameManager.Instance.SoundManager.Play(ResourceEnum.SFX.coin, Vector3.zero);
        }
    }
}
