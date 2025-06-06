using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogScript : MonoBehaviour
{
    [Header("Basic stats")]
    [SerializeField] private float damage_amount;
    [SerializeField] private float damage_interval;
    [Space]

    [Header("Damage multipliers")]
    [SerializeField] private string[] hp_controllers_names;
    [SerializeField] private float[] hp_controllers_damage_multipliers;

    private Dictionary<int, HP.HPComponent> health_controllers = new Dictionary<int, HP.HPComponent>();
    private bool damaged = false;

    // Add new object to list
    private void OnTriggerEnter(Collider other)
    {
        health_controllers.Add(other.transform.GetInstanceID(), other.transform.GetComponent<HP.HPComponent>());
    }

    // Remove objects from list
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<HP.HPComponent>() != null)
        {
            health_controllers.Remove(other.transform.GetInstanceID());
        }
    }

    private void Update()
    {
        if(health_controllers.Count > 0 && !damaged)
        {
            damaged = true;

            List<int> destroyed_objects_ids = new List<int>();

            //damage all objects inside
            foreach(KeyValuePair<int, HP.HPComponent> item in health_controllers)
            {
                HP.HPComponent health_controller = item.Value;

                //if object is not destroyed, damage
                if (health_controller != null)
                {
                    float object_damage = damage_amount;

                    //check, if this health controller has damage multiplier
                    for (int i = 0; i < hp_controllers_names.Length; i++)
                    {
                        if(hp_controllers_names[i] == health_controller.GetType().Name)
                        {
                            object_damage *= hp_controllers_damage_multipliers[i];
                            break;
                        }
                    }

                    health_controller.Damage(object_damage);
                }
                //if destroyed, remove from the list
                else
                {
                    destroyed_objects_ids.Add(item.Key);
                }
            }

            //remove destroyed objects from list
            foreach (int object_id in destroyed_objects_ids)
            {
                health_controllers.Remove(object_id);
            }

            StartCoroutine("CoolDownDamage");
        }
    }

    private IEnumerator CoolDownDamage()
    {
        yield return new WaitForSeconds(damage_interval);
        damaged = false;
    }

}
