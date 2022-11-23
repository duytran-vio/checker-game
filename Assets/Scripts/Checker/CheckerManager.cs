using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerManager : MonoBehaviour
{
    public PlayerType Type;
    public float time;
    public Vector2Int Cell;

    private Renderer _renderer;

    public void Init(PlayerType type, int i, int j){
        Type = type;
        Cell = new Vector2Int(i, j);
    }

    void Start(){
        _renderer = transform.GetChild(0).GetComponent<Renderer>();
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

    public void Fade(){
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine(){
        Color c = _renderer.material.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
        {
            c.a = alpha;
            _renderer.material.color = c;
            yield return null;
        }
    }
}
