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
   
    private bool _vehiclePlaced = false;

    private SelectSystem _selectSystem;
    private Action<BuildingState> _changeState;
    private Action<Vector3> _instantiateTarget;
    private Action _removeTargets;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private int _positionsAssigned;
    private int _notificationCount;

    public VehiclePlacement(SelectSystem selectSystem, Action<BuildingState> changeState, List<GameObject> vehicles, Action<Vector3> instantiateTarget, Action removeTargets) {
        if (vehicles.Count == 0) throw new Exception("Not enough targets!");
        
        _selectSystem = selectSystem;
        _changeState = changeState;
        _instantiateTarget = instantiateTarget;
        _removeTargets = removeTargets;
        
        var prefab = vehicles[Random.Range(0, vehicles.Count)];
        _selectedVehicle = Object.Instantiate(prefab);
        _vehicleScript = _selectedVehicle.GetComponent<Vehicle>();
        
        _vehicleScript.enabled = false;
        _selectedVehicle.GetComponent<NavMeshAgent>().enabled = false;
        
        _mainCamera = Camera.main;
    }

    public void PlaceVehicle() {
        if (_vehiclePlaced) {
            SelectDrivingPoints();
            return;
        }
        
        var mouseWorldPosition = MouseRaycast();
        _selectedVehicle.transform.position = new Vector3(mouseWorldPosition.x,2, mouseWorldPosition.z);
        
        if (Input.GetMouseButtonDown(0) && !_selectSystem.LockSelection) {
            DropVehicle();
        }
    }
    
    private Vector3 MouseRaycast() {
        var mousePosition = Input.mousePosition;
        var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 100));
        
        var ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, _mainCamera.farClipPlane)) {
            mouseWorldPosition = hit.point;
        }

        return mouseWorldPosition;
    }
    
    public void DestroyVehicle() {
        if (_vehiclePlaced) return;
        Object.Destroy(_selectedVehicle);
    }

    private void DropVehicle() {
        _vehiclePlaced = true;
        _selectedVehicle.transform.position = new Vector3(_selectedVehicle.transform.position.x, 0, _selectedVehicle.transform.position.z);
    }

    private void SelectDrivingPoints() {
        var pos = MouseRaycast();

        if (_positionsAssigned == 0 && _notificationCount == 0) {
            UIManager.Instance.AddNotification("Click on a starting point!");
            _notificationCount++;
        }
        else if (_positionsAssigned == 1 && _notificationCount == 1) {
            UIManager.Instance.AddNotification("Click on a destination point!");
            _notificationCount++;
        }
        
        if (_positionsAssigned == 0 && Input.GetMouseButtonDown(0) && !_selectSystem.LockSelection) {
            _startPosition = pos;
            _positionsAssigned++;
            _instantiateTarget(pos);
        }
        else if (_positionsAssigned == 1 && Input.GetMouseButtonDown(0) && !_selectSystem.LockSelection) {
            UIManager.Instance.AddNotification("Click on a destination point!");
            _endPosition = pos;
            _positionsAssigned++;
            _instantiateTarget(pos);
            StartVehicle();
        }
    }

    private void StartVehicle() {
        _vehicleScript.AddTarget(_startPosition);
        _vehicleScript.AddTarget(_endPosition);
        _selectedVehicle.GetComponent<NavMeshAgent>().enabled = true;
        _vehicleScript.enabled = true;
        _vehicleScript.StartVehicle();
        _removeTargets();
        
        _changeState(BuildingState.None);
    }
}