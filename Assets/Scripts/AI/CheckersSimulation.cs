using Array2DEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;



public class CheckersSimulation : MonoSingleton<CheckersSimulation>
{
    [SerializeField] private List<Move> _gizmoMoveList;
    [SerializeField] private Move _gizmoSelectedMove;

    public (Vector2Int, Vector2Int) Minimax(Transform[,] board, int depth, PlayerType currentTurn, PlayerType selfPlayer)
    {
        Move chosenMove = Minimax(board, depth, currentTurn, selfPlayer, out float score);
        _gizmoSelectedMove = chosenMove;
        return (chosenMove.From, chosenMove.To);
    }

    private Move Minimax(Transform[,] board, int depth, PlayerType currentTurn, PlayerType selfPlayer, out float score)
    {
        // Generate possible moves
        bool isKillableMoveList = false;
        List<Move> moveList = new List<Move>();
        for (int i = 0; i < Config.TableSize; i++)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                if (board[i, j] != null)
                {
                    CheckerManager piece = board[i, j].GetComponent<CheckerManager>();
                    if (piece.Type == currentTurn)
                    {
                        List<Transform> possibleMoveTransforms = GridManager.GetMoveableFloor(board, board[i, j], out bool isKillableCached);
                        if (isKillableCached == true && isKillableMoveList == false)
                        {
                            moveList = (from move in moveList where move.IsKillingMove select move).ToList();
                        }

                        if (!isKillableMoveList || isKillableMoveList && isKillableCached)
                        {
                            foreach (var move in possibleMoveTransforms)
                            {
                                FloorManager manager = move.GetComponent<FloorManager>();
                                //Debug.Log($"Possible move: [{i},{j}] -> [{manager.Cell.x}, {manager.Cell.y}]");
                                moveList.Add(new Move(new Vector2Int(i, j), manager.Cell, isKillableMoveList));
                            }
                        }

                        isKillableMoveList = isKillableMoveList || isKillableCached;
                    }
                }
            }
        }

        Debug.Log($"Simulating board on depth {depth} generates {moveList.Count} moves.");

        //Evaluating each move
        score = currentTurn == selfPlayer ? float.NegativeInfinity : float.PositiveInfinity;
        Move selectedMove = _gizmoSelectedMove;

        foreach (Move move in moveList)
        {
            Transform[,] boardSimulated = new Transform[Config.TableSize, Config.TableSize];
            Array.Copy(GridManager.CurrentBoardState, boardSimulated, Config.TableSize * Config.TableSize);

            if (move.IsKillingMove)
            {
                Vector2Int destroyedCell = (move.To - move.From) / 2 + move.From;
                GridManager.DestroyChecker(ref boardSimulated, destroyedCell);
            }

            GridManager.MoveChecker(ref boardSimulated, move.From, move.To);
            float thisMoveScore;

            if (depth <= 0)
            {
                //if max depth is reached, we evaluate the board for score
                thisMoveScore = EvaluateStaticScore(boardSimulated, selfPlayer);
            }
            else
            {
                //else we run minimax for this simulated move for the score
                Minimax(boardSimulated, depth - 1, 1 - currentTurn, selfPlayer, out thisMoveScore);
            }

            Debug.Log($"Simulating {move.ToString()} on depth {depth} yield {thisMoveScore}.");

            if (currentTurn == selfPlayer)
            {
                if (thisMoveScore > score)
                {
                    selectedMove = move;
                    score = thisMoveScore;
                }
            }
            else
            {
                if (thisMoveScore < score)
                {
                    selectedMove = move;
                    score = thisMoveScore;
                }
            }
        }

        if (depth > 0) Debug.Log($"Depth {depth} simulation choosing {selectedMove.ToString()}.");

        if (depth <= 0)
        {
            _gizmoMoveList.Clear();
            _gizmoMoveList = moveList;
        }
        else
        {

        }
        return selectedMove;
    }

    private float EvaluateStaticScore(Transform[,] board, PlayerType selfPlayer)
    {
        int selfPieces = 0;
        int enemyPieces = 0;

        foreach (Transform cell in board)
        {
            if (cell != null)
            {
                CheckerManager piece = cell.GetComponent<CheckerManager>();
                if (piece.Type == selfPlayer)
                {
                    selfPieces++;
                }
                else
                {
                    enemyPieces++;
                }
            }
        }
        return selfPieces - enemyPieces;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (_gizmoMoveList != null)
            {
                foreach (Move move in _gizmoMoveList)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(GridManager.GetWorldPos(move.From), GridManager.GetWorldPos(move.To));
                }
            }

            Gizmos.DrawSphere(GridManager.GetWorldPos(_gizmoSelectedMove.To), 1f);
        }

    }

    [System.Serializable]
    private struct Move
    {
        public Vector2Int From;
        public Vector2Int To;
        public bool IsKillingMove;

        public Move(Vector2Int from, Vector2Int to, bool isKillingMove = false)
        {
            From = from;
            To = to;
            IsKillingMove = isKillingMove;
        }

        public override string ToString()
        {
            return $"{(IsKillingMove ? "Killing move" : "Normal move")} [{From.ToString()}] -> [{To.ToString()}]";
        }
    }
}
