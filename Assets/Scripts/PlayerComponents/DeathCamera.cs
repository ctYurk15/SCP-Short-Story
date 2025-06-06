using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    [SerializeField] private HP.PlayerHealth player_health_component;
    [SerializeField] private float checkFallAfterTime = 0.1f;

    private bool falled = false;
    private bool checkFall = false;

    private void Start()
    {
        StartCoroutine("waitForFallCheck");
    }


    void Update()
    {
        if(!falled && checkFall)
        {
            Rigidbody myRigidbody = GetComponent<Rigidbody>();
            if (myRigidbody.velocity.x == 0 && myRigidbody.velocity.y == 0 && myRigidbody.velocity.z == 0)
            {
                falled = true;
                doSomethingWhenFalling();
            }
        }
    }

    IEnumerator waitForFallCheck()
    {
        yield return new WaitForSeconds(checkFallAfterTime);
        checkFall = true;
    }

    private void doSomethingWhenFalling()
    {
        player_health_component.DeathCameraFall();
    }
}
