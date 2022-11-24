using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class CheckersSimulation : MonoSingleton<CheckersSimulation>
{
    [SerializeField] private List<Move> _gizmoMoveList;
    [SerializeField] private Move _gizmoSelectedMove;
    //[SerializeField] private UnityEvent<Transform[,], PlayerType> _boardHeuristic;

    public (Vector2Int, Vector2Int) AIGetNextMove(SimulatedCell[,] board, int depth, PlayerType currentTurn, PlayerType selfPlayer)
    {
        _gizmoMoveList.Clear();
        (float score, Move chosenMove) = Minimax(board, depth, currentTurn, selfPlayer);
        Debug.Log($"Move selected: {chosenMove.ToString()}");
        _gizmoSelectedMove = chosenMove;
        return (chosenMove.From, chosenMove.To);
    }

    private (float, Move) Minimax(SimulatedCell[,] board, int edgeDepth, PlayerType currentTurn, PlayerType selfPlayer)
    {
        // Generate possible moves
        List<Move> moveList = GeneratePossibleMoves(board, currentTurn);

        //Debug.Log($"Simulating board on edge depth {edgeDepth} generates {moveList.Count} moves.");

        //Evaluating each move
        float bestScore = currentTurn == selfPlayer ? float.NegativeInfinity : float.PositiveInfinity;

        Move selectedMove = new Move(Vector2Int.zero, Vector2Int.zero);
        foreach (Move currentMove in moveList)
        {
            //Simulate the move on a virtual board
            SimulatedCell[,] boardSimulated = new SimulatedCell[Config.TableSize, Config.TableSize];
            {
                for (int i = 0; i < Config.TableSize; i++)
                {
                    for (int j = 0; j < Config.TableSize; j++)
                    {
                        boardSimulated[i, j] = board[i, j].Clone() as SimulatedCell;
                    }
                }
            }

            if (currentMove.IsKillingMove)
            {
                Vector2Int destroyedCell = (currentMove.To - currentMove.From) / 2 + currentMove.From;
                DestroyChecker(ref boardSimulated, destroyedCell);
            }

            MoveChecker(ref boardSimulated, currentMove.From, currentMove.To);


            float thisMoveScore;
            //Evaluate the board after move
            if (edgeDepth <= 0 || EvaluateWinner(boardSimulated) != -1)
            {
                thisMoveScore = EvaluateStaticScore(boardSimulated, selfPlayer);
            }
            else
            {
                (thisMoveScore, _) = Minimax(boardSimulated, edgeDepth - 1, 1 - currentTurn, selfPlayer);
            }


            //Debug.Log($"Simulating {move.ToString()} on edge depth {edgeDepth} yield {thisMoveScore}.");

            bool maximize = currentTurn == selfPlayer;
            if (maximize)
            {

                if (thisMoveScore > bestScore)
                {
                    Debug.Log($"On my turn ${edgeDepth / 2} switch selected move from {selectedMove.ToString()} - {bestScore} to {currentMove.ToString()} - {thisMoveScore}");
                    selectedMove = currentMove;
                    bestScore = thisMoveScore;
                }
            }
            else
            {
                if (thisMoveScore < bestScore)
                {
                    Debug.Log($"On enemy turn ${(edgeDepth - 1) / 2} switch selected move from {selectedMove.ToString()} - {bestScore} to {currentMove.ToString()} - {thisMoveScore}");
                    selectedMove = currentMove;
                    bestScore = thisMoveScore;
                }
            }
        }

        //if (edgeDepth > 0) Debug.Log($"Edge depth {edgeDepth} simulation choosing {selectedMove.ToString()}.");

        _gizmoMoveList.Add(selectedMove);

        return (bestScore, selectedMove);
    }

    public static void DestroyChecker(ref SimulatedCell[,] s_checkers, Vector2Int destroyedCell)
    {
        if (s_checkers[destroyedCell.x, destroyedCell.y] is CheckerSimulated)
            s_checkers[destroyedCell.x, destroyedCell.y] = new FloorSimulated(s_checkers[destroyedCell.x, destroyedCell.y].Cell);
    }

    public static void MoveChecker(ref SimulatedCell[,] s_checkers, Vector2Int oldPos, Vector2Int newPos)
    {
        FloorSimulated cellToMoveTo = s_checkers[newPos.x, newPos.y] as FloorSimulated;
        CheckerSimulated checker = s_checkers[oldPos.x, oldPos.y] as CheckerSimulated;

        s_checkers[newPos.x, newPos.y] = checker;
        s_checkers[oldPos.x, oldPos.y] = cellToMoveTo;

        cellToMoveTo.Cell = oldPos;
        checker.Cell = newPos;

        if ((checker.Type == PlayerType.PLAYER && checker.Cell.x == Config.TableSize - 1)
            || (checker.Type == PlayerType.OPPONENT && checker.Cell.x == 0))
        {
            checker.BecomeQueen();
            Debug.Log(checker.ToString());
        }
    }

    private List<Move> GeneratePossibleMoves(SimulatedCell[,] board, PlayerType player)
    {
        bool isKillableMoveList = false;
        List<Move> moveList = new List<Move>();
        for (int i = 0; i < Config.TableSize; i++)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                if (board[i, j] is CheckerSimulated)
                {
                    CheckerSimulated piece = board[i, j] as CheckerSimulated;
                    if (piece.Type == player)
                    {
                        List<Move> possibleMoveByThisChecker = GetMoveableFloor(board, piece, out bool isKillableCached);
                        if (isKillableCached == true && isKillableMoveList == false)
                        {
                            moveList = (from move in moveList where move.IsKillingMove select move).ToList();
                        }

                        if (!isKillableMoveList || isKillableMoveList && isKillableCached)
                        {
                            foreach (var move in possibleMoveByThisChecker)
                            {
                                moveList.Add(move);
                            }
                        }

                        isKillableMoveList = isKillableMoveList || isKillableCached;
                    }
                }
            }
        }
        return moveList;
    }

    private static bool CheckPositionInBoard(int x, int y)
    {
        return x >= 0 && x < Config.TableSize && y >= 0 && y < Config.TableSize;
    }

    private static int IsCheckerOnCell(SimulatedCell[,] s_checkers, int x, int y)
    {
        if (!CheckPositionInBoard(x, y)) return -1;
        return ((s_checkers[x, y] is CheckerSimulated)) ? 1 : 0;
    }

    private static List<Move> GetMoveableFloor(SimulatedCell[,] s_checkers, CheckerSimulated checker, out bool isKillableMoveList)
    {
        int x = checker.Cell.x;
        int y = checker.Cell.y;
        PlayerType playerType = checker.Type;
        bool isQueen = checker.IsQueen;
        List<Move> moveableFloors = new List<Move>();
        List<Move> killableMove = new List<Move>();
        // int k = (playerType == PlayerType.PLAYER) ? 1 : -1;
        for (int i = -1; i <= 1; i++)
        {
            for (int k = -1; k <= 1; k++)
            {
                if (i == 0 || k == 0 || !CheckPositionInBoard(x + k, y + i)) continue;
                if (!isQueen && ((playerType == PlayerType.PLAYER && k == -1) || (playerType == PlayerType.OPPONENT && k == 1)))
                    continue;
                if (IsCheckerOnCell(s_checkers, x + k, y + i) == 0)
                {
                    moveableFloors.Add(new Move(checker.Cell, s_checkers[x + k, i + y].Cell));
                }
                if (IsCheckerOnCell(s_checkers, x + k, y + i) == 1
                    &&
                    ((CheckerSimulated)s_checkers[x + k, y + i]).Type != playerType
                    &&
                    IsCheckerOnCell(s_checkers, x + k * 2, y + i * 2) == 0)
                {
                    killableMove.Add(new Move(checker.Cell, s_checkers[x + k * 2, y + i * 2].Cell, true));
                }
            }
        }
        if (killableMove.Count > 0)
        {
            isKillableMoveList = true;
            return killableMove;
        }
        isKillableMoveList = false;
        return moveableFloors;
    }

    private float EvaluateStaticScore(SimulatedCell[,] board, PlayerType selfPlayer)
    {
        //int selfPieces = 0;
        //int enemyPieces = 0;

        float selfScore = 0;
        float enemyScore = 0;

        foreach (SimulatedCell cell in board)
        {
            if (cell is CheckerSimulated)
            {
                CheckerSimulated piece = cell as CheckerSimulated;
                float checkerScore = piece.CalculateSelfWorth(board);

                if (piece.Type == selfPlayer)
                {
                    selfScore += checkerScore;
                }
                else
                {
                    enemyScore += checkerScore;
                }
            }
        }
        //return selfPieces - enemyPieces;
        return selfScore - enemyScore;
    }

    private int EvaluateWinner(SimulatedCell[,] board)
    {
        int playerPieceCount = 0, opponentPieceCount = 0;
        foreach (SimulatedCell cell in board)
        {
            if (cell is CheckerSimulated)
            {
                CheckerSimulated piece = cell as CheckerSimulated;
                if (piece.Type == PlayerType.PLAYER)
                {
                    playerPieceCount++;
                    if (opponentPieceCount > 0) return -1;
                }
                else
                {
                    opponentPieceCount++;
                    if (playerPieceCount > 0) return -1;
                }
            }
        }

        if (playerPieceCount == 0) return 1;
        else return 0;
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

            Gizmos.DrawWireSphere(GridManager.GetWorldPos(_gizmoSelectedMove.To), 1f);
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
