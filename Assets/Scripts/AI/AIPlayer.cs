﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] PlayerType _side;

    private void Start()
    {
        GameManager.TurnChanged += OnTurnChanged;
    }

    private void OnDestroy()
    {
        GameManager.TurnChanged -= OnTurnChanged;
    }

    internal async void OnTurnChanged(PlayerType turn)
    {
        if (Application.isPlaying && turn == _side)
        {
            (var fromPos, var toPos) = await CheckersSimulation.Instance.AIGetNextMove(6, _side);
            await Task.Delay(500);
            //Debug.Log($"{_side} wants to move from {fromPos} to {toPos}.");

            //TO DO: Làm tiếp cho con AI nó đi giùm anh với
            GameManager.Instance.OnClickChecker(GridManager.GetCell(fromPos));
            GameManager.Instance.OnClickFloor(GridManager.GetCell(toPos));
            SimulatedCell[,] temp = GridManager.CurrentSimulatedBoardState;

        }
    }
}
