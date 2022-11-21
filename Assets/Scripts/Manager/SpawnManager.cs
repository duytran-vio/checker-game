using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static string s_blackFloorPath = "Prefabs/BlackFloor";
    private static string s_whiteFloorPath = "Prefabs/WhiteFloor";
    private static string s_playerCheckerPath = "Prefabs/Checker 1";
    private static string s_oppCheckerPath = "Prefabs/Checker 2";
    public static void InitTable(out Transform[,] floors, out Transform[,] checkers){
        InitFloor(out floors);
        InitChecker(out checkers);
        checkers = null;
    }

    private static void InitFloor(out Transform[,] floors){
        GameObject tableObj = GameObject.Find("Table");
        if (tableObj == null){
            tableObj = new GameObject("Table");
        }

        floors = new Transform[Config.TableSize, Config.TableSize];
        GameObject blackFloorPrefab = Resources.Load<GameObject>(s_blackFloorPath);
        GameObject whiteFloorPrefab = Resources.Load<GameObject>(s_whiteFloorPath);

        for(int i = 0; i < Config.TableSize; i++){
            for(int j = 0; j < Config.TableSize; j++){
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                GameObject floorPrefab = ((i + j) % 2 == 0) ? whiteFloorPrefab : blackFloorPrefab;
                floors[i, j] = Instantiate<GameObject>(floorPrefab, worldPos, Quaternion.identity, tableObj.transform).transform;
            }
        }
    }

    private static void InitChecker(out Transform[,] checkers){
        GameObject checkersObj = GameObject.Find("Checkers");
        if (checkersObj == null){
            checkersObj = new GameObject("Checkers");
        }

        checkers = new Transform[Config.TableSize, Config.TableSize];
        GameObject playerCheckerPrefab = Resources.Load<GameObject>(s_playerCheckerPath);
        GameObject oppCheckerPrefab = Resources.Load<GameObject>(s_oppCheckerPath);

        for(int i = 0; i < 2; i++){
            for(int j = 0; j < Config.TableSize; j++){
                if ((i + j) % 2 == 0)
                    continue;
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                checkers[i, j] = Instantiate<GameObject>(playerCheckerPrefab, worldPos, Quaternion.identity, checkersObj.transform).transform;
                checkers[i,j].GetComponent<CheckerManager>().Init(PlayerType.PLAYER);
            }
        }

        for(int i = Config.TableSize - 1; i >= Config.TableSize - 2; i--){
            for(int j = 0; j < Config.TableSize; j++){
                if ((i + j) % 2 == 0)
                    continue;
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                checkers[i, j] = Instantiate<GameObject>(oppCheckerPrefab, worldPos, Quaternion.identity, checkersObj.transform).transform;
                checkers[i,j].GetComponent<CheckerManager>().Init(PlayerType.OPPONENT);
            }
        }
    }
}
