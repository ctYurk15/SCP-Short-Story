using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    [Tooltip("Time in seconds, after which the object will return to the pool")]
    public float return_time = 1f;

    private Queue<GameObject> return_queue = new Queue<GameObject>();

    public GameObject getNext()
    {
        if(transform.childCount != 0)
        {
            //get next children object
            Transform result = transform.GetChild(0);

            //detach from pool
            result.parent = null;
            result.gameObject.SetActive(true);

            //add to return-queue
            return_queue.Enqueue(result.gameObject);

            //return link to it
            return result.gameObject;
        }

        return null;
    }

    private void Start()
    {
        InvokeRepeating("Return", return_time, return_time);
    }

    private void Return()
    {
        if(return_queue.Count != 0)
        {
            GameObject object_to_return = return_queue.Dequeue();
            if (object_to_return != null)
            {
                object_to_return.transform.parent = this.transform;
                object_to_return.transform.position = this.transform.position;
                object_to_return.SetActive(false);
            }
        }
    }
}
