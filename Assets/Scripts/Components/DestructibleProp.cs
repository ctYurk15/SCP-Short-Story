using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleProp : MonoBehaviour
{
    public GameObject[] parts;
    public float hp = 100;

    public void Damage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].transform.SetParent(null);
            parts[i].SetActive(true);
        }

        Destroy(this.gameObject);
    }
}