using System;
using System.Collections;
using System.Collections.Generic;

public class RoadNode {
    private readonly (float x, float y) position;
    
    public (float x, float y) Position => position;
    
    public RoadNode((float x, float y) position) {
        this.position = position;
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