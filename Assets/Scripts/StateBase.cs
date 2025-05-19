using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    protected StateUser user;

    public virtual void OnStateStart(StateUser user)
    {
        this.user = user;
    }

    public abstract void OnStateStay();

    public abstract void OnStateEnd();
}
