using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    #region Server

    [Command]  // client is going to tell the server what the client's target is
    public void CmdSetTarget(GameObject gameObject)
    {
        if (!gameObject.TryGetComponent<Targetable>(out Targetable target))
        {
            return;
        }

        this.target = target;
    }

    [Server]  // here the method is not called by any client so it is not a [Command]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion
}
