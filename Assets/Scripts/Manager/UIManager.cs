using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    public Text BlackScoreText;
    public  Text YellowScoreText;
    public Button PauseBtn;
    public Transform PauseMenu;
    public Button SaveBtn;
    public Button ResumeBtn;
    public Button MenuBtn;

    void Start(){
        PauseMenu.gameObject.SetActive(false);
        PauseBtn.onClick.AddListener(() => OnClickPause());
        SaveBtn.onClick.AddListener(() => OnClickSave());
        ResumeBtn.onClick.AddListener(() => OnClickResume());
        MenuBtn.onClick.AddListener(() => OnClickMenu());
    }

    private void OnClickPause(){
        PauseMenu.gameObject.SetActive(true);
    }

    public void UpdateScore(int playerScore, int opponentScore){
        BlackScoreText.text = playerScore.ToString();
        YellowScoreText.text = opponentScore.ToString();
    }

    public void OnClickSave(){
        GameManager.SaveCurrentTable();
    }

    public void OnClickResume(){
        PauseMenu.gameObject.SetActive(false);
    }

    public void OnClickMenu(){
        SceneManager.LoadScene("Menu");
    }
}
