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
    public static void HandleMouseInput(){
        if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = s_mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)){
                Transform obj = hit.transform;
                if (obj.tag == CheckerTag){
                    GameManager.Instance.OnClickChecker(hit.transform);
                }
                else if (obj.tag == FloorTag ){
                    Debug.Log(FloorTag);
                    
                }
            }
        }
    }
}
