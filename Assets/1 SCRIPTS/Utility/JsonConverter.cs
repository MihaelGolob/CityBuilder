using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class JsonConverter {
    public static string RoadMapToJson(Dictionary<(int,int), RoadTile> roadMap) {
        var dict = new Dictionary<(int, int), RoadTileJson>();
        
        foreach (var key in roadMap.Keys) {
            if (roadMap[key].Status == RoadStatus.Preview) continue;
            
            var rc = roadMap[key].RoadColor;
            var pc = roadMap[key].PavementColor;
            var rt = new RoadTileJson() {
                Type = roadMap[key].Type,
                Position = roadMap[key].Position,
                Orientation = roadMap[key].Orientation,
                RoadColor = (rc.r, rc.g, rc.b),
                PavementColor = (pc.r, pc.g, pc.b)
            };
            
            dict.Add(key, rt);
        }
        
        return JsonConvert.SerializeObject(dict, Formatting.Indented);
    }
    
    public static Dictionary<(int,int), RoadTile> JsonToRoadMap(string json) {
        var dict = JsonConvert.DeserializeObject<Dictionary<string, RoadTileJson>>(json);
        var roadMap = new Dictionary<(int, int), RoadTile>();
        
        foreach (var key in dict.Keys) {
            var t = dict[key];
            // create a proper RoadTile
            var rc = t.RoadColor;
            var pc = t.PavementColor;

            var roadColor = new Color(rc.r, rc.g, rc.b);
            var pavementColor = new Color(pc.r, pc.g, pc.b);

            var rt = new RoadTile(t.Type, t.Orientation, t.Position, roadColor, pavementColor);

            roadMap.Add(ToTuple(key), rt);
        }
        
        return roadMap;
    }
    
    private static (int,int) ToTuple(string key) {
        // remove the parentheses
        key = key.Substring(1, key.Length - 2);
        // split the items
        var split = key.Split(',');
        // parse each item to int and return a tuple
        return (int.Parse(split[0]), int.Parse(split[1]));
    }
}