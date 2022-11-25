using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private static Camera s_mainCamera;
    public static string CheckerTag = "Checker";
    public static string FloorTag = "Floor";

    void Awake(){
        s_mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    public static void HandleMouseInput(PlayerType playerType = PlayerType.NONE){
        if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = s_mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue)){
                Transform obj = hit.transform;
                if (obj.tag == CheckerTag){
                    GameManager.Instance.OnClickChecker(hit.transform, playerType);
                }
                else if (obj.tag == FloorTag ){
                    GameManager.Instance.OnClickFloor(hit.transform, playerType);
                }
            }
        }
    }
}
