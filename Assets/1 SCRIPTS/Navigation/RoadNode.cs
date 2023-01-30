using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode {
    private readonly (float x, float y) position;
    private readonly float margin = 2f;
    
    public (float x, float y) Position => position;
    
    public RoadNode((float x, float y) position) {
        this.position = position;
    }

    public Vector3 DriveThroughPoint(Orientation before, Orientation after) {
        // return a point on the road based on where the car is coming from and where it's going
        // this is a bit of a hack, but it works
        if (before == Orientation.Right && after == Orientation.Right)
            return new Vector3(position.x, 0, position.y - margin);
        if (before == Orientation.Right && after == Orientation.Up)
            return new Vector3(position.x, 0, position.y - margin);
        if (before == Orientation.Right && after == Orientation.Down)
            return new Vector3(position.x - margin, 0, position.y - margin);
        
        if (before == Orientation.Left && after == Orientation.Left)
            return new Vector3(position.x, 0, position.y + margin);
        if (before == Orientation.Left && after == Orientation.Up)
            return new Vector3(position.x + margin, 0, position.y + margin);
        if (before == Orientation.Left && after == Orientation.Down)
            return new Vector3(position.x, 0, position.y + margin);
        
        if (before == Orientation.Up && after == Orientation.Up)
            return new Vector3(position.x + margin, 0, position.y);
        if (before == Orientation.Up && after == Orientation.Right)
            return new Vector3(position.x + margin, 0, position.y - margin);
        if (before == Orientation.Up && after == Orientation.Left)
            return new Vector3(position.x + margin, 0, position.y);
        
        if (before == Orientation.Down && after == Orientation.Down)
            return new Vector3(position.x - margin, 0, position.y);
        if (before == Orientation.Down && after == Orientation.Right)
            return new Vector3(position.x - margin, 0, position.y);
        if (before == Orientation.Down && after == Orientation.Left)
            return new Vector3(position.x - margin, 0, position.y + margin);
        
        if (before == Orientation.None && after == Orientation.Right)
            return new Vector3(position.x, 0, position.y - margin);
        if (before == Orientation.None && after == Orientation.Left)
            return new Vector3(position.x, 0, position.y + margin);
        if (before == Orientation.None && after == Orientation.Up)
            return new Vector3(position.x + margin, 0, position.y);
        if (before == Orientation.None && after == Orientation.Down)
            return new Vector3(position.x - margin, 0, position.y);
        
        if (before == Orientation.Right && after == Orientation.None)
            return new Vector3(position.x, 0, position.y - margin);
        if (before == Orientation.Left && after == Orientation.None)
            return new Vector3(position.x, 0, position.y + margin);
        if (before == Orientation.Up && after == Orientation.None)
            return new Vector3(position.x + margin, 0, position.y);
        if (before == Orientation.Down && after == Orientation.None)
            return new Vector3(position.x - margin, 0, position.y);
       
        throw new Exception($"No drive through point found for: {before} -> {after}");
    } 

    public override bool Equals(object obj) {
        if (obj is not RoadNode node) {
            return false;
        }

        return Math.Abs(position.x - node.position.x) < 0.2f && Math.Abs(position.y - node.position.y) < 0.2f;
    }
    
    public override int GetHashCode() {
        return position.GetHashCode();
    }
}