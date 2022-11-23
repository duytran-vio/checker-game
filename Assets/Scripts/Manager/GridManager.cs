using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    private static Transform[,] s_floors;
    private static Transform[,] s_checkers;

    public static void InitGrid(){
        SpawnManager.InitTable(out s_floors, out s_checkers);
    }

    public static Vector3 GetWorldPos(int i, int j){
        Vector3 pos = new Vector3();
        pos.z = i * Config.CellSize;
        pos.x = j * Config.CellSize;
        return pos;
    }

    private static bool isValid(int x, int y){
        return x >= 0 && x < Config.TableSize && y >= 0 && y < Config.TableSize;
    }

    private static int isCheckerOnCell(int x, int y){
        if (!isValid(x, y)) return -1;
        return (s_checkers[x, y] != null) ? 1 : 0;
    }

    public static List<Transform> GetMoveableFloor(Transform checker, out bool isKillableMoveList){
        CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
        int x = checkerManager.Cell.x;
        int y = checkerManager.Cell.y;
        PlayerType playerType = checkerManager.Type;
        bool isQueen = false;
        List<Transform> moveableFloors = new List<Transform>();
        List<Transform> killableMove = new List<Transform>();
        int k = (playerType == PlayerType.PLAYER) ? 1 : -1;
        for (int i = -1; i <= 1; i++){
            if (i == 0 || !isValid(x + k, y + i) ) continue;
            if (isCheckerOnCell(x + k, y + i) == 0){
                moveableFloors.Add(s_floors[x + k, y + i]);
            }
            if (isCheckerOnCell(x + k, y + i) == 1 && s_checkers[x + k, y + i].GetComponent<CheckerManager>().Type != playerType && isCheckerOnCell(x + k * 2, y + i * 2) == 0){
                killableMove.Add(s_floors[x + k * 2, y + i * 2]);
            }
        }
        if (killableMove.Count > 0){
            isKillableMoveList = true;
            return killableMove;
        }
        isKillableMoveList = false;
        return moveableFloors;
    }

    public static void MoveChecker(Vector2Int oldCell, Vector2Int newCell){
        s_checkers[newCell.x, newCell.y] = s_checkers[oldCell.x, oldCell.y];
        s_checkers[oldCell.x, oldCell.y] = null;
    }

    public static List<Transform> GetCheckerCanKill(PlayerType type){
        List<Transform> checkCanKill = new List<Transform>();
        for(int i = 0; i < Config.TableSize; i++){
            for(int j = 0; j < Config.TableSize; j++){
                if (s_checkers[i,j] == null)
                    continue;
                Transform checker = s_checkers[i,j];
                if (checker.GetComponent<CheckerManager>().Type != type) continue;
                bool isKillableMoveList = false;
                GetMoveableFloor(checker, out isKillableMoveList);
                if (isKillableMoveList){
                    checkCanKill.Add(checker);
                }
            }
        }
        return checkCanKill;
    }

    public static Transform GetFloor(Vector2Int cell){
        return s_floors[cell.x, cell.y];
    }

    public static Transform GetChecker(Vector2Int cell){
        return s_checkers[cell.x, cell.y];
    }

    public static void DestroyChecker(Vector2Int destroyedCell){
        Transform destroyedChecker = GetChecker(destroyedCell);
        destroyedChecker.GetComponent<CheckerManager>().DestroyThisChecker();
        s_checkers[destroyedCell.x, destroyedCell.y] = null;
    }
}
