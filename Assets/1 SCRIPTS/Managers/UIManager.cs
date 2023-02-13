using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Script references")]
    [SerializeField] private RoadBuilderStateMachine stateMachine;
    [SerializeField] private RoadRenderer roadRenderer;

    [Header("UI elements")] 
    [SerializeField] private GameObject buildWindow;
    [SerializeField] private GameObject exitWindow;
    [SerializeField] private ButtonManager handButton;
    [SerializeField] private ButtonManager buildWindowButton;
    [SerializeField] private ButtonManager destroyButton;
    [SerializeField] private ButtonManager buildRoadButton;
    [SerializeField] private ButtonManager trafficLightButton;
    [SerializeField] private SliderManager zoomSlider;

    private void Start() {
        OnNoneButtonPressed();
        exitWindow.SetActive(false);
    }

    private void Update() {
        buildWindowButton.Interactable(!buildWindow.activeSelf);
        handButton.Interactable(stateMachine.CurrentState != BuildingState.None);
        buildRoadButton.Interactable(stateMachine.CurrentState != BuildingState.Build);
        destroyButton.Interactable(stateMachine.CurrentState != BuildingState.Destroy);
        trafficLightButton.Interactable(stateMachine.CurrentState != BuildingState.PlaceTrafficLights);
    }
    
    #region Button Events

    public void OnCloseBuildWindow() {
        buildWindow.SetActive(false);
        stateMachine.SetState(BuildingState.None);
    }

    public void OnBuildButtonPressed() {
        buildWindow.SetActive(!buildWindow.activeSelf);
        stateMachine.SetState(BuildingState.None);
    }

    public void OnBuildRoadButtonPressed() {
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
    
    public void OnPlaceTrafficLightsButtonPressed() {
        stateMachine.SetState(BuildingState.PlaceTrafficLights);
    }

    public void OnExitButtonPressed() {
        exitWindow.SetActive(true);
    }
    
    public void OnExitApplicationButtonPressed() {
        Application.Quit();
    }
    
    #endregion
}