using System;
using Michsky.MUIP;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {
    // singleton pattern
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    
    [Header("Script references")]
    [SerializeField] private RoadBuilderStateMachine stateMachine;
    [SerializeField] private RoadRenderer roadRenderer;

    [Header("UI elements")] 
    [SerializeField] private GameObject buildWindow;
    [SerializeField] private GameObject exitWindow;
    [SerializeField] private GameObject notificationWindow;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private ButtonManager handButton;
    [SerializeField] private ButtonManager buildWindowButton;
    [SerializeField] private ButtonManager destroyButton;
    [SerializeField] private ButtonManager buildRoadButton;
    [SerializeField] private ButtonManager trafficLightButton;
    
    // private 
    private NotificationSystem _notificationSystem;

    private void Awake() {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }

    private void Start() {
        OnNoneButtonPressed();
        exitWindow.SetActive(false);
        _notificationSystem = new NotificationSystem(ShowNotification, HideNotification, 3f);
    }

    private void Update() {
        buildWindowButton.Interactable(!buildWindow.activeSelf);
        handButton.Interactable(stateMachine.CurrentState != BuildingState.None);
        buildRoadButton.Interactable(stateMachine.CurrentState != BuildingState.Build);
        destroyButton.Interactable(stateMachine.CurrentState != BuildingState.Destroy);
        trafficLightButton.Interactable(stateMachine.CurrentState != BuildingState.PlaceTrafficLights);
    }
    
    #region Notification System
    
    public int AddNotification(string message) {
        return _notificationSystem.AddNotification(message);
    }
    
    public void RemoveNotification(int id) {
        _notificationSystem.RemoveNotification(id);
    }

    private void ShowNotification(string message) {
        notificationText.text = message;
        notificationWindow.SetActive(true);
    }
    
    private void HideNotification() {
        notificationWindow.SetActive(false);
    }
    
    #endregion
    
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