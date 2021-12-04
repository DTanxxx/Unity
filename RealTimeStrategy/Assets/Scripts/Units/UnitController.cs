using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            // we have clicked on a targetable unit
            // now we want to make sure this unit is NOT owned by this client (don't want friendly fire)
            if (!target.hasAuthority)
            {
                TryTarget(target);
            }
            else
            {
                TryMove(hit.point);
            }

            return;
        }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 destination)
    {
        foreach (var unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(destination);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach (var unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }
}
