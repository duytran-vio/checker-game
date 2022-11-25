using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private Transform _currentChecker;
    private List<Transform> _moveableFloors;
    private List<Transform> _checkerCanKill;
    [SerializeField] private PlayerType _currentTurn = PlayerType.NONE;

    public static event Action<PlayerType> TurnChanged;

    public PlayerType CurrentTurn { get { return _currentTurn; } private set { _currentTurn = value; } }

    void Start()
    {
        GridManager.InitGrid();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // InputManager.HandleMouseInput();
    }

    private void Init()
    {
        CurrentTurn = PlayerType.PLAYER;
        _currentChecker = null;
        _checkerCanKill = new List<Transform>();
        TurnChanged?.Invoke(CurrentTurn);
    }

    private bool HasCheckerCanKill()
    {
        return _checkerCanKill.Count > 0;
    }

    private void UpdateScore(){

    }

    private void ChangeTurn()
    {
        UpdateScore();
        CurrentTurn = Config.SwitchTurn(CurrentTurn);
        TurnChanged?.Invoke(CurrentTurn);
        //if (CurrentTurn == PlayerType.OPPONENT)
        //{
        //    CheckersSimulation.Instance.AIGetNextMove(4, PlayerType.OPPONENT);
        //}
        //else
        //{
        //    CheckersSimulation.Instance.AIGetNextMove(4, PlayerType.PLAYER);
        //}
        _checkerCanKill = GridManager.GetCheckerCanKill(CurrentTurn);
    }

    private bool IsCurrentChecker(Transform checker)
    {
        return _currentChecker == checker;
    }

    private void UnSelectCurrentChecker()
    {
        if (_currentChecker == null) return;
        _currentChecker = null;
        foreach (Transform floor in _moveableFloors)
        {
            floor.GetComponent<FloorManager>().ResetFloorColor();
        }
        _moveableFloors = null;
    }

    private void ChangeCurrentChecker(Transform checker)
    {
        _currentChecker = checker;
        CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
        _moveableFloors = GridManager.GetMoveableFloor(checker, out bool isKillableMoveList);
        foreach (Transform floor in _moveableFloors)
        {
            floor.GetComponent<FloorManager>().SelectFloor();
        }
    }

    private void SelectChecker(Transform checker)
    {
        if (IsCurrentChecker(checker))
        {
            UnSelectCurrentChecker();
            return;
        }

        UnSelectCurrentChecker();
        ChangeCurrentChecker(checker);
    }

    private void ShowWarning()
    {
        foreach (Transform checker in _checkerCanKill)
        {
            CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
            Transform floorUnderChecker = GridManager.GetFloor(checkerManager.Cell);
            floorUnderChecker.GetComponent<FloorManager>().Warning();
        }
    }

    public void OnClickChecker(Transform checker, PlayerType fromPlayerType)
    {
        Debug.Log(fromPlayerType + " " + CurrentTurn);
        if (checker == null || fromPlayerType != CurrentTurn) return;
        CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
        //Debug.Log($"From mouse: {checkerManager.Cell.x}, {checkerManager.Cell.y}");
        if (CurrentTurn != checkerManager.Type)
            return;
        bool isShowWarning = HasCheckerCanKill() && !_checkerCanKill.Contains(checker);
        if (isShowWarning)
        {
            ShowWarning();
        }
        else
        {
            SelectChecker(checker);
        }
    }

    private void CheckBecomeQueen()
    {
        CheckerManager checkerManager = _currentChecker.GetComponent<CheckerManager>();
        //Debug.Log(checkerManager.Type + " " + checkerManager.Cell);
        if ((checkerManager.Type == PlayerType.PLAYER && checkerManager.Cell.x == Config.TableSize - 1)
            || (checkerManager.Type == PlayerType.OPPONENT && checkerManager.Cell.x == 0))
        {
            checkerManager.BecomeQueen();
        }
    }

    public void OnClickFloor(Transform floor, PlayerType fromPlayerType)
    {
        if (fromPlayerType != CurrentTurn || _currentChecker == null || !_moveableFloors.Contains(floor))
            return;
        CheckerManager checkerManager = _currentChecker.GetComponent<CheckerManager>();
        FloorManager floorManager = floor.GetComponent<FloorManager>();

        //Destroy Opponent Checker
        if (HasCheckerCanKill())
        {
            Vector2Int destroyedCell = (floorManager.Cell - checkerManager.Cell) / 2 + checkerManager.Cell;

            Transform destroyedChecker = GridManager.GetChecker(destroyedCell);
            GridManager.DestroyChecker(destroyedCell);
            destroyedChecker.GetComponent<CheckerManager>().DestroyThisChecker();
        }

        //Move
        GridManager.MoveChecker(checkerManager.Cell, floorManager.Cell);
        checkerManager.MoveToCell(floorManager.Cell);

        //Check become queen
        CheckBecomeQueen();

        //End move
        UnSelectCurrentChecker();
        ChangeTurn();
    }

}
