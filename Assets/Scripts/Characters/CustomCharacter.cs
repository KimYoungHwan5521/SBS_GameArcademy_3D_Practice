using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomCharacter : MonoBehaviour
{
    CustomController _controller;
    public virtual CustomController Controller => _controller;

    public virtual bool TryPossesion() => _controller == null;

    protected abstract void RegistrationFunction(CustomController targetController);
    protected abstract void UnRegistrationFunction(CustomController customController);

    public virtual void Possesion(CustomController targetController)
    {
        if (!TryPossesion()) return;
        _controller = targetController;

        RegistrationFunction(targetController);
    }

    public virtual void UnPossess()
    {
        if (_controller == null) return;
        UnRegistrationFunction(_controller);
        _controller = null;
    }
}
