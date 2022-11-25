using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static Transform s_camera1;
    private static Transform s_camera2;
    void Awake(){
        s_camera1 = GameObject.Find("Player 1 Camera").transform;
        s_camera2 = GameObject.Find("Player 2 Camera")?.transform;
    }

    void Start(){
        SetPlayer1Camera();
    }

    public static void SetPlayer1Camera(){
        s_camera1.gameObject.SetActive(true);
        s_camera2?.gameObject.SetActive(false);
        InputManager.SetCamera(s_camera1);
    }

    public static void SetPlayer2Camera(){
        s_camera1.gameObject.SetActive(false);
        s_camera2?.gameObject.SetActive(true);
        InputManager.SetCamera(s_camera2);
    }
}
