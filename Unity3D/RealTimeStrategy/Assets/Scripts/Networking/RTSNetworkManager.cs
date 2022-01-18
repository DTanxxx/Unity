using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] GameObject unitSpawnerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // spawn an unit spawner on the server
        GameObject unitSpawnerInstance = Instantiate(
            unitSpawnerPrefab, 
            conn.identity.transform.position, 
            conn.identity.transform.rotation);

        // spawn the spawner on the network too so all clients have that object in their world
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }
}
