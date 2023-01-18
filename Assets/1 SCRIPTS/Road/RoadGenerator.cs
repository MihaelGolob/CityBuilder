using System;
using UnityEngine;

public enum RoadType { Straight, Cross, T, End, Turn }

public static class RoadGenerator {
    // vertex settings
    public static int VertexResolution { get; set; } = 1;
    public static float TileSize { get; set; } = 1;

    // cosmetics
    public static Color RoadColor { get; set; }
    public static Color PavementColor { get; set; }

    public static float PavementHeight { get; set; } = 0.1f;
    public static float PavementWidth { get; set; } = 0.3f;

    #region public API
    
    public static void GenerateRoad(RoadType roadType, Vector3 position, Color roadColor, Color pavementColor, out Vector3[] vertices, out int[] triangles, out Color[] colors) {
        // set colors
        RoadColor = roadColor;
        PavementColor = pavementColor;
        
        // generate roads
        (Vector3[] vertices, int[] triangles, Color[] colors) result = roadType switch {
            RoadType.Straight => GenerateStraightRoad(position),
            RoadType.Cross => GenerateCrossRoad(position),
            RoadType.End => GenerateEndRoad(position),
            RoadType.Turn => GenerateTurnRoad(position),
            RoadType.T => GenerateTRoad(position),
            _ => (null, null, null)
        };

        // assign results
        vertices = result.vertices;
        triangles = result.triangles;
        colors = result.colors;
    }
    
    public static Vector3[] Rotate(Vector3[] vertices, Vector3 center, Quaternion rotation) {
        for (var i = 0; i < vertices.Length; i++) {
            vertices[i] = rotation * (vertices[i] - center) + center;
        }

        return vertices;
    }
    
    #endregion
    
    #region private methods

