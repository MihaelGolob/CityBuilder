using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState { Build, Destroy}

public class RoadBuilderStateMachine : MonoBehaviour {
    [SerializeField] private SelectSystem selectSystem;
    
    // private variables
    private BuildingState _currentState;
    
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
        }
    }
    
    // public methods
    public void SetState(BuildingState state) {
        _currentState = state;
        selectSystem.ChangeStateTransitions();
    }
}