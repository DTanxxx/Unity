using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            // disable NavMesh if player/enemy is dead
            navMeshAgent.enabled = !health.IsDead();

            // update animation of player
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;            
        }

        private void UpdateAnimator()
        {
            // get the global velocity from Nav Mesh Agent
            Vector3 velocity = navMeshAgent.velocity;

            // convert the global velocity to a more meaningful local velocity relative to the player
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            // this is needed because the global velocity gives information about the player in the
            // world space, however for our animator, it only needs to know whether the player is moving
            // forward, so we use InverseTransformDirection() to say that "no matter where you are in the world,
            // convert to a local velocity relative to the player, eg moving forward at speed of 5 units"

            // set the animator's blend value to localVelocity's z component
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //// TODO: Another way to save multiple parameters -> using a struct
        //[System.Serializable]
        //struct MoverSaveData
        //{
        //    public SerializableVector3 position;
        //    public SerializableVector3 rotation;
        //}

        //public object CaptureState()
        //{
        //    // TODO: One way to save multiple parameters -> using a dictionary
        //    //Dictionary<string, object> data = new Dictionary<string, object>();
        //    //data["position"] = new SerializableVector3(transform.position);
        //    //data["rotation"] = new SerializableVector3(transform.eulerAngles);
        //    //return data;

        //    MoverSaveData data = new MoverSaveData();
        //    data.position = new SerializableVector3(transform.position);
        //    data.rotation = new SerializableVector3(transform.eulerAngles);
        //    return data;
        //}

        //public void RestoreState(object state)
        //{
        //    //Dictionary<string, object> data = (Dictionary<string, object>)state;

        //    MoverSaveData data = (MoverSaveData)state;
        //    GetComponent<NavMeshAgent>().enabled = false;
        //    //transform.position = ((SerializableVector3)data["position"]).ToVector();
        //    //transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();

        //    transform.position = data.position.ToVector();
        //    transform.eulerAngles = data.rotation.ToVector();
        //    GetComponent<NavMeshAgent>().enabled = true;
        //}
    }
}
