using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private Transform _currentChecker;
    private List<Transform> _moveableFloors;
    private List<Transform> _checkerCanKill;
    [SerializeField] private PlayerType _currentTurn = PlayerType.NONE;

    public static event Action<PlayerType> TurnChanged;

    public PlayerType CurrentTurn { get { return _currentTurn; } private set { _currentTurn = value; } }
    private int _thisUserId = -1;

    public void SetThisUserId(int id){
        _thisUserId = id;
    }

    void Start()
    {
        if (PlayerPrefs.GetString("FromFile") == "")
            GridManager.InitGrid();
        else 
            GridManager.InitGrid(PlayerPrefs.GetString("FromFile"));

        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOnline()){
            InputManager.HandleMouseInput(PlayerType.PLAYER);
        }
    }

    private void Init()
    {
        CurrentTurn = PlayerType.PLAYER;
        _currentChecker = null;
        _checkerCanKill = new List<Transform>();
        UpdateScore();
        TurnChanged?.Invoke(CurrentTurn);
    }

    private bool HasCheckerCanKill()
    {
        return _checkerCanKill.Count > 0;
    }

    private void UpdateScore(){
        GridManager.GetCheckerCount(out int playerCheckerCount, out int opponentCheckerCount);
        UIManager.Instance.UpdateScore(playerCheckerCount, opponentCheckerCount);
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
        // Debug.Log(fromPlayerType + " " + CurrentTurn);
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

        //
        var fromCell = checkerManager.Cell;
        var toCell = floorManager.Cell;

        //Move
        GridManager.MoveChecker(checkerManager.Cell, floorManager.Cell);
        checkerManager.MoveToCell(floorManager.Cell);

        //Check become queen
        CheckBecomeQueen();

        //End move
        UnSelectCurrentChecker();
        ChangeTurn();

        if (isOnline()){
            SendRequest.Instance.SendRequestMovement(_thisUserId, fromPlayerType, fromCell, toCell);
        }
    }

    private bool isOnline(){
        return _thisUserId != -1;
    }

    public void MoveFromNetwork(int userId, PlayerType playerType, Vector2Int fromCell, Vector2Int toCell){
        if (userId == _thisUserId) return;
        GameManager.Instance.OnClickChecker(GridManager.GetCell(fromCell), playerType);
        GameManager.Instance.OnClickFloor(GridManager.GetCell(toCell), playerType);
    }

    public static void SaveCurrentTable(){
        string save_path = "Assets/Resources/Save/save_" + PlayerPrefs.GetInt("depth").ToString() + ".txt";
        Debug.Log(save_path);
        Transform[,] checkers = GridManager.GetCurrentTable();
        StreamWriter writer = new StreamWriter(save_path, false);
        for(int i = 0; i < Config.TableSize; i++){
            for(int j = 0; j < Config.TableSize; j++){
                if (checkers[i, j] == null) {
                    writer.WriteLine("" + i + " " + j + " " + (int)PlayerType.NONE);
                    continue;
                }
                CheckerManager checkerManager = checkers[i,j].GetComponent<CheckerManager>();
                writer.WriteLine("" + i + " " + j + " " + (int)checkerManager.Type);
            }
        }
        writer.Close();
    }
}
