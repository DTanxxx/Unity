using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();

    public List<Unit> GetUnits()
    {
        return myUnits;
    }

    #region Server

    // called only on server or host (in this case, host), subscribe to ServerOnUnitSpawned and ServerOnUnitDespawned events
    public override void OnStartServer()
    {
        base.OnStartServer();

        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    // called only on server or host (in this case, host), unsubscribe from the events
    public override void OnStopServer()
    {
        base.OnStopServer();

        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    // called when Unit is spawned, only on the server or host (host in this case)
    public void ServerHandleUnitSpawned(Unit unit)
    {
        // only add the unit if the unit's client is the same as this player object's client
        // in other words, if the server or host that the unit belongs to is the same as the server or host this player object belongs to
        // (this isn't necessary unless there are multiple servers)
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Add(unit);
    }

    // called when Unit is destroyed, only on the server or host (host in this case)
    public void ServerHandleUnitDespawned(Unit unit)
    {
        // only remove ... 
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    #endregion

    #region Client

    // called only on client or host (this means the host would have both OnStartServer and OnStartClient called,
    // resulting in duplication when adding/removing units), subscribe to events
    public override void OnStartClient()
    {
        base.OnStartClient();

        // only subscribe to events if this client is NOT the host (otherwise unit adding would be duplicated)
        if (!isClientOnly)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitSpawned;
    }

    // called only on client or host, unsubscribe from the events
    public override void OnStopClient()
    {
        base.OnStopClient();

        // only unsubscribe ... 
        if (!isClientOnly)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    // called when Unit is spawned, only on the client or host (there won't be duplication here since the host is NOT subscribed
    // to the AuthorityOnUnitSpawned/Despawned events)
    public void AuthorityHandleUnitSpawned(Unit unit)
    {
        // add unit only to the player that this client has authority over (since the all the players will be registered for the event,
        // we want the player that is owned by me out of the ones controlled by other clients)
        if (!hasAuthority)
        {
            return;
        }

        myUnits.Add(unit);
    }

    // called when Unit is destroyed, only on the client or host
    public void AuthorityHandleUnitDespawned(Unit unit)
    {
        // remove ...
        if (!hasAuthority)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    #endregion
}
