using Array2DEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CheckersSimulation : MonoBehaviour
{

    public (Vector2Int, Vector2Int) Minimax(Transform[,] board, int depth, int turn, int red_best, int black_best)
    {
        return (Vector2Int.zero, Vector2Int.zero);
    }

    private void OnDrawGizmos()
    {

    }
}
