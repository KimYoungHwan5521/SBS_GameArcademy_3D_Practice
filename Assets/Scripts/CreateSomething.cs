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
            SoundManager.Play(ResourceEnum.BGM.time_for_adventure);
        }
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            SoundManager.Play(ResourceEnum.SFX.coin, Vector3.zero);
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            PoolManager.Destroy(PoolManager.Instantiate(ResourceEnum.Prefab.Arissa), 3f);
        }
        
        if(Input.GetKeyDown(KeyCode.Q)) 
        {
            NetworkManager.ClaimMatch();
        }
        
    }
}
