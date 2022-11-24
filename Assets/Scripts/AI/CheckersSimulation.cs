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
    [SerializeField] private PlayerType _gizmoCurrentPlayer;
    [SerializeField] private Vector2Int _fromPos;
    [SerializeField] private Vector2Int _toPos;

    [SerializeField] private readonly bool _logBestChoice;
    //[SerializeField] private UnityEvent<Transform[,], PlayerType> _boardHeuristic;


    public (Vector2Int, Vector2Int) AIGetNextMove(int depth, PlayerType selfPlayer)
    {
        return AIGetNextMove(GridManager.CurrentSimulatedBoardState, depth, selfPlayer, selfPlayer);
    }
    public (Vector2Int, Vector2Int) AIGetNextMove(SimulatedCell[,] board, int depth, PlayerType currentTurn, PlayerType selfPlayer)
    {
        _gizmoMoveList.Clear();

        (float score, SimulatedCell[,] chosenBoard) = Minimax(board, depth, float.NegativeInfinity, float.PositiveInfinity, currentTurn, selfPlayer);
        (Vector2Int fromPos, Vector2Int toPos) = GetMoveBetweenBoard(board, chosenBoard);

        Move chosenMove = new Move(fromPos, toPos, selfPlayer);
        Debug.Log($"{chosenMove}");
        _fromPos = fromPos;
        _toPos = toPos;
        _gizmoCurrentPlayer = selfPlayer;
        return (fromPos, toPos);
    }

    private static (Vector2Int, Vector2Int) GetMoveBetweenBoard(SimulatedCell[,] fromBoard, SimulatedCell[,] toBoard)
    {
        Vector2Int fromPos = default;
        Vector2Int toPos = default;

        for (int i = 0; i < Config.TableSize; i++)
        {
            for (int j = 0; j < Config.TableSize; j++)
            {
                if (toBoard[i, j] is CheckerSimulated && fromBoard[i, j] is FloorSimulated)
                    toPos = fromBoard[i, j].Cell;
                if (fromBoard[i, j] is CheckerSimulated && toBoard[i, j] is FloorSimulated)
                    fromPos = fromBoard[i, j].Cell;
            }
        }
        return (fromPos, toPos);
    }

    private static SimulatedCell[,] DeepCloneBoard(SimulatedCell[,] original)
    {
        SimulatedCell[,] boardSimulated = new SimulatedCell[Config.TableSize, Config.TableSize];
        {
            for (int i = 0; i < Config.TableSize; i++)
            {
                for (int j = 0; j < Config.TableSize; j++)
                {
                    boardSimulated[i, j] = original[i, j].Clone() as SimulatedCell;
                }
            }
        }

        return boardSimulated;
    }

    private (float, SimulatedCell[,]) Minimax(SimulatedCell[,] board, int depth, float alpha, float beta, PlayerType currentTurn, PlayerType selfPlayer, string debugStr = "")
    {
        if (depth <= 0 || EvaluateWinner(board) != -1)
        {
            // if leave node reached, static evaluate the board and return the value
            return (EvaluateStaticScore(board, selfPlayer), board);
        }
        // Generate possible moves
        List<Move> moveList = GeneratePossibleMoves(board, currentTurn);


        //Evaluating each move


        SimulatedCell[,] selectedMove = new SimulatedCell[Config.TableSize, Config.TableSize];
        string pathStr = "";
        int bestIndex = 0;
        bool maximize = currentTurn == selfPlayer;
        float bestScore = maximize ? float.NegativeInfinity : float.PositiveInfinity;

        for (int index = 0; index < moveList.Count; index++)
        {
            Move currentMove = moveList[index];

            //Simulate the move on a virtual board
            SimulatedCell[,] boardSimulated = DeepCloneBoard(board);
            SimulateCheckerMove(ref boardSimulated, currentMove);

            //prepare string for debug log
            string tempStr = debugStr
                + $"\nLevel {depth}: {currentMove.ToString()}\n"
                + LogSimulatedBoard(boardSimulated, Config.TableSize, Config.TableSize);

            //Evaluate the board after move
            (float thisMoveScore, _) = Minimax(boardSimulated, depth - 1, alpha, beta, 1 - currentTurn, selfPlayer, tempStr + "\n\n");


            if (maximize)
            {
                if (thisMoveScore > bestScore)
                {
                    //Debug.Log($"On my turn ${edgeDepth / 2} switch selected move from {selectedMove.ToString()} - {bestScore} to {currentMove.ToString()} - {thisMoveScore}");
                    selectedMove = boardSimulated;
                    bestScore = thisMoveScore;
                    pathStr = debugStr
                        + $"\nLevel {depth}: {currentMove.ToString()} of score {bestScore}\n"
                        + LogSimulatedBoard(boardSimulated, Config.TableSize, Config.TableSize);
                    bestIndex = index;
                }
                alpha = Mathf.Max(alpha, bestScore);
                if (beta <= alpha) break;
            }
            else
            {
                if (thisMoveScore < bestScore)
                {
                    //Debug.Log($"On enemy turn ${(edgeDepth - 1) / 2} switch selected move from {selectedMove.ToString()} - {bestScore} to {currentMove.ToString()} - {thisMoveScore}");
                    selectedMove = boardSimulated;
                    bestScore = thisMoveScore;
                    pathStr = debugStr
                        + $"\nLevel {depth}: {currentMove.ToString()} of score {bestScore}\n"
                        + LogSimulatedBoard(boardSimulated, Config.TableSize, Config.TableSize);
                    bestIndex = index;
                }

                beta = Mathf.Min(beta, bestScore);
                if (beta <= alpha) break;
            }
            //Debug.Log($"Alpha: {alpha}, Beta: {beta}");
        }

        if (_logBestChoice) Debug.Log($"{(maximize ? "Maximize" : "Minimize")} Level {depth} choose node {bestIndex} of score {bestScore}: \n" + pathStr);

        return (bestScore, selectedMove);
    }

    private static void SimulateCheckerMove(ref SimulatedCell[,] boardSimulated, Move currentMove)
    {
        if (currentMove.IsKillingMove)
        {
            Vector2Int destroyedCell = (currentMove.To - currentMove.From) / 2 + currentMove.From;
            DestroyChecker(ref boardSimulated, destroyedCell);
        }

        MoveChecker(ref boardSimulated, currentMove.From, currentMove.To);
    }

    private static void DestroyChecker(ref SimulatedCell[,] s_checkers, Vector2Int destroyedCell)
    {
        if (s_checkers[destroyedCell.x, destroyedCell.y] is CheckerSimulated)
            s_checkers[destroyedCell.x, destroyedCell.y] = new FloorSimulated(s_checkers[destroyedCell.x, destroyedCell.y].Cell);
    }

    private static void MoveChecker(ref SimulatedCell[,] s_checkers, Vector2Int oldPos, Vector2Int newPos)
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

    private static int HasCheckerPiece(SimulatedCell[,] board, int x, int y)
    {
        if (!CheckPositionInBoard(x, y)) return -1;
        return ((board[x, y] is CheckerSimulated)) ? 1 : 0;
    }

    private static List<Move> GetMoveableFloor(SimulatedCell[,] board, CheckerSimulated piece, out bool isKillableMoveList)
    {
        int x = piece.Cell.x;
        int y = piece.Cell.y;
        PlayerType playerType = piece.Type;
        bool isQueen = piece.IsQueen;
        List<Move> normalMoves = new List<Move>();
        List<Move> killingMoves = new List<Move>();

        for (int i = -1; i <= 1; i++)
        {
            for (int k = -1; k <= 1; k++)
            {
                if (i == 0 || k == 0 || !CheckPositionInBoard(x + k, y + i)) continue;
                if (!isQueen && ((playerType == PlayerType.PLAYER && k == -1) || (playerType == PlayerType.OPPONENT && k == 1)))
                    continue;
                if (HasCheckerPiece(board, x + k, y + i) == 0)
                {
                    normalMoves.Add(new Move(piece.Cell, board[x + k, i + y].Cell, piece.Type));
                }
                if (HasCheckerPiece(board, x + k, y + i) == 1
                    &&
                    ((CheckerSimulated)board[x + k, y + i]).Type != playerType
                    &&
                    HasCheckerPiece(board, x + k * 2, y + i * 2) == 0)
                {
                    killingMoves.Add(new Move(piece.Cell, board[x + k * 2, y + i * 2].Cell, piece.Type));
                }
            }
        }
        if (killingMoves.Count > 0)
        {
            isKillableMoveList = true;
            return killingMoves;
        }
        isKillableMoveList = false;
        return normalMoves;
    }

    private float EvaluateStaticScore(SimulatedCell[,] board, PlayerType selfPlayer)
    {
        int selfPieces = 0;
        int enemyPieces = 0;

        int selfQueens = 0;
        int enemyQueens = 0;

        //float selfScore = 0;
        //float enemyScore = 0;

        foreach (SimulatedCell cell in board)
        {
            if (cell is CheckerSimulated)
            {
                CheckerSimulated piece = cell as CheckerSimulated;
                //float checkerScore = piece.CalculateSelfWorth(board);

                if (piece.Type == selfPlayer)
                {
                    //selfScore += checkerScore;
                    selfPieces++;
                    if (piece.IsQueen) selfQueens++;
                }
                else
                {
                    //enemyScore += checkerScore;
                    enemyPieces++;
                    if (piece.IsQueen) enemyQueens++;
                }
            }
        }
        return selfPieces - enemyPieces + (selfQueens - enemyQueens) * 0.5f;
        //return selfScore - enemyScore;
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

    private static string LogSimulatedBoard(SimulatedCell[,] array, int rows, int cols)
    {
        string ret = "";
        for (int i = cols - 1; i >= 0; i--)
        {
            ret += "[";
            for (int j = 0; j < rows; j++)
            {
                string temptTxt = "";
                if (array[i, j] is CheckerSimulated)
                {
                    var temp = array[i, j] as CheckerSimulated;
                    temptTxt += temp.IsQueen ? "Q" : "+";
                    temptTxt += temp.Type == PlayerType.PLAYER ? "0" : "1";
                }
                else temptTxt += "___";
                ret += temptTxt;
                if (j < rows - 1) ret += ",";
            }
            ret += "]\n";
        }

        return ret;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (_gizmoMoveList != null)
            {
                foreach (Move move in _gizmoMoveList)
                {
                    Gizmos.color = move.Type == PlayerType.OPPONENT ? Color.yellow : Color.blue;
                    Gizmos.DrawLine(GridManager.GetWorldPos(move.From), GridManager.GetWorldPos(move.To));
                }
            }

            Gizmos.color = _gizmoCurrentPlayer == PlayerType.OPPONENT ? Color.yellow : Color.blue;
            Gizmos.DrawWireSphere(GridManager.GetWorldPos(_toPos), 1f);
        }

    }

    [System.Serializable]
    private struct Move
    {
        public PlayerType Type;
        public Vector2Int From;
        public Vector2Int To;
        public bool IsKillingMove { get { return Math.Abs(From.x - To.x) == 2; } }

        public Move(Vector2Int from, Vector2Int to, PlayerType type)
        {
            From = from;
            To = to;
            Type = type;
        }

        public override string ToString()
        {
            return $"{(Type == PlayerType.OPPONENT ? "Yellow" : "Black")} {(IsKillingMove ? "kill move" : "normal move")} [{From.ToString()}] -> [{To.ToString()}]";
        }
    }
}
