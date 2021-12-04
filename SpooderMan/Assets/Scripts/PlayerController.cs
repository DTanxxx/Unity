using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GrappleGun gun = null;

    private PlayerMovement playerMovement;
    public bool isHanging = false;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleFireInput();
    }

    private void HandleFireInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // left key down fire
            gun.Fire();
        }
        if (Input.GetMouseButtonUp(0))
        {
            // destroy rope
            gun.stopExtending = false;
            isHanging = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            FindObjectOfType<GrappleGun>().GetComponent<Rigidbody>().useGravity = true;
            FindObjectOfType<GrappleGun>().GetComponent<Rigidbody>().isKinematic = false;
            gun.DestroyRope();
        }
    }

    private void HandleMovementInput()
    {
        if (isHanging) { return; }
        playerMovement.RotateX(Time.deltaTime, Input.GetAxis("Mouse X"));
        playerMovement.RotateY(Time.deltaTime, Input.GetAxis("Mouse Y"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // jump
            playerMovement.Jump();
        }
    }
}
