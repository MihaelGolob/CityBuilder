using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class NavigationSystem : MonoBehaviour {
    // singleton pattern
    private static NavigationSystem _instance;
    public static NavigationSystem Instance => _instance;
    
    // inspector assigned
    [SerializeField] private NavMeshSurface _roadNavMesh;
    
    // private variables
    private NavigationGraph _graph;

    #region UNITY methods
    
    private void Awake() {
        // setup singleton
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
        
        _graph = new NavigationGraph();
        _roadNavMesh.BuildNavMesh();
    }
    
    #endregion
    
    #region public methods
    
    public List<Vector3> GetShortestPath(Vector3 start, Vector3 end) {
        // get the closest road node to the start and end points
        var startNode = _graph.GetClosestNode((start.x, start.z));
        var endNode = _graph.GetClosestNode((end.x, end.z));
        
        // get the shortest path between the two nodes
        var path = _graph.FindPath(startNode, endNode);
        // convert to a list of vector3
        var result = path.Select(n => new Vector3(n.Position.x, 0f, n.Position.y)).ToList();

        return result;
    }

    public void AddRoadNode((int x, int y) mapPosition, (float x, float y) roadPosition) {
        var node = new RoadNode(roadPosition);
        _graph.AddNode(mapPosition, node);
    }

    public void RemoveRoadNode((int x, int y) position) {
        _graph.RemoveNode(position);
    }

    public void Clear() {
        _graph.Clear();
    }

    public void BakeNavMesh() => _roadNavMesh.BuildNavMesh();

    #endregion
}
