using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HP
{
    public class DestructibleDeath : DeathController
    {
        public GameObject[] parts;

        public override void Death()
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].transform.SetParent(null);
                parts[i].SetActive(true);
            }

            if(this != null && this.gameObject != null) Destroy(this.gameObject);
        }
    }
}
