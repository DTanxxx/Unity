using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        // sometimes, you may see jitters in the camera trying to follow player's movement
        // the camera may move first, then player's animation kicks in -> not what we want
        // we can use Unity's Function Execution Order to fix the issue
        // we want player animation to kick in first, before our camera starts to follow it
        // in Unity docs, we see that the Game Loop functions contain a LateUpdate() function,
        // which is called after Update() -> we can make camera follow player in LateUpdate(),
        // so that we can always be sure that player animation kicks in first (in Update() in
        // Mover.cs) and camera moves after (in LateUpdate() in FolllowCamera.cs)
        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}
