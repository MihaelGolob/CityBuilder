using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Script references")]
    [SerializeField] private RoadBuilderStateMachine stateMachine;
    [SerializeField] private RoadRenderer roadRenderer;

    [Header("UI elements")] 
    [SerializeField] private Button buildButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private Button noneButton;
    [SerializeField] private Button newVehicleButton;

    private void Start() {
        OnNoneButtonPressed();
    }

    private void Update() {
        // refresh button states
        buildButton.interactable = stateMachine.CurrentState != BuildingState.Build;
        destroyButton.interactable = stateMachine.CurrentState != BuildingState.Destroy;
        noneButton.interactable = stateMachine.CurrentState != BuildingState.None;
        newVehicleButton.interactable = stateMachine.CurrentState != BuildingState.PlaceVehicle;
    }

    public void OnBuildButtonPressed() {
        stateMachine.SetState(BuildingState.Build);
    }

    public void OnNewVehicleButtonPressed() {
        stateMachine.SetState(BuildingState.PlaceVehicle);
    }

    public void OnNoneButtonPressed() {
        stateMachine.SetState(BuildingState.None);
    }
    
    public void OnDestroyButtonPressed() {
        stateMachine.SetState(BuildingState.Destroy);
    }

    public void OnDestroyAllButtonPressed() {
        roadRenderer.RemoveAllRoads();
    }
    
    public void OnSaveButtonPressed() {
        roadRenderer.SaveMesh();
    }
}