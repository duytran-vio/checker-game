using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    private Renderer _renderer;
    private Color _originalColor;
    // Start is called before the first frame update
    void Awake(){
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.GetColor("_Color");
    }
    void Start()
    {
        
    }

    public void ResetFloorColor(){
        ChangeFloorColor(_originalColor);
    }

    public void SelectFloor(){
        ChangeFloorColor(Color.green);
    }

    private void ChangeFloorColor(Color color){
        _renderer.material.SetColor("_Color", color); 
    } 
}
