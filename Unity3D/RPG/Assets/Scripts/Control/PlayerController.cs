using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.EventSystems;
using System;
using UnityEngine.AI;
using GameDevTV.Inventories;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1.0f;
        [SerializeField] float raycastRadius = 1f;

        bool movementStarted = false;  // used for preventing player movement when clicking on pickup
        bool isDraggingUI = false;  // used for preventing player movement when dragging UI

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
            CheckSpecialAbilityKeys();
            if (Input.GetMouseButtonUp(0))
            {
                movementStarted = false;
            }

            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private void CheckSpecialAbilityKeys()
        {
            ActionStore actionStore = GetComponent<ActionStore>();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(0, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                actionStore.Use(1, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                actionStore.Use(2, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                actionStore.Use(3, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                actionStore.Use(4, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                actionStore.Use(5, gameObject);
            }
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }

            // check if the cursor is hovered over an UI component on screen
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            
            // if still dragging UI, then don't interact with other things eg movement
            if (isDraggingUI)
            {
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            // raycast through the world and get all the raycast hits, then get all the IRaycastable components
            // on each game object hit by raycast
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                // note that we can call GetComponent<>() on a component as well (not
                // necessarily on a game object) which will return the specified component
                // on the same game object (if there is one)
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    // check if a given IRaycastable component can handle the raycast
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            // create a ray with origin at the position of mouse click on the 2D screen,
            // and direction, from origin of camera to the location of mouse click on the 2D screen,
            // so that when we extend the direction vector, it will reach the same point on the 3D terrain
            //Ray ray = GetMouseRay();

            //RaycastHit hit;
            Vector3 target;

            // we pass in 'ray' and 'hit'
            // the 'out' keyword means that the method will take in 'hit' variable and change it inside the method,
            // so that after the method completes, our 'hit' variable will contain some data (in this
            // case, where the ray has hit the terrain)
            //bool hasHit = Physics.Raycast(ray, out hit);
            bool hasHit = RaycastNavMesh(out target);

            // if hasHit is true, ie the ray has hit something, move player to that hit location
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButtonDown(0))
                {
                    movementStarted = true;
                }
                
                if (Input.GetMouseButton(0) && movementStarted)
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;

            // draw the ray, extending its direction (which is a unit length Vector3) by a factor of 100
            //Debug.DrawRay(lastRay.origin, lastRay.direction * 100);
        }

        // check if the mouse ray hits anywhere on the NavMesh
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;

            // raycast to terrain
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            // find nearest NavMesh point
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            // return true if so
            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);  // to allow margin of error while clicking with cursor

            // build array of keys (ie distances)
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            // sort hits array by an array of keys (ie the distances array)
            Array.Sort(distances, hits);
            return hits;
        }
    }
}
