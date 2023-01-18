using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vehicle : MonoBehaviour {
    [SerializeField] private List<Transform> targets;
    
    // private variables
    // components
    private NavMeshAgent _navAgent;

    private List<Vector3> _path;
    private int _currentNode = 0;
    private bool _forward = true;
    
    #region Unity_methods

    private IEnumerator Start() {
        _navAgent = GetComponent<NavMeshAgent>();

        // wait for the navigation system to setup
        yield return new WaitForSeconds(2f);
        
        _path = NavigationSystem.Instance.GetShortestPath(targets[0].position, targets[1].position);
        _navAgent.SetDestination(_path[_currentNode]);
    }
    
    private void Update() {
        if (_path == null || _path.Count == 0) return;
        if (!(_navAgent.remainingDistance < 0.5f)) return;
        
        if ((_currentNode == _path.Count - 1 && _forward) || (_currentNode == 0 && !_forward)) {
            _path = NavigationSystem.Instance.GetShortestPath(targets[0].position, targets[1].position);
            
            if (_forward) {
                _currentNode = _path.Count - 1;
            } else {
                _currentNode = 0;
            }
            
            _forward = !_forward;
        }
        else {
            if (_forward)
                _currentNode++;
            else
                _currentNode--;
        }
        
        _navAgent.SetDestination(_path[_currentNode]);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (_path == null || _path.Count == 0) return;
        for (var i = 0; i < _path.Count - 1; i++) {
            Gizmos.DrawLine(_path[i], _path[i + 1]);
            Gizmos.DrawSphere(_path[i], 0.5f);
        }
    }

    #endregion
}