using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    
    public Button PauseBtn;
    public Transform PauseMenu;
    public Button SaveBtn;
    public Button ResumeBtn;
    public Button MenuBtn;

    void Start(){
        PauseMenu?.gameObject.SetActive(false);
        PauseBtn.onClick.AddListener(() => OnClickPause());
        SaveBtn?.onClick.AddListener(() => OnClickSave());
        ResumeBtn?.onClick.AddListener(() => OnClickResume());
        MenuBtn?.onClick.AddListener(() => OnClickMenu());
    }

    public void OnClickPause(){
        Debug.Log("Pause");
        PauseMenu.gameObject.SetActive(true);
    }

    public void OnClickSave(){
        GameManager.SaveCurrentTable();
    }

    public void OnClickResume(){
        PauseMenu.gameObject.SetActive(false);
    }

    public void OnClickMenu(){
        PlayerPrefs.SetString("FromFile","");
        SceneManager.LoadScene("Menu");
    }
}
