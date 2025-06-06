using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeScripts : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
            Debug.Log("Quit");
        }
    }
}
