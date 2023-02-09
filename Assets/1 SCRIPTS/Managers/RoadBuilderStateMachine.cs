using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState { Build, Destroy, PlaceVehicle, PlaceTrafficLights, None}

public class RoadBuilderStateMachine : MonoBehaviour {
    [SerializeField] private SelectSystem selectSystem;
    [Header("Vehicle settings")]
    [SerializeField] private List<GameObject> vehiclePrefabs;
    [SerializeField] private GameObject targetPrefab;
    
    // private variables
    private BuildingState _currentState;
    private VehiclePlacement _vehiclePlacement;
    private List<GameObject> _targets = new (); 
    
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
            case BuildingState.PlaceTrafficLights:
                selectSystem.PlaceTrafficLights();
                break;
            case BuildingState.PlaceVehicle:
                _vehiclePlacement ??= new VehiclePlacement(selectSystem, SetState, vehiclePrefabs, InstantiateTarget, RemoveTargetsWithDelay);
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

    private void InstantiateTarget(Vector3 pos) {
        var tmp = Instantiate(targetPrefab, pos, Quaternion.identity);
        _targets.Add(tmp);
    }

    private void RemoveTargetsWithDelay() {
        StartCoroutine(Delay(1));
    }

    private IEnumerator Delay(int seconds) {
        yield return new WaitForSeconds(seconds);
        
        foreach (var point in _targets) {
            Destroy(point);
        }
        _targets.Clear();
    }
}