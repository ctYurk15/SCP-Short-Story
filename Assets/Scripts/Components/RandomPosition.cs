using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private bool use_seed;

    private void Start()
    {
        int index = 0;

        if(use_seed)
        {
            System.Random random_generator = new System.Random(MicroScenesSystem.SeedGenerator.current_seed);
            index = random_generator.Next(0, positions.Length);
        }
        else
        {
            index = UnityEngine.Random.Range(0, positions.Length);
        }

        this.transform.position = positions[index].position;
        this.transform.rotation = positions[index].rotation;
    }
}
