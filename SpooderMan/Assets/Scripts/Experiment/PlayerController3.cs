using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3 : MonoBehaviour
{
    private Grapple3 gun;
    private PlayerMovement3 playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement3>();
        gun = GetComponent<Grapple3>();
    }

    void Update()
    {
        HandleGrappleGuide();
        HandleMovementInput();
        HandleFireInput();
    }

    private void HandleGrappleGuide()
    {
        gun.ShowGrappleGuide();
    }

    private void HandleFireInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gun.StartGrapple();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gun.EndGrapple();
        }
    }

    private void HandleMovementInput()
    {
        playerMovement.RotateX(Time.deltaTime, Input.GetAxis("Mouse X"));
        playerMovement.RotateY(Time.deltaTime, Input.GetAxis("Mouse Y"));
    }
}
