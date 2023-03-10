using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vehicle : MonoBehaviour {
    [SerializeField] private List<Vector3> targets = new();
    [SerializeField] private float visionDistance = 4f;
    [SerializeField] private bool autoStart;

    // private variables
    // components
    private NavMeshAgent _navAgent;

    private List<Vector3> _path;
    private int _currentNode;
    private bool _onPosition;

    #region Unity_methods

    private IEnumerator Start() {
        _navAgent = GetComponent<NavMeshAgent>();
        
        // UIManager.Instance.AddNotification("Vehicle Created!");
        if (!autoStart) yield break;
        // wait for the navigation system to setup
        yield return new WaitForSeconds(2f);

        _path = NavigationSystem.Instance.GetShortestPath(transform.position, targets[0], CalculateOrientation());
        if (_path.Count == 0) {
            _path = NavigationSystem.Instance.GetShortestPath(targets[0], targets[1], CalculateOrientation());
            _onPosition = true;
        }

        _navAgent.SetDestination(_path[_currentNode]);
    }

    private void Update() {
        if (_path?.Count <= 1) return;
        
        MoveToNextNode();
        RecalculatePathIfNotOnNavMesh();
    }

    private void FixedUpdate() {
        DetectVehicles();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (_path == null || _path.Count == 0) return;
        for (var i = 0; i < _path.Count - 1; i++) {
            Gizmos.DrawLine(_path[i], _path[i + 1]);
            Gizmos.DrawSphere(_path[i], 0.5f);
        }

        Gizmos.DrawSphere(_path[^1], 0.5f);
    }

    #endregion

    #region public methods

    public void StartVehicle() {
        if (targets.Count < 2) return;
        _navAgent = GetComponent<NavMeshAgent>();

        _path = NavigationSystem.Instance.GetShortestPath(transform.position, targets[0], CalculateOrientation());
        if (_path.Count <= 1) {
            // if we are already at target 0 go to target 1
            _path = NavigationSystem.Instance.GetShortestPath(targets[0], targets[1], CalculateOrientation());
            _onPosition = true;
        }
        _navAgent.SetDestination(_path[_currentNode]);
    }

    public void AddTarget(Vector3 target) {
        targets.Add(target);
    }

    #endregion

    #region private methods

    private void RecalculatePathIfNotOnNavMesh() {
        if (_path == null || _currentNode >= _path.Count) return;
        
        // check if next point is on navmesh
        var result = NavMesh.SamplePosition(_path[_currentNode], out _, 0.3f, NavMesh.AllAreas);
        // recalculate path if next point is not on navmesh
        if (!result) {
            if (_currentNode == _path.Count - 1) {
                (targets[0], targets[1]) = (targets[1], targets[0]);
                targets[0] = _path[_currentNode - 1];
                _path = NavigationSystem.Instance.GetShortestPath(targets[0], targets[1], CalculateOrientation());
            }
            else {
                _path = NavigationSystem.Instance.GetShortestPath(transform.position, targets[0], CalculateOrientation());
            }

            if (_path.Count == 0) return;

            _currentNode = 0;
            _navAgent.SetDestination(_path[_currentNode]);
        }
    }

    private void MoveToNextNode() {
        if (_path == null || _path.Count == 0) return;
        if (!(_navAgent.remainingDistance < 1.5f)) return;

        _currentNode++;
        if (_currentNode >= _path.Count) {
            if (!_onPosition) {
                _path = NavigationSystem.Instance.GetShortestPath(targets[0], targets[1], CalculateOrientation());
                _currentNode = 0;
                _onPosition = true;
                _navAgent.SetDestination(_path[_currentNode]);
                return;
            }

            // swap start and finish 
            (targets[0], targets[1]) = (targets[1], targets[0]);

            _path = NavigationSystem.Instance.GetShortestPath(targets[0], targets[1], CalculateOrientation());
            _currentNode = 0;
        }

        _navAgent.SetDestination(_path[_currentNode]);
    }

    private void DetectVehicles() {
        // slow down or stop the vehicle if too close to another vehicle
        // cast ray to check if there is another vehicle in front of this vehicle
        var ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * visionDistance);

        if (Physics.Raycast(ray, out var hit, visionDistance)) {
            if (hit.collider.CompareTag("Vehicle")) {
                // stop the vehicle
                _navAgent.isStopped = true;
                return;
            }
        }

        _navAgent.isStopped = false;
    }

    private Orientation CalculateOrientation() {
        // calculate the where is the vehicle facing
        var forward = transform.forward;
        var angle = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);

        return angle switch {
            > 45 and < 135 => Orientation.Right,
            > 135 or < -135 => Orientation.Down,
            > -135 and < -45 => Orientation.Left,
            _ => Orientation.Up
        };
    }

    #endregion
}