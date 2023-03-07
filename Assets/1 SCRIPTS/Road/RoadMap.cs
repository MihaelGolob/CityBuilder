using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class RoadMap {
    private readonly Dictionary<(int, int), RoadTile> _roadMap;

    public Color RoadColor { get; set; }
    public Color PavementColor { get; set; }
    public Color RoadPreviewColor { get; set; }
    public Color PavementPreviewColor { get; set; }

    private (int? x, int? y) _highlightedTile = (null, null);
    
    // public getters
    public Dictionary<(int, int), RoadTile> RoadMapDictionary => _roadMap;

    public RoadMap(Dictionary<(int, int), RoadTile> map) {
        _roadMap = map ?? new Dictionary<(int, int), RoadTile>();
        foreach (var tile in _roadMap.Keys) {
            var roadCenterPos = (_roadMap[tile].Position.x + RoadGenerator.TileSize/2, _roadMap[tile].Position.y + RoadGenerator.TileSize/2);
            NavigationSystem.Instance.AddRoadNode(tile, roadCenterPos);
        }
    }

    #region Public API

    public void AddRoad(float x, float y, bool isPreviewRoad = false) {
        var mapCoord = ToMapCoordinates(x, y);
        if (_roadMap.ContainsKey(mapCoord)) {
            // road already exists create a non preview road instead
            var newTile = new RoadTile(_roadMap[mapCoord].Type, _roadMap[mapCoord].Orientation, _roadMap[mapCoord].Position, RoadColor, PavementColor);
            AddToMap(mapCoord, newTile, false);
            return;
        }

        // create new road
        var type = GetRoadType(mapCoord.x, mapCoord.y);
        var realPos = (x - RoadGenerator.TileSize / 2, y - RoadGenerator.TileSize / 2);

        var rc = isPreviewRoad ? RoadPreviewColor : RoadColor;
        var pc = isPreviewRoad ? PavementPreviewColor : PavementColor;

        var tile = new RoadTile(type.type, type.orientation, realPos, rc, pc) {
            Status = isPreviewRoad ? RoadStatus.Preview : RoadStatus.Normal
        };

        // add road to map
        AddToMap(mapCoord, tile, isPreviewRoad);

        // Update neighbours
        var neighbours = GetAdjacent(mapCoord.x, mapCoord.y);
        foreach (var n in neighbours) {
            _roadMap.TryGetValue((n.x, n.y), out var t);

            var nt = GetRoadType(n.x, n.y);
            var rcolor = t.Status == RoadStatus.Preview ? RoadPreviewColor : RoadColor;
            var pcolor = t.Status == RoadStatus.Preview ? PavementPreviewColor : PavementColor;
            
            _roadMap[(n.x, n.y)] = new RoadTile(nt.type, nt.orientation, (t.Position.x, t.Position.y), rcolor, pcolor);
        }
    }

    public void RemoveRoad(float x, float y, RoadStatus status = RoadStatus.Normal) {
        var mapCoord = ToMapCoordinates(x, y);
        _roadMap.TryGetValue(mapCoord, out var tile);

        if (tile == null) return;

        if (tile.Status == status) {
            // remove road
            RemoveFromMap(mapCoord);
        }

        if (tile.HasTrafficLights) {
            // destroy traffic lights
            Object.Destroy(tile.TrafficLight);
        }

        // remove highlighted road
        _highlightedTile = (null, null);

        // Update neighbours
        var neighbours = GetAdjacent(mapCoord.x, mapCoord.y);
        foreach (var n in neighbours) {
            _roadMap.TryGetValue((n.x, n.y), out var t);

            var nt = GetRoadType(n.x, n.y);
            _roadMap[(n.x, n.y)] = new RoadTile(nt.type, nt.orientation, (t.Position.x, t.Position.y), RoadColor, PavementColor, hasTrafficLights: t.HasTrafficLights, trafficLight: t.TrafficLight);
        }
    }

    public void RemoveAllRoads() {
        _roadMap.Clear();
        NavigationSystem.Instance.Clear();
    }

    public void HighlightRoad(float x, float y, Color roadColor, Color pavementColor) {
        var mapCoord = ToMapCoordinates(x, y);
        if (mapCoord == _highlightedTile) return;

        // TODO: implement this with changing the colors of existing roads
        // TODO: instead of creating new ones
        // change previously highlighted tile to normal
        if (_highlightedTile.x != null && _highlightedTile.y != null) {
            var tile = _roadMap[((int, int))_highlightedTile];
            _roadMap[((int, int))_highlightedTile] = new RoadTile(tile, RoadColor, PavementColor);
        }

        // change new highlighted tile to highlighted
        if (_roadMap.ContainsKey(mapCoord)) {
            var tile = _roadMap[mapCoord];
            _roadMap[mapCoord] = new RoadTile(tile, roadColor, pavementColor);
            _highlightedTile = mapCoord;
        }
        else {
            _highlightedTile = (null, null);
        }
    }

    public IEnumerable<RoadTile> GetAllTiles() {
        return _roadMap.Values.ToList();
    }

    public void CleanUp() {
        // remove preview roads and change highlighted tile to normal
        var previewRoads = _roadMap.Where(x => x.Value.Status == RoadStatus.Preview).ToList();
        foreach (var road in previewRoads) {
            RemoveFromMap(road.Key);
        }
        
        if (_highlightedTile.x != null && _highlightedTile.y != null) {
            var tile = _roadMap[((int, int))_highlightedTile];
            _roadMap[((int, int))_highlightedTile] = new RoadTile(tile, RoadColor, PavementColor);
        }
        _highlightedTile = (null, null);
    }

    public bool RoadExists(float x, float y) {
        var mapCoord = ToMapCoordinates(x, y);
        _roadMap.TryGetValue(mapCoord, out var tile);
        
        return tile?.Status == RoadStatus.Normal;
    }
    
    public int CountNeighbors(float x, float y) {
        var mapCoord = ToMapCoordinates(x, y);
        var neighbours = GetAdjacent(mapCoord.x, mapCoord.y);
        
        return neighbours.Count;
    }
    
    public bool TrafficLightExists(float x, float y) {
        var mapCoord = ToMapCoordinates(x, y);
        _roadMap.TryGetValue(mapCoord, out var tile);
        return tile?.HasTrafficLights ?? true;
    }
    
    public void PlaceTrafficLight(float x, float y, GameObject gameObject) {
        var mapCoord = ToMapCoordinates(x, y);
        _roadMap.TryGetValue(mapCoord, out var tile);
        if (tile == null) return;
        
        tile.HasTrafficLights = true;
        tile.TrafficLight = gameObject;
    }
    
    public Orientation GetTileOrientation(float x, float z) {
        var mapCoord = ToMapCoordinates(x, z);
        _roadMap.TryGetValue(mapCoord, out var tile);
        
        return tile?.Orientation ?? Orientation.None;
    }
    
    #endregion

    #region private methods
    
    private void AddToMap((int x, int y) position, RoadTile roadTile, bool isPreview) {
        var roadCenterPos = (roadTile.Position.x + RoadGenerator.TileSize/2, roadTile.Position.y + RoadGenerator.TileSize/2);
        _roadMap[position] = roadTile;
        if (!isPreview)
            NavigationSystem.Instance.AddRoadNode(position, roadCenterPos);
    }

    private void RemoveFromMap((int x, int y) position) {
        _roadMap.Remove(position);
        NavigationSystem.Instance.RemoveRoadNode(position);
    }
    
    private static (int x, int y) ToMapCoordinates(float x, float y) {
        var mapX = Mathf.CeilToInt(Mathf.Abs(x) / RoadGenerator.TileSize);
        var mapY = Mathf.CeilToInt(Mathf.Abs(y) / RoadGenerator.TileSize);
        return ((int)Mathf.Sign(x) * mapX, (int)Mathf.Sign(y) * mapY);
    }

    private (RoadType type, Orientation orientation) GetRoadType(int x, int y) {
        var north = _roadMap.ContainsKey((x, y + 1 == 0 ? 1 : y + 1));
        var south = _roadMap.ContainsKey((x, y - 1 == 0 ? -1 : y - 1));
        var east = _roadMap.ContainsKey((x + 1 == 0 ? 1 : x + 1, y));
        var west = _roadMap.ContainsKey((x - 1 == 0 ? -1 : x - 1, y));

        // cross
        if (north && south && east && west) {
            return (RoadType.Cross, Orientation.Up);
        }

        // T
        if (north && east && west)
            return (RoadType.T, Orientation.Down);
        if (north && east && south)
            return (RoadType.T, Orientation.Left);
        if (east && south && west)
            return (RoadType.T, Orientation.Up);
        if (south && west && north)
            return (RoadType.T, Orientation.Right);

        // turns
        if (south && east)
            return (RoadType.Turn, Orientation.Up);
        if (south && west)
            return (RoadType.Turn, Orientation.Right);
        if (north && east)
            return (RoadType.Turn, Orientation.Left);
        if (north && west)
            return (RoadType.Turn, Orientation.Down);

        // straight
        if (north && south)
            return (RoadType.Straight, Orientation.Up);

        if (west && east)
            return (RoadType.Straight, Orientation.Right);

        // end
        if (north)
            return (RoadType.End, Orientation.Up);
        if (south)
            return (RoadType.End, Orientation.Down);
        if (east)
            return (RoadType.End, Orientation.Right);
        if (west)
            return (RoadType.End, Orientation.Left);

        return (RoadType.End, Orientation.Up);
    }

    private List<(int x, int y)> GetAdjacent(int x, int y) {
        List<(int x, int y)> adjacent = new();
        if (_roadMap.ContainsKey((x + 1 == 0 ? 1 : x + 1, y)))
            adjacent.Add((x + 1 == 0 ? 1 : x + 1, y));

        if (_roadMap.ContainsKey((x - 1 == 0 ? -1 : x - 1, y)))
            adjacent.Add((x - 1 == 0 ? -1 : x - 1, y));

        if (_roadMap.ContainsKey((x, y + 1 == 0 ? 1 : y + 1)))
            adjacent.Add((x, y + 1 == 0 ? 1 : y + 1));

        if (_roadMap.ContainsKey((x, y - 1 == 0 ? -1 : y - 1)))
            adjacent.Add((x, y - 1 == 0 ? -1 : y - 1));

        return adjacent;
    }
    
    #endregion
}