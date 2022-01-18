using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour  // not a NetworkBehaviour because on each client, there will
// only be one UnitSelectionHandler game object (there won't be other UnitSelectionHandler objects from other
// clients on this client)
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea = null;

    private Camera mainCamera;
    private RTSPlayer player;

    private Vector2 startPosition;  // for multiple selection box

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;        
    }

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea(); 
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            // if left mouse button is held down, update the unit selection area rect transform
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        // if we are holding shift key, we are allowed to create another selection area without erasing the
        // currently selected units
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            // deselect, clear the list, then start to create a selection area
            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }

            SelectedUnits.Clear();
        }

        // enable selection area
        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        // call UpdateSelectionArea() as we want the selection area to have the correct transform in the same
        // frame as this method is called
        UpdateSelectionArea();
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        // if we just clicked (selection area has size 0) then do a point raycast
        if (unitSelectionArea.sizeDelta.magnitude == 0.0f)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
            {
                return;
            }

            if (!unit.hasAuthority)
            {
                return;
            }

            // if this unit is already selected, then don't add it again (avoid duplicates in SelectedUnits)
            if (SelectedUnits.Contains(unit))
            {
                return;
            }

            // if we reach here, it means we have hit something, that something is an unit, and the unit belongs to us (this client)
            SelectedUnits.Add(unit);
            unit.Select();

            return;
        }

        // calculate the bottom left and upper right coordinates of the selection area to find the bounds
        // of multiselection
        Vector2 bottomLeftPoint = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 upperRightPoint = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);
        
        // iterate through units from the list in RTSPlayer and check if each of them has screenspace position
        // that is inside our bounds coordinates
        foreach (var unit in player.GetUnits())
        {
            // if this unit is already selected, then don't add it again (avoid duplicates in SelectedUnits)
            // this can happen if we keep holding shift and reselect the same units over and over
            if (SelectedUnits.Contains(unit))
            {
                continue;
            }

            // convert world position of unit to screen position
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x > bottomLeftPoint.x && screenPosition.x < upperRightPoint.x
                && screenPosition.y > bottomLeftPoint.y && screenPosition.y < upperRightPoint.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = Mathf.Abs(mousePosition.x - startPosition.x);
        float areaHeight = Mathf.Abs(mousePosition.y - startPosition.y);

        // set selection area's size
        unitSelectionArea.sizeDelta = new Vector2(areaWidth, areaHeight);

        // set selection area's position (note that in the editor, the rect transform has anchors preset to bottom left
        // to match our mouse's coordinate system)
        unitSelectionArea.anchoredPosition = startPosition + (mousePosition - startPosition) / 2;
    }
}
