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
        var result = new List<Vector3>();
        for (var i = 0; i < path.Count; i++) {
            var before = i == 0 ? 0 : i - 1;
            var next = i == path.Count - 1 ? i : i + 1;
            var beforeDir = GetDirection(path[before], path[i]);
            var nextDir = GetDirection(path[i], path[next]);
            
            result.Add(path[i].DriveThroughPoint(beforeDir, nextDir));
        }

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
    
    #region private methods

    private Orientation GetDirection(RoadNode current, RoadNode next) {
        if (current.Position.x < next.Position.x)
            return Orientation.Right;
        if (current.Position.x > next.Position.x)
            return Orientation.Left;
        if (current.Position.y < next.Position.y)
            return Orientation.Up;
        if (current.Position.y > next.Position.y)
            return Orientation.Down;
        
        return Orientation.None;
    }
    
    #endregion
}
