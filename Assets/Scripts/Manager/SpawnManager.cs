using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static string s_blackFloorPath = "Prefabs/BlackFloor";
    private static string s_whiteFloorPath = "Prefabs/WhiteFloor";
    private static string s_playerCheckerPath = "Prefabs/Checker 1";
    private static string s_oppCheckerPath = "Prefabs/Checker 2";
    public static void InitTable(out Transform[,] floors, out Transform[,] checkers)
    {
        InitFloor(out floors);
        InitChecker(out checkers);
    }

    public static void LoadFromFile(string fromFile, out Transform[,] floors, out Transform[,] checkers)
    {
        InitFloor(out floors);
        LoadTableFromFile(fromFile, out checkers);
    }

    private static void InitFloor(out Transform[,] floors)
    {
        GameObject tableObj = GameObject.Find("Table");
        if (tableObj == null)
        {
            tableObj = new GameObject("Table");
        }

        floors = new Transform[Config.TableSize, Config.TableSize];
        GameObject blackFloorPrefab = Resources.Load<GameObject>(s_blackFloorPath);
        GameObject whiteFloorPrefab = Resources.Load<GameObject>(s_whiteFloorPath);

        for (int i = 0; i < Config.TableSize; i++)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                GameObject floorPrefab = ((i + j) % 2 == 0) ? whiteFloorPrefab : blackFloorPrefab;
                floors[i, j] = Instantiate<GameObject>(floorPrefab, worldPos, Quaternion.identity, tableObj.transform).transform;
                floors[i, j].GetComponent<FloorManager>().Init(i, j);
            }
        }
    }

    private static void InitChecker(out Transform[,] checkers)
    {
        GameObject checkersObj = GameObject.Find("Checkers");
        if (checkersObj == null)
        {
            checkersObj = new GameObject("Checkers");
        }

        checkers = new Transform[Config.TableSize, Config.TableSize];
        GameObject playerCheckerPrefab = Resources.Load<GameObject>(s_playerCheckerPath);
        GameObject oppCheckerPrefab = Resources.Load<GameObject>(s_oppCheckerPath);

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                if ((i + j) % 2 == 0)
                    continue;
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                checkers[i, j] = Instantiate<GameObject>(playerCheckerPrefab, worldPos, Quaternion.identity, checkersObj.transform).transform;
                checkers[i, j].GetComponent<CheckerManager>().Init(PlayerType.PLAYER, i, j);
            }
        }

        for (int i = Config.TableSize - 1; i >= Config.TableSize - 2; i--)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                if ((i + j) % 2 == 0)
                    continue;
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                checkers[i, j] = Instantiate<GameObject>(oppCheckerPrefab, worldPos, Quaternion.identity, checkersObj.transform).transform;
                checkers[i, j].GetComponent<CheckerManager>().Init(PlayerType.OPPONENT, i, j);
            }
        }
    }

    public static void LoadTableFromFile(string fromFile, out Transform[,] checkers){
        GameObject checkersObj = GameObject.Find("Checkers");
        if (checkersObj == null)
        {
            checkersObj = new GameObject("Checkers");
        }

        checkers = new Transform[Config.TableSize, Config.TableSize];
        GameObject playerCheckerPrefab = Resources.Load<GameObject>(s_playerCheckerPath);
        GameObject oppCheckerPrefab = Resources.Load<GameObject>(s_oppCheckerPath);

        string file_path = fromFile;
        StreamReader reader = new StreamReader(file_path);
        // string temp = reader.ReadLine();
        // string[] s = temp.Split(' ');
        
        
        for (int i = 0; i < Config.TableSize; i++){
            for(int j = 0; j < Config.TableSize; j++){
                string temp = reader.ReadLine();
                string[] s = temp.Split(' ');
                int x = (int)System.Convert.ToInt64(s[0]);
                int y = (int)System.Convert.ToInt64(s[1]);
                int type = (int)System.Convert.ToInt64(s[2]);
                if (type == (int)PlayerType.PLAYER){
                    Vector3 worldPos = GridManager.GetWorldPos(i, j);
                    checkers[x, y] = Instantiate<GameObject>(playerCheckerPrefab, worldPos, Quaternion.identity, checkersObj.transform).transform;
                    checkers[x, y].GetComponent<CheckerManager>().Init(PlayerType.PLAYER, x, y);
                }
                else if (type == (int)PlayerType.OPPONENT){
                    Vector3 worldPos = GridManager.GetWorldPos(x, y);
                    checkers[x, y] = Instantiate<GameObject>(oppCheckerPrefab, worldPos, Quaternion.identity, checkersObj.transform).transform;
                    checkers[x, y].GetComponent<CheckerManager>().Init(PlayerType.OPPONENT, x, y);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Config.TableSize; i++)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                Vector3 worldPos = GridManager.GetWorldPos(i, j);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(worldPos, new Vector3(2, 0.2f, 2));
                DrawString($"{i},{j}", worldPos + 0.5f * Vector3.up, Color.green, Vector2.zero);
            }
        }
    }

    static public void DrawString(string text, Vector3 worldPosition, Color textColor, Vector2 anchor, float textSize = 15f)
    {
#if UNITY_EDITOR
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        if (!view)
            return;
        Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPosition);
        if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
            return;
        var pixelRatio = UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x - UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
        UnityEditor.Handles.BeginGUI();
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = (int)textSize,
            normal = new GUIStyleState() { textColor = textColor }
        };
        Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
        var alignedPosition =
            ((Vector2)screenPosition +
            size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
            Vector2.up * view.camera.pixelHeight;
        GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
        UnityEditor.Handles.EndGUI();
#endif
    }
}
