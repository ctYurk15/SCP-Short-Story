using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    void Start()
    {
        int index = Random.Range(0, objects.Length);
        Instantiate(objects[index], transform);
    }
}
