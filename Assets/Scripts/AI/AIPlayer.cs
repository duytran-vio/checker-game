using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] PlayerType _side;
    [SerializeField] int depth;

    private void Start()
    {
        depth = PlayerPrefs.GetInt("depth");
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
            (var fromPos, var toPos) = await CheckersSimulation.Instance.AIGetNextMove(depth, _side);
            await Task.Delay(500);
            //Debug.Log($"{_side} wants to move from {fromPos} to {toPos}.");

            GameManager.Instance.OnClickChecker(GridManager.GetCell(fromPos), _side);
            GameManager.Instance.OnClickFloor(GridManager.GetCell(toPos), _side);
            SimulatedCell[,] temp = GridManager.CurrentSimulatedBoardState;

        }
    }
}
