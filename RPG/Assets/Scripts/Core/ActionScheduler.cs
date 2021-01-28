using UnityEngine;
using System.Collections;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        public void StartAction(IAction action)
        {
            if (currentAction == action) { return; }
            if (currentAction != null)
            {
                // since any class that implements IAction interface will need to
                // implement a Cancel() method, we can be sure that we can call Cancel()
                // on the currentAction, without knowing whether currentAction is a Fighter
                // or a Mover -> no dependencies required!!
                currentAction.Cancel();
            }
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            // cancel currentAction, then set currentAction to null
            StartAction(null);
        }
    }
}

