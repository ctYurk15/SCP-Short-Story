using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [SerializeField] private float damage_amount = 10;

    private void OnCollisionEnter(Collision collision)
    {
        HP.HPComponent player_health = collision.transform.GetComponent<HP.HPComponent>();

        if(player_health != null)
        {
            player_health.Damage(damage_amount);
            Destroy(this.gameObject);
        }
    }
}
