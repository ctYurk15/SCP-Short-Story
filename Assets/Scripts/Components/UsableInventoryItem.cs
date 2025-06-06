using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableInventoryItem : MonoBehaviour
{
    [Header("Basic stats")]
    [SerializeField] private string pick_up_animation_name;
    [SerializeField] private float pick_up_animation_time;
    [SerializeField] private string item_type;
    [Space]

    [Header("Audio")]
    [SerializeField] private AudioSource pick_up_sound;
    [SerializeField] private AudioSource use_sound;
    [Space]

    [Header("Animation")]
    [SerializeField] private string using_animation_name = "";
    [SerializeField] private float using_time_amount = 0;
    [SerializeField] private float hiding_time_amount = 0;
    [Space]

    [Header("Droping parts")]
    [SerializeField] private GameObject[] dropingObjects;
    [SerializeField] private Transform[] dropingPositions;
    [SerializeField] private float[] dropingTimings;
    [Space]

    [Header("Health stats")]
    [SerializeField] private float healing_amount = 0;
    [SerializeField] private float health_time = 0;
    [SerializeField] private float health_effect_time = 0;

    public float HealingAmount { get => healing_amount; }
    public float UsingTimeAmount { get => using_time_amount; }
    public float HealthTime { get => health_time; }
    public float HealthEffectTime { get => health_effect_time; }

    public void pickUp()
    {
        Animator animator = GetComponent<Animator>();

        pick_up_sound.Play();
        animator.Play(pick_up_animation_name);
    }

    public float Use()
    {
        Animator animator = GetComponent<Animator>();

        if(use_sound != null) use_sound.Play();
        animator.Play(using_animation_name);

        if(dropingObjects.Length > 0)
        {
            StartCoroutine("DropingItems");
        }

        return using_time_amount;
    }

    IEnumerator DropingItems()
    {
        for(int i = 0; i < dropingObjects.Length; i++)
        {
            Transform dropping_position = dropingPositions[i];
            float dropping_timing = dropingTimings[i];

            yield return new WaitForSeconds(dropping_timing);
            GameObject.Instantiate(dropingObjects[i], dropping_position.position, dropping_position.rotation);
            //Time.timeScale = 0f;
        }
    }

    public float getPickUpAnimationTime()
    {
        return pick_up_animation_time;
    }

    public float getUsageTime()
    {
        return using_time_amount;
    }

    public float getHidingTime()
    {
        return hiding_time_amount;
    }
}
