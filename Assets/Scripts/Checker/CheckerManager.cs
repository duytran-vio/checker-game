using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerManager : MonoBehaviour
{
    public PlayerType Type;
    public float time;
    public Vector2Int Cell;

    public void Init(PlayerType type, int i, int j){
        Type = type;
        Cell = new Vector2Int(i, j);
    }

    void Start(){
        // GoToPosition(GridManager.GetWorldPos(1, 1));
    }

    public void MoveToCell(Vector2Int newCell){
        Cell = newCell;
        Vector3 newPos = GridManager.GetWorldPos(newCell.x, newCell.y);
        GoToPosition(newPos);
    }

    void GoToPosition(Vector3 pos){
        StartCoroutine(GoToPositionCoroutine(pos));
    }

    IEnumerator GoToPositionCoroutine(Vector3 pos){
        int cnt = 0;
        float v = Vector3.Distance(transform.position, pos) / time;
        Vector3 dir = (pos - transform.position).normalized;
        while (true){
            Vector3 newPos = transform.position + Time.deltaTime * v * dir;
            transform.position = newPos;
            cnt++;
            if (Vector3.Distance(newPos,pos) <= 0.01f) 
            {
                transform.position = pos;
                break;
            }
            else
                yield return null;
        }
    }
}
