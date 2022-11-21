using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    public static Vector3 GetWorldPos(int i, int j){
        Vector3 pos = new Vector3();
        pos.z = i * Config.CellSize;
        pos.x = j * Config.CellSize;
        return pos;
    }
}
