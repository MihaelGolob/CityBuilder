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

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private int _positionsAssigned;

    public VehiclePlacement(SelectSystem selectSystem, Action<BuildingState> changeState, List<GameObject> vehicles) {
        if (vehicles.Count == 0) throw new Exception("Not enough targets!");
        
        _selectSystem = selectSystem;
        _changeState = changeState;
        
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
        if (_positionsAssigned == 0 && Input.GetMouseButtonDown(0) && !_selectSystem.LockSelection) {
            _startPosition = pos;
            _positionsAssigned++;
        }
        else if (_positionsAssigned == 1 && Input.GetMouseButtonDown(0) && !_selectSystem.LockSelection) {
            _endPosition = pos;
            _positionsAssigned++;
            StartVehicle();
        }
    }

    private void StartVehicle() {
        _vehicleScript.AddTarget(_startPosition);
        _vehicleScript.AddTarget(_endPosition);
        _selectedVehicle.GetComponent<NavMeshAgent>().enabled = true;
        _vehicleScript.enabled = true;
        _vehicleScript.StartVehicle();
        
        _changeState(BuildingState.None);
    }
}