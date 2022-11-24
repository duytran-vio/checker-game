using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerManager : MonoBehaviour
{
    public PlayerType Type;
    public float time;
    public Vector2Int Cell;
    public bool isQueen;

    private Renderer _renderer;
    private Transform _crow;

    public void Init(PlayerType type, int i, int j){
        Type = type;
        Cell = new Vector2Int(i, j);
        isQueen = false;
        _crow.gameObject.SetActive(false);
    }

    void Awake(){
        _renderer = transform.GetChild(0).GetComponent<Renderer>();
        _crow = transform.GetChild(1);
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
        while (true){
            Vector3 dir = (pos - transform.position).normalized;
            Vector3 newPos = transform.position + Time.deltaTime * v * dir;
            transform.position = newPos;
            cnt++;
            if (Vector3.Distance(newPos,pos) <= 0.1f) 
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
        gameObject.SetActive(false);
    }

    public void DestroyThisChecker(){
        Fade();
    }

    public void BecomeQueen(){
        isQueen = true;
        _crow.gameObject.SetActive(true);
    }
}
