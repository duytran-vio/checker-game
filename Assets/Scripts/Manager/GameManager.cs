using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private PlayerType _turn;
    private Transform _currentChecker;
    private List<Transform> _moveableFloors;
    private List<Transform> _checkerCanKill;
    void Start()
    {
        GridManager.InitGrid();
        Init();   
    }

    // Update is called once per frame
    void Update()
    {
        InputManager.HandleMouseInput();
    }

    private void Init(){
        _turn = PlayerType.PLAYER;
        _currentChecker = null;
        _checkerCanKill = new List<Transform>();
    }

    private bool HasCheckerCanKill(){
        return _checkerCanKill.Count > 0;
    }

    private void Change_turn(){
        _turn = 1 - _turn;
        _checkerCanKill = GridManager.GetCheckerCanKill(_turn);
    }

    private bool isCurrentChecker(Transform checker){
        return _currentChecker == checker;
    }

    private void UnSelectCurrentChecker(){
        if (_currentChecker == null) return;
        _currentChecker = null;
        foreach(Transform floor in _moveableFloors){
            floor.GetComponent<FloorManager>().ResetFloorColor();
        }
        _moveableFloors = null;
    }

    private void ChangeCurrentChecker(Transform checker){
        _currentChecker = checker;
        CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
        _moveableFloors = GridManager.GetMoveableFloor(checker, out bool isKillableMoveList);
        foreach(Transform floor in _moveableFloors){
            floor.GetComponent<FloorManager>().SelectFloor();
        }
    }

    private void SelectChecker(Transform checker){
        if (isCurrentChecker(checker)){
            UnSelectCurrentChecker();
            return;
        }

        UnSelectCurrentChecker();
        ChangeCurrentChecker(checker);
    }

    private void ShowWarning(){
        foreach(Transform checker in _checkerCanKill){
            CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
            Transform floorUnderChecker = GridManager.GetFloor(checkerManager.Cell);
            floorUnderChecker.GetComponent<FloorManager>().Warning();
        }
    }

    public void OnClickChecker(Transform checker){
        CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
        Debug.Log($"From mouse: {checkerManager.Cell.x}, {checkerManager.Cell.y}");
        if (_turn != checkerManager.Type) 
            return;
        bool isShowWarning = HasCheckerCanKill() && !_checkerCanKill.Contains(checker);
        if (isShowWarning){
            ShowWarning();
        }
        else{
            SelectChecker(checker);
        }
    }

    public void OnClickFloor(Transform floor){
        if (_currentChecker == null || !_moveableFloors.Contains(floor)) 
            return;
        CheckerManager checkerManager = _currentChecker.GetComponent<CheckerManager>();
        FloorManager floorManager = floor.GetComponent<FloorManager>();
        GridManager.MoveChecker(checkerManager.Cell, floorManager.Cell);
        checkerManager.MoveToCell(floorManager.Cell);
        UnSelectCurrentChecker();
        Change_turn();
    }

}
