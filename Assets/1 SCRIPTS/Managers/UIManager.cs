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

    private void Start() {
        buildButton.interactable = false;
    }

    public void OnBuildButtonPressed() {
        stateMachine.SetState(BuildingState.Build);
        buildButton.interactable = false;
        destroyButton.interactable = true;
    }
    
    public void OnDestroyButtonPressed() {
        stateMachine.SetState(BuildingState.Destroy);
        buildButton.interactable = true;
        destroyButton.interactable = false;
    }

    public void OnDestroyAllButtonPressed() {
        roadRenderer.RemoveAllRoads();
    }
    
    public void OnSaveButtonPressed() {
        roadRenderer.SaveMesh();
    }
}