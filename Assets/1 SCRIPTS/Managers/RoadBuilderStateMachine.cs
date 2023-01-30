using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState { Build, Destroy, PlaceVehicle, None}

public class RoadBuilderStateMachine : MonoBehaviour {
    [SerializeField] private SelectSystem selectSystem;
    [Header("Vehicle settings")]
    [SerializeField] private List<GameObject> vehiclePrefabs;
    [SerializeField] private List<Transform> vehicleTargets;
    
    // private variables
    private BuildingState _currentState;
    private VehiclePlacement _vehiclePlacement;
    
    // public getters
    public BuildingState CurrentState => _currentState;
    
    // private methods
    private void Update() {
        // lock selection if mouse over any UI element
        selectSystem.LockSelection = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        switch (_currentState) {
            case BuildingState.Build:
                selectSystem.Build();
                break;
            case BuildingState.Destroy:
                selectSystem.Destroy();
                break;
            case BuildingState.PlaceVehicle:
                _vehiclePlacement ??= new VehiclePlacement(selectSystem, SetState, vehiclePrefabs, vehicleTargets);
                _vehiclePlacement.PlaceVehicle();
                break;
        }
    }
    
    // public methods
    public void SetState(BuildingState state) {
        _currentState = state;
        selectSystem.ChangeStateTransitions();
        _vehiclePlacement?.DestroyVehicle();
        _vehiclePlacement = null;
    }
}