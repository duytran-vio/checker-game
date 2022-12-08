using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private Transform _mainMenu;
    private Transform _1PMenu;
    private Transform _2PMenu;
    private Transform _loadingScreen;
    private Transform _option;


    private Transform _1PbtnObj;
    private Transform _2PbtnObj;

    private Transform _easyBtnObj;
    private Transform _hardBtnObj;
    private Transform _quitBtnObj;
    private Transform _1PBackBtnObj;

    private Transform _newGameBtnObj;
    private Transform _continueBtnObj;

    private Transform _createBtnObj;
    private Transform _createIdObj;
    private Transform _joinBtnObj;
    private Transform _joinIdObj;
    private Transform _2PBackBtnObj;

    void Start()
    {
        _mainMenu = GameObject.Find("Menu").transform;
        _1PMenu = GameObject.Find("1P Menu").transform;
        _2PMenu = GameObject.Find("2P Menu").transform;
        _loadingScreen = GameObject.Find("Loading").transform;
        _option = GameObject.Find("Option").transform;

        _1PbtnObj = _mainMenu.Find("1Pbtn");
        _2PbtnObj = _mainMenu.Find("2Pbtn");
        _quitBtnObj = _mainMenu.Find("Quit");

        _easyBtnObj = _1PMenu.Find("Easybtn");
        _hardBtnObj = _1PMenu.Find("Hardbtn");
        _1PBackBtnObj = _1PMenu.Find("Backbtn");

        _newGameBtnObj = _option.Find("NewGameBtn");
        _continueBtnObj = _option.Find("ContinueBtn");

        _createBtnObj = _2PMenu.Find("Create/CreateBtn");
        _createIdObj = _2PMenu.Find("Create/CreateRoomID");
        _joinBtnObj = _2PMenu.Find("Join/JoinBtn");
        _joinIdObj = _2PMenu.Find("Join/JoinRoomID");
        _2PBackBtnObj = _2PMenu.Find("Backbtn");

        ShowMenu();
        AddButtonListener(_1PbtnObj, Show1PMenu);
        AddButtonListener(_2PbtnObj, Show2PMenu);
        AddButtonListener(_quitBtnObj, OnClickQuitBtn);

        AddButtonListener(_easyBtnObj, OnClickEasyBtn);
        AddButtonListener(_hardBtnObj, OnClickHardBtn);
        AddButtonListener(_1PBackBtnObj, ShowMenu);

        AddButtonListener(_newGameBtnObj, OnClickNewGame);
        AddButtonListener(_continueBtnObj, OnClickContinue);

        AddButtonListener(_createBtnObj, CreateRoom);
        AddButtonListener(_joinBtnObj, JoinRoom);
        AddButtonListener(_2PBackBtnObj, ShowMenu);
    }

    private void AddButtonListener(Transform btnObj, Action listener){
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(() => listener());
    }

    private void ShowMenu(){
        _mainMenu.gameObject.SetActive(true);
        _1PMenu.gameObject.SetActive(false);
        _2PMenu.gameObject.SetActive(false);
        _loadingScreen.gameObject.SetActive(false);
        _option.gameObject.SetActive(false);
    }

    private void Show1PMenu(){
        _mainMenu.gameObject.SetActive(false);
        _1PMenu.gameObject.SetActive(true);
        _2PMenu.gameObject.SetActive(false);
        _loadingScreen.gameObject.SetActive(false);
        _option.gameObject.SetActive(false);
    }

    private void ShowOption(){
        _mainMenu.gameObject.SetActive(false);
        _1PMenu.gameObject.SetActive(false);
        _2PMenu.gameObject.SetActive(false);
        _loadingScreen.gameObject.SetActive(false);
        _option.gameObject.SetActive(true);
    }

    private void Show2PMenu(){
        _mainMenu.gameObject.SetActive(false);
        _1PMenu.gameObject.SetActive(false);
        _loadingScreen.gameObject.SetActive(true);
        _option.gameObject.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnClickEasyBtn(){
        PlayerPrefs.SetInt("depth", 2);
        ShowOption();
    }

    private void OnClickHardBtn(){
        PlayerPrefs.SetInt("depth", 6);
        ShowOption();
    }

    private void OnClickNewGame(){
        PlayerPrefs.SetString("FromFile", "");
        SceneManager.LoadScene("AI Scene");
    }

    private void OnClickContinue(){
        PlayerPrefs.SetString("FromFile", "Assets/Resources/Save/save_" + PlayerPrefs.GetInt("depth").ToString() + ".txt");
        SceneManager.LoadScene("AI Scene");
    }

    private void OnClickQuitBtn(){
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _loadingScreen.gameObject.SetActive(false);
        _2PMenu.gameObject.SetActive(true);
    }

    private void CreateRoom(){
        var roomId = _createIdObj.GetComponent<InputField>().text;
        PhotonNetwork.CreateRoom(roomId);
    }

    private void JoinRoom(){
        var roomId = _joinIdObj.GetComponent<InputField>().text;
        PhotonNetwork.JoinRoom(roomId);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("2 Players");
    }
}
