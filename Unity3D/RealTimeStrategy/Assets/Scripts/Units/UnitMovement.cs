using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10.0f;

    #region Server

    [ServerCallback]  // this Update should only work on the server; clients do not need to handle the movement logic
    // it's a "Callback" because Update is a Unity callback method and we don't call it ourselves; if we need to
    // mark some method that we defined ourselves, then we would use [Server] instead
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        // chasing code
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                // if distance between us and target is greater than a defined chaseRange, chase
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                // stop chasing when distance is smaller than chaseRange and if agent has a path
                agent.ResetPath();
            }

            return;
        }

        // check first if this unit has a path calculated
        // path calculation takes more than one frame so if this check doesn't take place then the
        // agent.remainingDistance would stay the same and is <= agent.stoppingDistance 
        // therefore we would reset the path, which means the unit wouldn't even move
        // however if the agent do have a path, then agent.remainingDistance is updated correctly and we
        // would not reset path right away as remainingDistance > stoppingDistance
        if (!agent.hasPath)
        {
            return;
        }

        // if this unit is within a certain stopping distance (set in the editor), remove/reset the nav mesh path
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 destination)
    {
        // first clear the target for this unit
        targeter.ClearTarget();

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(destination, out hit, 1.0f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(hit.position);
    }

    #endregion
}
