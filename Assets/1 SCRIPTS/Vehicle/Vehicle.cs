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

        _currentNode++;
        if (_currentNode >= _path.Count) {
            var tmp = targets[0];
            targets[0] = targets[1];
            targets[1] = tmp;
            
            _path = NavigationSystem.Instance.GetShortestPath(targets[0].position, targets[1].position);
            _currentNode = 0;
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