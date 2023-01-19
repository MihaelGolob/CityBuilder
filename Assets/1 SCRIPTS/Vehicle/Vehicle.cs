using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vehicle : MonoBehaviour {
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float visionDistance = 4f;

    // private variables
    // components
    private NavMeshAgent _navAgent;

    private List<Vector3> _path;
    private int _currentNode = 0;

    #region Unity_methods

    private IEnumerator Start() {
        _navAgent = GetComponent<NavMeshAgent>();

        // wait for the navigation system to setup
        yield return new WaitForSeconds(2f);

        _path = NavigationSystem.Instance.GetShortestPath(targets[0].position, targets[1].position);
        _navAgent.SetDestination(_path[_currentNode]);
    }

    private void Update() {
        MoveToNextNode();

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

    #region private methods

    private void MoveToNextNode() {
        if (_path == null || _path.Count == 0) return;
        if (!(_navAgent.remainingDistance < 1.5f)) return;

        _currentNode++;
        if (_currentNode >= _path.Count) {
            // swap start and finish 
            (targets[0], targets[1]) = (targets[1], targets[0]);

            _path = NavigationSystem.Instance.GetShortestPath(targets[0].position, targets[1].position);
            _currentNode = 0;
        }

        _navAgent.SetDestination(_path[_currentNode]);
    }

    private void DetectVehicles() {
        // slow down or stop the vehicle if too close to another vehicle
        // cast ray to check if there is another vehicle in front of this vehicle
        var ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * visionDistance);

        if (Physics.Raycast(ray, out var hit, visionDistance)) {
            if (hit.collider.CompareTag("Vehicle")) {
                // stop the vehicle
                _navAgent.isStopped = true;
                return;
            }
        }

        _navAgent.isStopped = false;
    }

    #endregion
}