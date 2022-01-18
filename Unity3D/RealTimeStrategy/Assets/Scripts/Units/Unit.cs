using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;  // using events to implement selection logic 
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;

    // event for spawning/despawning this unit
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    // this method is called only on the server or host (in this case, host), when Unit is spawned by UnitSpawner
    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerOnUnitSpawned?.Invoke(this);
    }

    // this method is called only on the server or host (in this case, host), when Unit is destroyed
    public override void OnStopServer()
    {
        base.OnStopServer();

        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    // this method is called only on the client or host, when Unit is spawned
    public override void OnStartClient()
    {
        base.OnStartClient();

        // only fire the event if I am client only (NOT a host) and if this Unit is owned by me (this client)
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    // this method is called only on the client or host, when Unit is destroyed
    public override void OnStopClient()
    {
        base.OnStopClient();

        // ...
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority)
        {
            return;
        }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
        {
            return;
        }

        onDeselected?.Invoke();
    }

    #endregion
}
