using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class NavigationGraph {
    private readonly Dictionary<(int x, int y), RoadNode> _nodes = new();
    
    #region Public API
    
    public void AddNode((int x, int y) position, RoadNode node) {
        _nodes[position] = node;
    }
    
    public void RemoveNode((int x, int y) position) {
        _nodes.Remove(position);
    }

    public void Clear() {
        _nodes.Clear();
    }
    
    public (int x, int y) GetClosestNode((float x, float y) position) {
        var min = (minDist: float.MaxValue, x: 0, y: 0);
        // get the node with the shortest distance
        foreach (var key in _nodes.Keys) {
            var n = _nodes[key];
            var dx = n.Position.x - position.x;
            var dy = n.Position.y - position.y;
            var d = dx * dx + dy * dy;

            if (d < min.minDist) {
                min = (d, key.x, key.y);
            }
        }

        return (min.x, min.y);
    }
    
    #endregion
    
    #region Pathfinding

    public List<RoadNode> FindPath((int x, int y) start, (int x, int y) end, Orientation startingOrientation) {
        // use BFS to find the shortest path
        var queue = new Queue<(int x, int y)>();
        var visited = new HashSet<(int x, int y)>();
        var parents = new Dictionary<(int x, int y), (int x, int y)>();
        var first = true;
        
        queue.Enqueue(start);
        visited.Add(start);
        
        while (queue.Count != 0) {
            var current = queue.Dequeue();
            visited.Add(current);

            var neighbors = GetNeighborsKeys(current, first ? startingOrientation : Orientation.None);
            foreach (var n in neighbors.Where(n => !visited.Contains(n))) {
                queue.Enqueue(n);
                visited.Add(n);
                parents[n] = current;
            }
            
            first = false;
        }
        
        return ReconstructPath(parents, start, end);
    }
    
    private List<RoadNode> ReconstructPath(Dictionary<(int x, int y), (int x, int y)> parents, (int x, int y) start, (int x, int y) end) {
        var path = new List<RoadNode>();
        var current = end;

        while (current != start) {
            path.Insert(0, _nodes[current]);
            
            if (parents.ContainsKey(current)) current = parents[current];
            else break;
        }
        
        return path;
    }
    
    #endregion
    
    #region Private methods
    
    private List<RoadNode> GetNeighbors((int x, int y) position) {
        // TODO: the fact that each node does not have an adjacency list might be a bottleneck in the future
        var neighbours = new List<RoadNode>();
        if (_nodes.TryGetValue(GetValidNeighbourKey(position, Orientation.Left), out var left))
            neighbours.Add(left);
        if (_nodes.TryGetValue(GetValidNeighbourKey(position, Orientation.Right), out var right))
            neighbours.Add(right);
        if (_nodes.TryGetValue(GetValidNeighbourKey(position, Orientation.Down), out var down))
            neighbours.Add(down);
        if (_nodes.TryGetValue(GetValidNeighbourKey(position, Orientation.Up), out var up))
            neighbours.Add(up);
        
        return neighbours;
    }

    private List<(int x, int y)> GetNeighborsKeys((int x, int y) node, Orientation orientation) {
        var neighbours = new List<(int x, int y)>();

        if (orientation != Orientation.Right) {
            var left = GetValidNeighbourKey(node, Orientation.Left);
            if (_nodes.ContainsKey(left))
                neighbours.Add(left);
        }

        if (orientation != Orientation.Left) {
            var right = GetValidNeighbourKey(node, Orientation.Right);
            if (_nodes.ContainsKey(right))
                neighbours.Add(right);
        }

        if (orientation != Orientation.Up) {
            var down = GetValidNeighbourKey(node, Orientation.Down);
            if (_nodes.ContainsKey(down))
                neighbours.Add(down);
        }

        if (orientation != Orientation.Down) {
            var up = GetValidNeighbourKey(node, Orientation.Up);
            if (_nodes.ContainsKey(up))
                neighbours.Add(up);
        }
        
        return neighbours;
    }

    private (int x, int y) GetValidNeighbourKey((int x, int y) key, Orientation direction) {
        switch (direction) {
            case Orientation.Left: {
                var x = key.x - 1 == 0 ? -1 : key.x - 1;
                return (x, key.y);
            }
            case Orientation.Right: {
                var x = key.x + 1 == 0 ? 1 : key.x + 1;
                return (x, key.y);
            }
            case Orientation.Down: {
                var y = key.y - 1 == 0 ? -1 : key.y - 1;
                return (key.x, y);
            }
            case Orientation.Up: {
                var y = key.y + 1 == 0 ? 1 : key.y + 1;
                return (key.x, y);
            }
            default:
                return (0, 0);
        }
    }
    
    #endregion
}
