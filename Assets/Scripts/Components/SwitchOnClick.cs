using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnClick : MonoBehaviour
{
    [Header("S to switch the object")]
    public GameObject object_to_switch;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            object_to_switch.SetActive(!object_to_switch.activeSelf);
        }
    }
}
