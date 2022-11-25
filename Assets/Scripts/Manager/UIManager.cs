using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public Text BlackScoreText;
    public  Text YellowScoreText;
    public Button PauseBtn;
    public Transform PauseMenu;

    void Start(){
        PauseBtn.onClick.AddListener(() => OnClickPause());
    }

    private void OnClickPause(){

    }

    public void UpdateScore(int playerScore, int opponentScore){
        BlackScoreText.text = playerScore.ToString();
        YellowScoreText.text = opponentScore.ToString();
    }
}
