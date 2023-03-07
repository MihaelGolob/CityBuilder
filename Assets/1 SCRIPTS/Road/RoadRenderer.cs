using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RoadRenderer : MonoBehaviour {
    // TODO: implement this properly and make it serialized
    private int _resolution = 1;
    [SerializeField] private float tileSize = 3;
    
    [Header("Road Settings")]
    [SerializeField] private float pavementWidth = 0.3f;
    [SerializeField] private float pavementHeight = 0.2f;

    [Header("Colors")] [SerializeField] private Color roadColor = Color.black;
    [SerializeField] private Color pavementColor = Color.gray;
    [SerializeField] private Color roadPreviewColor;
    [SerializeField] private Color pavementPreviewColor;
    [SerializeField] private Color roadDestroyColor;
    [SerializeField] private Color pavementDestroyColor;
    [SerializeField] private Color lightRoadColor;
    [SerializeField] private Color lightPavementColor;

    // private 
    private MeshFilter _meshFilter;
    private RoadEffects _roadEffects;

    private RoadMap _roadMap;
    private HashSet<Vector3> _previewPositions;

    #region Unity Methods

    private void Start() {
        _meshFilter = GetComponent<MeshFilter>();
        _roadEffects = GetComponent<RoadEffects>();

        // setup road generator
        RoadGenerator.VertexResolution = _resolution;
        RoadGenerator.TileSize = tileSize;
        RoadGenerator.PavementWidth = pavementWidth;
        RoadGenerator.PavementHeight = pavementHeight;

        _previewPositions = new HashSet<Vector3>();

        // import saved road map
        var import = ImportData();

        _roadMap = new RoadMap(import) {
            RoadColor = roadColor,
            PavementColor = pavementColor,
            RoadPreviewColor = roadPreviewColor,
            PavementPreviewColor = pavementPreviewColor
        };
        
        // bake nav mesh
        NavigationSystem.Instance.BakeNavMesh();
    }

    #endregion

    #region private methods

    private void UpdateMesh() {
        var data = GetMeshData();

        var mesh = new Mesh() {
            vertices = data.vertices,
            triangles = data.triangles,
            colors = data.colors
        };

        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;
    }

    private (Vector3[] vertices, int[] triangles, Color[] colors) GetMeshData() {
        var vertexCount = 0;
        var triangleCount = 0;

        var tiles = _roadMap.GetAllTiles();
        foreach (var tile in tiles) {
            vertexCount += tile.VertexCount;
            triangleCount += tile.TriangleCount;
        }

        var vertices = new Vector3[vertexCount];
        var triangles = new int[triangleCount];
        var colors = new Color[vertexCount];

        var vertexIndex = 0;
        var triangleIndex = 0;
        foreach (var tile in tiles) {
            // copy vertices and colors
            for (var i = 0; i < tile.VertexCount; i++) {
                vertices[vertexIndex] = tile.Vertices[i];
                colors[vertexIndex] = tile.Colors[i];
                vertexIndex++;
            }

            // copy triangles
            for (var i = 0; i < tile.TriangleCount; i++) {
                triangles[triangleIndex] = tile.Triangles[i] + vertexIndex - tile.VertexCount;
                triangleIndex++;
            }
        }

        return (vertices, triangles, colors);
    }

    private Dictionary<(int, int), RoadTile> ImportData() {
        #if UNITY_EDITOR
        // import road map from json file
        var json = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/MeshData/GeneratedRoadMap.json");
        if (json == null) {
            Debug.LogWarning("No road map data found!");
        }
        else {
            return JsonConverter.JsonToRoadMap(json.text);
        }
        #endif

        return null;
    }
    
    private void HighlightRoad(Vector3 pos, Color roadColor, Color pavementColor) {
        _roadMap.HighlightRoad(pos.x, pos.z, roadColor, pavementColor);
        UpdateMesh();
    }

    #endregion

    #region public methods

    public void AddRoads() {
        // change preview roads to actual roads
        foreach (var position in _previewPositions) {
            _roadMap.AddRoad(position.x, position.z);
            _roadEffects.PlayRoadBuildParticleEffect(position);
        }
        _roadEffects.PlayRoadBuildAudioEffect(_previewPositions.Count <= 1 ? 0 : 1);
        _previewPositions.Clear();
        
        UpdateMesh();
        NavigationSystem.Instance.BakeNavMesh();
    }

    public void AddPreviewRoad(Vector3 pos, bool removeOld = true) {
        if (_previewPositions.Contains(pos)) return;

        // remove old preview
        if (removeOld) {
            foreach (var previewPos in _previewPositions) {
                _roadMap.RemoveRoad(previewPos.x, previewPos.z, RoadStatus.Preview);
            }
            _previewPositions.Clear();
        }

        var x = pos.x;
        var y = pos.z;
        _previewPositions.Add(pos);

        _roadMap.AddRoad(x, y, true);
        UpdateMesh();
    }

    public void RemoveRoad(Vector3 pos) {
        _roadMap.RemoveRoad(pos.x, pos.z);
        _roadEffects.RoadDestroyEffects(pos);
        
        UpdateMesh();
        NavigationSystem.Instance.BakeNavMesh();
    }

    public void RemoveAllRoads() {
        _roadMap.RemoveAllRoads();
        UpdateMesh();
        NavigationSystem.Instance.BakeNavMesh();
    }

    public void RemovePreviewRoad() {
        foreach (var previewPos in _previewPositions) {
            _roadMap.RemoveRoad(previewPos.x, previewPos.z, RoadStatus.Preview);
        }
        _previewPositions.Clear();

        UpdateMesh();
    }

    public void CleanUp() {
        _roadMap.CleanUp();
        UpdateMesh();
    }

    public void HighlightRoadToDestroy(Vector3 pos) {
        HighlightRoad(pos, roadDestroyColor, pavementDestroyColor);
    }

    public void HighlightRoadToPlaceTrafficLight(Vector3 pos) {
        HighlightRoad(pos, lightRoadColor, lightPavementColor);
    }
    
    public bool RoadExists(Vector3 pos) {
        return _roadMap.RoadExists(pos.x, pos.z);
    }
    
    public int CountNeighbours(Vector3 pos) => _roadMap.CountNeighbors(pos.x, pos.z);
    
    public bool TrafficLightExists(Vector3 pos) => _roadMap.TrafficLightExists(pos.x, pos.z);
    
    public void PlaceTrafficLigth(Vector3 gridPos, GameObject o) {
        _roadMap.PlaceTrafficLight(gridPos.x, gridPos.z, o);
    }
        
    public Orientation GetOrientation(Vector3 pos) => _roadMap.GetTileOrientation(pos.x, pos.z);

    public void SaveMesh() {
        // save roadmap as json
        var json = JsonConverter.RoadMapToJson(_roadMap.RoadMapDictionary);
        var path = "Assets/MeshData/GeneratedRoadMap.json";
        // remove old file
        System.IO.File.Delete(path);
        // write to new file
        System.IO.File.WriteAllText(path, json);
        #if UNITY_EDITOR
        // also save mesh as asset
        var mesh = _meshFilter.mesh;
        AssetDatabase.CreateAsset(mesh, "Assets/MeshData/GeneratedRoadMesh.asset");
        AssetDatabase.SaveAssets();
        #endif
    }

    #endregion
}