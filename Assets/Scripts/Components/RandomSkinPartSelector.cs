using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkinPartSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;
    [Range(0f, 1f)]
    [SerializeField] private float[] parts_disappear_probabilities; 

    private void Start()
    {
        for(int i = 0; i < parts.Length; i++)
        {
            float random_number = Random.Range(0f, 100f) / 100f;
            if (random_number < parts_disappear_probabilities[i]) Destroy(parts[i]);
        }
    }
}
