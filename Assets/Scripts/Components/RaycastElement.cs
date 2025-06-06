using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastElement : MonoBehaviour
{
    [SerializeField] private float max_distance = 5;

    public RaycastHit getRayCastElement()
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, max_distance);

        return hit;
    }

    public GameObject getElement()
    {
        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, max_distance))
        {
            return hit.transform.gameObject;
        }

        return null;*/

        RaycastHit hit = this.getRayCastElement();
        if (hit.transform != null)
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    public float getMaxDistance()
    {
        return max_distance;
    }
}
