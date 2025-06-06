using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    public float spawn_rate = 1f;
    
    private void Start()
    {
        InvokeRepeating("Spawn", spawn_rate, spawn_rate);
    }
    
    private void Spawn()
    {
        GameObject obj = GetComponent<ObjectsPool>().getNext();
        if(obj != null)
        {
            Vector3 new_position = new Vector3(
                obj.transform.position.x + UnityEngine.Random.Range(-2, 2), 
                obj.transform.position.y, 
                obj.transform.position.z + UnityEngine.Random.Range(-2, 2));
            obj.transform.position = new_position;
        }
    }
}
