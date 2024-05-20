using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    protected virtual void MyStart()
    {
        
    }

    protected virtual void MyUpdate(float deltaTime)
    {
        
    }

    protected virtual void MyDestroy()
    {

    }

    protected virtual void OnEnable()
    {
        GameManager.ObjectStarts += MyStart;
        GameManager.ObjectUpdates += MyUpdate;
    }

    protected virtual void OnDisable()
    {
        GameManager.ObjectDestroies -= MyDestroy;
        GameManager.ObjectDestroies += MyDestroy;
        GameManager.ObjectUpdates -= MyUpdate;
        GameManager.ObjectStarts -= MyStart;
    }
}
