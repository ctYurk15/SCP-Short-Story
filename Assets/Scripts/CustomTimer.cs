using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTimer : MonoBehaviour
{
    [Header("Timer step (in seconds)")]
    [SerializeField] private float step;

    [SerializeField] private float total_seconds;

    public float TotalSeconds
    {
        get => total_seconds;
    }

    public void StartCount()
    {
        total_seconds = 0;
        StartCoroutine("TimerLoop");
    }

    public void StopCount()
    {
        StopCoroutine("TimerLoop");
    }

    private IEnumerator TimerLoop()
    {
        yield return new WaitForSeconds(step);
        total_seconds += step;

        StartCoroutine("TimerLoop");
    }
}
