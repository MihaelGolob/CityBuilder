using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Orientation { Up, Down, Left, Right, None }

public enum RoadStatus { Normal, Destroy, Preview }

public class RoadTile {
    public RoadType Type { get; private set; }
    public RoadStatus Status { get; set; } = RoadStatus.Normal;
    public (float x, float y) Position { get; private set; }
    public bool HasTrafficLights { get; set; }
    public GameObject TrafficLight { get; set; }

    private Color _roadColor;
    private Color _pavementColor;
    private Orientation _orientation;

    // public getters
    public int VertexCount => _vertices.Length;
    public int TriangleCount => _triangles.Length;

    public Orientation Orientation => _orientation;
    public Color RoadColor => _roadColor;
    public Color PavementColor => _pavementColor;

    public Vector3[] Vertices => _vertices;
    public int[] Triangles => _triangles;
    public Color[] Colors => _colors;

    // private variables
    private Vector3[] _vertices;
    private int[] _triangles;
    private Color[] _colors;

    private RoadNode _navigationNode;

    public RoadTile(RoadType type, Orientation orientation, (float x, float y) position, Color roadColor, Color pavementColor, RoadStatus status = RoadStatus.Normal, bool hasTrafficLights = false,
        GameObject trafficLight = null) {
        Type = type;
        Position = position;
        _roadColor = roadColor;
        _pavementColor = pavementColor;
        Status = status;
        HasTrafficLights = hasTrafficLights;
        TrafficLight = trafficLight;

        RoadGenerator.GenerateRoad(type, new Vector3(position.x, 0.0f, position.y), _roadColor, _pavementColor, out _vertices, out _triangles, out _colors);
        Rotate(orientation);
    }

    public RoadTile(RoadTile tile, Color roadColor, Color pavementColor, RoadStatus status = RoadStatus.Normal) {
        Type = tile.Type;
        Position = tile.Position;
        _roadColor = roadColor;
        _pavementColor = pavementColor;
        Status = status;
        HasTrafficLights = tile.HasTrafficLights;
        TrafficLight = tile.TrafficLight;

        RoadGenerator.GenerateRoad(Type, new Vector3(Position.x, 0.0f, Position.y), _roadColor, _pavementColor, out _vertices, out _triangles, out _colors);
        Rotate(tile.Orientation);
    }

    public void Rotate(Orientation orientation) {
        var center = new Vector3(Position.x + RoadGenerator.TileSize / 2, 0.0f, Position.y + RoadGenerator.TileSize / 2);
        _orientation = orientation;

        var rot = orientation switch {
            Orientation.Up => Quaternion.identity,
            Orientation.Left => Quaternion.Euler(0.0f, -90.0f, 0.0f),
            Orientation.Right => Quaternion.Euler(0.0f, 90.0f, 0.0f),
            Orientation.Down => Quaternion.Euler(0.0f, 180.0f, 0.0f),
            _ => Quaternion.identity
        };

        _vertices = RoadGenerator.Rotate(_vertices, center, rot);
    }
}