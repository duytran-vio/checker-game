using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private PlayerType _turn;
    private Transform _currentChecker;
    private List<Transform> _moveableFloors;
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
    }

    private void Change_turn(){
        _turn = 1 - _turn;
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
        _moveableFloors = GridManager.GetMoveableFloor(checkerManager.GridPos.x, checkerManager.GridPos.y, checkerManager.Type, false);
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

    public void OnClickChecker(Transform checker){
        CheckerManager checkerManager = checker.GetComponent<CheckerManager>();
        if (_turn != checkerManager.Type) 
            return;
        SelectChecker(checker);
    }

}
