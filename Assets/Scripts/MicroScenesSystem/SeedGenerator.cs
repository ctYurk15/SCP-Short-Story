using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem
{
    public class SeedGenerator : MonoBehaviour
    {
        [SerializeField] private int seed;

        public static int current_seed;

        private void Awake()
        {
            SeedGenerator.current_seed = seed != 0 ? seed : (int)Random.Range(10000, 100000);
        }
    }
}