    private static (Vector3[] vertices, int[] triangles, Color[] colors) GenerateStraightRoad(Vector3 position) {
        // generate vertices
        var vertices = new Vector3[VertexResolution * 12];
        var colors = new Color[VertexResolution * 12];
        var triangles = new int[VertexResolution * 10 * 3];


        var vertexIndex = 0;
        for (var i = 0; i < VertexResolution + 1; i++) {
            var z = i * TileSize / VertexResolution;

            vertices[vertexIndex] = new Vector3(position.x, position.y + PavementHeight, position.z + z);
            vertices[vertexIndex + 1] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + z);
            vertices[vertexIndex + 2] = new Vector3(position.x + PavementWidth, position.y, position.z + z);
            vertices[vertexIndex + 3] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + z);
            vertices[vertexIndex + 4] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + z);
            vertices[vertexIndex + 5] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + z);

            colors[vertexIndex] = PavementColor;
            colors[vertexIndex + 1] = PavementColor;
            colors[vertexIndex + 2] = RoadColor;
            colors[vertexIndex + 3] = RoadColor;
            colors[vertexIndex + 4] = PavementColor;
            colors[vertexIndex + 5] = PavementColor;

            vertexIndex += 6;
        }

        // generate triangles
        var triangleIndex = 0;
        for (var i = 0; i < VertexResolution; i++) {
            triangles[triangleIndex] = i;
            triangles[triangleIndex + 1] = i + 6;
            triangles[triangleIndex + 2] = i + 1;

            triangles[triangleIndex + 3] = i + 1;
            triangles[triangleIndex + 4] = i + 6;
            triangles[triangleIndex + 5] = i + 7;

            triangles[triangleIndex + 6] = i + 1;
            triangles[triangleIndex + 7] = i + 7;
            triangles[triangleIndex + 8] = i + 2;

            triangles[triangleIndex + 9] = i + 2;
            triangles[triangleIndex + 10] = i + 7;
            triangles[triangleIndex + 11] = i + 8;

            triangles[triangleIndex + 12] = i + 2;
            triangles[triangleIndex + 13] = i + 8;
            triangles[triangleIndex + 14] = i + 3;

            triangles[triangleIndex + 15] = i + 3;
            triangles[triangleIndex + 16] = i + 8;
            triangles[triangleIndex + 17] = i + 9;

            triangles[triangleIndex + 18] = i + 3;
            triangles[triangleIndex + 19] = i + 9;
            triangles[triangleIndex + 20] = i + 4;

            triangles[triangleIndex + 21] = i + 4;
            triangles[triangleIndex + 22] = i + 9;
            triangles[triangleIndex + 23] = i + 10;

            triangles[triangleIndex + 24] = i + 4;
            triangles[triangleIndex + 25] = i + 10;
            triangles[triangleIndex + 26] = i + 5;

            triangles[triangleIndex + 27] = i + 5;
            triangles[triangleIndex + 28] = i + 10;
            triangles[triangleIndex + 29] = i + 11;

            triangleIndex += 30;
        }
        
        return (vertices, triangles, colors); 
    }

    private static (Vector3[] vertices, int[] triangles, Color[] colors) GenerateCrossRoad(Vector3 position) {
        var vertices = new Vector3[VertexResolution * 28];
        var colors = new Color[VertexResolution * 28];
        var triangles = new int[VertexResolution * 102];
        
        // create vertices
        var z = TileSize;
        vertices[0] = new Vector3(position.x, position.y, position.z + PavementWidth);
        vertices[1] = new Vector3(position.x, position.y, position.z + TileSize - PavementWidth);
        vertices[2] = new Vector3(position.x, position.y + PavementHeight, position.z + PavementWidth);
        vertices[3] = new Vector3(position.x, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        
        vertices[4] = new Vector3(position.x + PavementWidth, position.y, position.z);
        vertices[5] = new Vector3(position.x + PavementWidth, position.y, position.z + PavementWidth);
        vertices[6] = new Vector3(position.x + PavementWidth, position.y, position.z + TileSize - PavementWidth);
        vertices[7] = new Vector3(position.x + PavementWidth, position.y, position.z + TileSize);
        
        vertices[8] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z);
        vertices[9] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        vertices[10] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        vertices[11] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + TileSize);
        
        vertices[12] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z);
        vertices[13] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + PavementWidth);
        vertices[14] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + TileSize - PavementWidth);
        vertices[15] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + TileSize);
        
        vertices[16] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z);
        vertices[17] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        vertices[18] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        vertices[19] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + TileSize);
        
        vertices[20] = new Vector3(position.x + TileSize, position.y, position.z + PavementWidth);
        vertices[21] = new Vector3(position.x + TileSize, position.y, position.z + TileSize - PavementWidth);
        vertices[22] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + PavementWidth);
        vertices[23] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        
        vertices[24] = new Vector3(position.x, position.y + PavementHeight, position.z);
        vertices[25] = new Vector3(position.x, position.y + PavementHeight, position.z + TileSize);
        vertices[26] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z);
        vertices[27] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize);
        
        // set colors
        colors[0] = RoadColor;
        colors[1] = RoadColor;
        colors[2] = PavementColor;
        colors[3] = PavementColor;
        
        colors[4] = RoadColor;
        colors[5] = RoadColor;
        colors[6] = RoadColor;
        colors[7] = RoadColor;
        
        colors[8] = PavementColor;
        colors[9] = PavementColor;
        colors[10] = PavementColor;
        colors[11] = PavementColor;
        
        colors[12] = RoadColor;
        colors[13] = RoadColor;
        colors[14] = RoadColor;
        colors[15] = RoadColor;
        
        colors[16] = PavementColor;
        colors[17] = PavementColor;
        colors[18] = PavementColor;
        colors[19] = PavementColor;     
       
        colors[20] = RoadColor;
        colors[21] = RoadColor;
        colors[22] = PavementColor;
        colors[23] = PavementColor;
        
        colors[24] = PavementColor;
        colors[25] = PavementColor;
        colors[26] = PavementColor;
        colors[27] = PavementColor;
        
        // create triangles for road
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 5;
        
        triangles[3] = 5;
        triangles[4] = 1;
        triangles[5] = 6;
        
        triangles[6] = 12;
        triangles[7] = 4;
        triangles[8] = 13;
        
        triangles[9] = 13;
        triangles[10] = 4;
        triangles[11] = 5;
        
        triangles[12] = 13;
        triangles[13] = 5;
        triangles[14] = 14;
        
        triangles[15] = 14;
        triangles[16] = 5;
        triangles[17] = 6;
        
        triangles[18] = 14;
        triangles[19] = 6;
        triangles[20] = 15;
        
        triangles[21] = 15;
        triangles[22] = 6;
        triangles[23] = 7;
        
        triangles[24] = 20;
        triangles[25] = 13;
        triangles[26] = 21;
        
        triangles[27] = 21;
        triangles[28] = 13;
        triangles[29] = 14;
        
        // create triangles for curb
        triangles[30] = 5;
        triangles[31] = 9;
        triangles[32] = 0;
        triangles[33] = 0;
        triangles[34] = 9;
        triangles[35] = 2;
        
        triangles[36] = 1;
        triangles[37] = 3;
        triangles[38] = 6;
        triangles[39] = 6;
        triangles[40] = 3;
        triangles[41] = 10;
        
        triangles[42] = 4;
        triangles[43] = 8;
        triangles[44] = 5;
        triangles[45] = 5;
        triangles[46] = 8;
        triangles[47] = 9;
        
        triangles[48] = 6;
        triangles[49] = 10;
        triangles[50] = 7;
        triangles[51] = 7;
        triangles[52] = 10;
        triangles[53] = 11;
        
        triangles[54] = 13;
        triangles[55] = 17;
        triangles[56] = 12;
        triangles[57] = 12;
        triangles[58] = 17;
        triangles[59] = 16;
        
        triangles[60] = 15;
        triangles[61] = 19;
        triangles[62] = 14;
        triangles[63] = 14;
        triangles[64] = 19;
        triangles[65] = 18;
        
        triangles[66] = 20;
        triangles[67] = 22;
        triangles[68] = 13;
        triangles[69] = 13;
        triangles[70] = 22;
        triangles[71] = 17;

        triangles[72] = 14;
        triangles[73] = 18;
        triangles[74] = 21;
        triangles[75] = 21;
        triangles[76] = 18;
        triangles[77] = 23;
        
        // create triangles for pavement
        triangles[78] = 8;
        triangles[79] = 24;
        triangles[80] = 9;
        triangles[81] = 9;
        triangles[82] = 24;
        triangles[83] = 2;
        
        triangles[84] = 10;
        triangles[85] = 3;
        triangles[86] = 11;
        triangles[87] = 11;
        triangles[88] = 3;
        triangles[89] = 25;
        
        triangles[90] = 26;
        triangles[91] = 16;
        triangles[92] = 22;
        triangles[93] = 22;
        triangles[94] = 16;
        triangles[95] = 17;
        
        triangles[96] = 23;
        triangles[97] = 18;
        triangles[98] = 27;
        triangles[99] = 27;
        triangles[100] = 18;
        triangles[101] = 19;

        return (vertices, triangles, colors);
    }

    private static (Vector3[] vertices, int[] triangles, Color[] color) GenerateEndRoad(Vector3 position) {
        var vertices = new Vector3[12];
        var colors = new Color[12];
        var triangles = new int[42];
        
        // create vertices for road
        vertices[0] = new Vector3(position.x, position.y + PavementHeight, position.z);
        vertices[1] = new Vector3(position.x, position.y + PavementHeight, position.z + TileSize);
        
        vertices[2] = new Vector3(position.x + PavementWidth, position.y, position.z + PavementWidth);
        vertices[3] = new Vector3(position.x + PavementWidth, position.y, position.z + TileSize);
        vertices[4] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        vertices[5] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + TileSize);
        
        vertices[6] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + PavementWidth);
        vertices[7] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + TileSize);
        vertices[8] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        vertices[9] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + TileSize);
        
        vertices[10] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z);
        vertices[11] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize);
        
        // create colors for road
        colors[0] = PavementColor;
        colors[1] = PavementColor;
        
        colors[2] = RoadColor;
        colors[3] = RoadColor;
        colors[4] = PavementColor;
        colors[5] = PavementColor;
        
        colors[6] = RoadColor;
        colors[7] = RoadColor;
        colors[8] = PavementColor;
        colors[9] = PavementColor;
        
        colors[10] = PavementColor;
        colors[11] = PavementColor;
        
        // create triangles
        // pavement 
        triangles[0] = 4;
        triangles[1] = 0;
        triangles[2] = 5;
        triangles[3] = 5;
        triangles[4] = 0;
        triangles[5] = 1;
        
        triangles[6] = 0;
        triangles[7] = 4;
        triangles[8] = 10;
        triangles[9] = 10;
        triangles[10] = 4;
        triangles[11] = 8;
        
        triangles[12] = 10;
        triangles[13] = 8;
        triangles[14] = 11;
        triangles[15] = 11;
        triangles[16] = 8;
        triangles[17] = 9;
        
        // road
        triangles[18] = 6;
        triangles[19] = 2;
        triangles[20] = 7;
        triangles[21] = 7;
        triangles[22] = 2;
        triangles[23] = 3;
        
        // curb
        triangles[24] = 2;
        triangles[25] = 4;
        triangles[26] = 3;
        triangles[27] = 3;
        triangles[28] = 4;
        triangles[29] = 5;
        
        triangles[30] = 6;
        triangles[31] = 8;
        triangles[32] = 2;
        triangles[33] = 2;
        triangles[34] = 8;
        triangles[35] = 4;
        
        triangles[36] = 7;
        triangles[37] = 9;
        triangles[38] = 6;
        triangles[39] = 6;
        triangles[40] = 9;
        triangles[41] = 8;
        

        return (vertices, triangles, colors);
    }

    private static (Vector3[] vertices, int[] triangles, Color[] color) GenerateTurnRoad(Vector3 position) {
        var vertices = new Vector3[16];
        var colors = new Color[16];
        var triangles = new int[54];
        
        // create vertices for road
        vertices[0] = new Vector3(position.x, position.y + PavementHeight, position.z);
        vertices[1] = new Vector3(position.x, position.y + PavementHeight, position.z + TileSize);
        vertices[2] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z);
        vertices[3] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        vertices[4] = new Vector3(position.x + PavementWidth, position.y, position.z);
        vertices[5] = new Vector3(position.x + PavementWidth, position.y, position.z + TileSize - PavementWidth);
        
        vertices[6] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z);
        vertices[7] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + PavementWidth);
        vertices[8] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z);
        vertices[9] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        
        vertices[10] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z);
        vertices[11] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + PavementWidth);
        vertices[12] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        vertices[13] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize);
        
        vertices[14] = new Vector3(position.x + TileSize, position.y, position.z + PavementWidth);
        vertices[15] = new Vector3(position.x + TileSize, position.y, position.z + TileSize - PavementWidth);
        
        // create colors for road
        colors[0] = PavementColor;
        colors[1] = PavementColor;
        colors[2] = PavementColor;
        colors[3] = PavementColor;
        colors[4] = RoadColor;
        colors[5] = RoadColor;
        
        colors[6] = RoadColor;
        colors[7] = RoadColor;
        colors[8] = PavementColor;
        colors[9] = PavementColor;
        
        colors[10] = PavementColor;
        colors[11] = PavementColor;
        colors[12] = PavementColor;
        colors[13] = PavementColor;
        
        colors[14] = RoadColor;
        colors[15] = RoadColor;
        
        // create triangles
        // pavement
        triangles[0] = 2;
        triangles[1] = 0;
        triangles[2] = 3;
        triangles[3] = 3;
        triangles[4] = 0;
        triangles[5] = 1;
        
        triangles[6] = 3;
        triangles[7] = 1;
        triangles[8] = 12;
        triangles[9] = 12;
        triangles[10] = 1;
        triangles[11] = 13;
        
        triangles[12] = 10;
        triangles[13] = 8;
        triangles[14] = 11;
        triangles[15] = 11;
        triangles[16] = 8;
        triangles[17] = 9;
        
        // curb
        triangles[18] = 4;
        triangles[19] = 2;
        triangles[20] = 5;
        triangles[21] = 5;
        triangles[22] = 2;
        triangles[23] = 3;
        
        triangles[24] = 7;
        triangles[25] = 9;
        triangles[26] = 6;
        triangles[27] = 6;
        triangles[28] = 9;
        triangles[29] = 8;
        
        triangles[30] = 14;
        triangles[31] = 11;
        triangles[32] = 7;
        triangles[33] = 7;
        triangles[34] = 11;
        triangles[35] = 9;
        
        triangles[36] = 5;
        triangles[37] = 3;
        triangles[38] = 15;
        triangles[39] = 15;
        triangles[40] = 3;
        triangles[41] = 12;
        
        // road
        triangles[42] = 6;
        triangles[43] = 4;
        triangles[44] = 7;
        triangles[45] = 7;
        triangles[46] = 4;
        triangles[47] = 5;
        
        triangles[48] = 7;
        triangles[49] = 5;
        triangles[50] = 14;
        triangles[51] = 14;
        triangles[52] = 5;
        triangles[53] = 15;

        return (vertices, triangles, colors);
    }

    private static (Vector3[] verticse, int[] triangles, Color[] color) GenerateTRoad(Vector3 position) {
        var vertices = new Vector3[20];
        var colors = new Color[20];
        var triangles = new int[60];

        // create vertices
        vertices[0] = new Vector3(position.x, position.y + PavementHeight, position.z);
        vertices[1] = new Vector3(position.x, position.y + PavementHeight, position.z + PavementWidth);
        vertices[2] = new Vector3(position.x, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        vertices[3] = new Vector3(position.x, position.y + PavementHeight, position.z + TileSize);
        
        vertices[4] = new Vector3(position.x, position.y, position.z + PavementWidth);
        vertices[5] = new Vector3(position.x, position.y, position.z + TileSize - PavementWidth);
        
        vertices[6] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z);
        vertices[7] = new Vector3(position.x + PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        
        vertices[8] = new Vector3(position.x + PavementWidth, position.y, position.z);
        vertices[9] = new Vector3(position.x + PavementWidth, position.y, position.z + PavementWidth);
        
        vertices[10] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z);
        vertices[11] = new Vector3(position.x + TileSize - PavementWidth, position.y + PavementHeight, position.z + PavementWidth);
        
        vertices[12] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z);
        vertices[13] = new Vector3(position.x + TileSize - PavementWidth, position.y, position.z + PavementWidth);
        
        vertices[14] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z);
        vertices[15] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + PavementWidth);
        vertices[16] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize - PavementWidth);
        vertices[17] = new Vector3(position.x + TileSize, position.y + PavementHeight, position.z + TileSize);
        
        vertices[18] = new Vector3(position.x + TileSize, position.y, position.z + PavementWidth);
        vertices[19] = new Vector3(position.x + TileSize, position.y, position.z + TileSize - PavementWidth);
        
        // create colors
        colors[0] = PavementColor;
        colors[1] = PavementColor;
        colors[2] = PavementColor;
        colors[3] = PavementColor;
        
        colors[4] = RoadColor;
        colors[5] = RoadColor;
        
        colors[6] = PavementColor;
        colors[7] = PavementColor;
        
        colors[8] = RoadColor;
        colors[9] = RoadColor;
        
        colors[10] = PavementColor;
        colors[11] = PavementColor;
        
        colors[12] = RoadColor;
        colors[13] = RoadColor;
        
        colors[14] = PavementColor;
        colors[15] = PavementColor;
        colors[16] = PavementColor;
        colors[17] = PavementColor;
        
        colors[18] = RoadColor;
        colors[19] = RoadColor;
        
        // create triangles
        // pavement
        triangles[0] = 6;
        triangles[1] = 0;
        triangles[2] = 7;
        triangles[3] = 7;
        triangles[4] = 0;
        triangles[5] = 1;
        
        triangles[6] = 14;
        triangles[7] = 10;
        triangles[8] = 15;
        triangles[9] = 15;
        triangles[10] = 10;
        triangles[11] = 11;
        
        triangles[12] = 2;
        triangles[13] = 3;
        triangles[14] = 16;
        triangles[15] = 16;
        triangles[16] = 3;
        triangles[17] = 17;
        
        // curb
        triangles[18] = 9;
        triangles[19] = 7;
        triangles[20] = 4;
        triangles[21] = 4;
        triangles[22] = 7;
        triangles[23] = 1;
        
        triangles[24] = 8;
        triangles[25] = 6;
        triangles[26] = 9;
        triangles[27] = 9;
        triangles[28] = 6;
        triangles[29] = 7;
        
        triangles[30] = 13;
        triangles[31] = 11;
        triangles[32] = 12;
        triangles[33] = 12;
        triangles[34] = 11;
        triangles[35] = 10;
        
        triangles[36] = 5;
        triangles[37] = 2;
        triangles[38] = 19;
        triangles[39] = 19;
        triangles[40] = 2;
        triangles[41] = 16;
        
        triangles[42] = 18;
        triangles[43] = 15;
        triangles[44] = 13;
        triangles[45] = 13;
        triangles[46] = 15;
        triangles[47] = 11;
        
        // road
        triangles[48] = 4;
        triangles[49] = 5;
        triangles[50] = 18;
        triangles[51] = 18;
        triangles[52] = 5;
        triangles[53] = 19;
        
        triangles[54] = 12;
        triangles[55] = 8;
        triangles[56] = 13;
        triangles[57] = 13;
        triangles[58] = 8;
        triangles[59] = 9;

        return (vertices, triangles, colors);
    }
    
    #endregion
}