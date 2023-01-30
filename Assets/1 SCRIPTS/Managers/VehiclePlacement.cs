using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class VehiclePlacement {
    private GameObject _selectedVehicle;
    private Vehicle _vehicleScript;
    private Camera _mainCamera;
    
    private Transform _startPoint;
    private Transform _endPoint;
    
    private bool _vehiclePlaced = false;

    private SelectSystem _selectSystem;
    private Action<BuildingState> _changeState;

    public VehiclePlacement(SelectSystem selectSystem, Action<BuildingState> changeState, List<GameObject> vehicles, List<Transform> targets) {
        if (targets.Count < 2) throw new Exception("No vehicles to choose from!");
        if (vehicles.Count == 0) throw new Exception("Not enough targets!");
        
        _selectSystem = selectSystem;
        _changeState = changeState;
        
        var prefab = vehicles[Random.Range(0, vehicles.Count)];
        _selectedVehicle = Object.Instantiate(prefab);
        _vehicleScript = _selectedVehicle.GetComponent<Vehicle>();
        
        _vehicleScript.enabled = false;
        _selectedVehicle.GetComponent<NavMeshAgent>().enabled = false;
        
        _startPoint = targets[Random.Range(0, targets.Count)];
        do {
            _endPoint = targets[Random.Range(0, targets.Count)];
        } while (_endPoint == _startPoint);
        
        _vehicleScript.AddTarget(_startPoint);
        _vehicleScript.AddTarget(_endPoint);
        
        _mainCamera = Camera.main;
    }

    public void PlaceVehicle() {
        if (_vehiclePlaced) return;
        
        var mousePosition = Input.mousePosition;
        var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 100));
        
        var ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, _mainCamera.farClipPlane)) {
            mouseWorldPosition = hit.point;
        }

        _selectedVehicle.transform.position = new Vector3(mouseWorldPosition.x,2, mouseWorldPosition.z);
        
        if (Input.GetMouseButtonDown(0) && !_selectSystem.LockSelection) {
            DropVehicle();
        }
    }
    
    public void DestroyVehicle() {
        if (_vehiclePlaced) return;
        Object.Destroy(_selectedVehicle);
    }

    private void DropVehicle() {
        _vehiclePlaced = true;
        
        _selectedVehicle.transform.position = new Vector3(_selectedVehicle.transform.position.x, 0, _selectedVehicle.transform.position.z);
        _selectedVehicle.GetComponent<NavMeshAgent>().enabled = true;
        _vehicleScript.enabled = true;
        _vehicleScript.StartVehicle();
        
        _changeState(BuildingState.None);
    }
}