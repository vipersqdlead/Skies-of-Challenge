using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StateUser
{
    public void ChangeState(StateBase newState);

    public void ExecuteStateOnUpdate();

    public void ExecuteStateOnStart();
}