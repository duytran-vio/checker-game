using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    private static Transform[,] s_floors;
    private static Transform[,] s_checkers;

    public static Transform[,] MapState => s_checkers;

    public static void InitGrid()
    {
        SpawnManager.InitTable(out s_floors, out s_checkers);
    }

    public static Vector3 GetWorldPos(int i, int j)
    {
        Vector3 pos = new Vector3();
        pos.z = i * Config.CellSize;
        pos.x = j * Config.CellSize;
        return pos;
    }

    private static bool isValid(int x, int y)
    {
        return x >= 0 && x < Config.TableSize && y >= 0 && y < Config.TableSize;
    }

    private static int isCheckerOnCell(int x, int y)
    {
        if (!isValid(x, y)) return -1;
        return (s_checkers[x, y] != null) ? 1 : 0;
    }

    public static List<Transform> GetMoveableFloor(int x, int y, PlayerType playerType, bool isQueen)
    {
        List<Transform> moveableFloors = new List<Transform>();
        int k = (playerType == PlayerType.PLAYER) ? 1 : -1;
        for (int i = -1; i <= 1; i++)
        {
            if (i == 0 || !isValid(x + k, y + i) || isCheckerOnCell(x + k, y + i) == 1) continue;
            moveableFloors.Add(s_floors[x + k, y + i]);
        }
        return moveableFloors;
    }

    //public static int[,] GetMapState()
    //{
    //    int[,] mapState = new int[Config.TableSize, Config.TableSize];

    //    for (int i = 0; i < Config.TableSize; i++)
    //    {
    //        for (int j = 0; j < Config.TableSize; j++)
    //        {
    //            if (s_checkers[i, j] == null)
    //            {
    //                mapState[i, j] = -1;
    //            }
    //            else
    //            {
    //                mapState[i, j] = (int)s_checkers[i, j].GetComponent<CheckerManager>()?.Type;
    //            }
    //        }
    //    }

    //    return mapState;
    //}

}
