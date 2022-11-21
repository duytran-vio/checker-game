using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Transform[,] floors;
    private Transform[,] checkers;
    void Start()
    {
        SpawnManager.InitTable(out floors, out checkers);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
