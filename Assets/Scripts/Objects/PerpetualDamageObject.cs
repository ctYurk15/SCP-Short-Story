using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerpetualDamageObject : MonoBehaviour
{
    [SerializeField] private float damage_amount;
    [SerializeField] private float damage_interval;

    private HP.PlayerHealth player_health;
    private bool is_damaging;

    private void Update()
    {
        if(player_health != null && !is_damaging)
        {
            is_damaging = true;
            StartCoroutine(Damage());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HP.PlayerHealth component = other.GetComponent<HP.PlayerHealth>();
        if (component != null) player_health = component;
    }

    private void OnTriggerExit(Collider other)
    {
        HP.PlayerHealth component = other.GetComponent<HP.PlayerHealth>();
        if (component != null) player_health = null;
    }

    IEnumerator Damage()
    {
        player_health.Damage(damage_amount);
        yield return new WaitForSeconds(damage_interval);
        is_damaging = false;
    }
}
