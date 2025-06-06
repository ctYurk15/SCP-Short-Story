using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brains;
using System.Linq;
using HologramsActivationsWin;

public class HologramsGroupsActivator : MonoBehaviour
{
    [Header("Enemies data")]
    [SerializeField] private RandomSkinSelector[] holograms_points;
    [SerializeField] private Brains.HologramBrain holograms_brain;
    [SerializeField] private int min_holograms_spawn;
    [SerializeField] private int max_holograms_spawn;
    [SerializeField] private Vector3 holograms_patrol_center;
    [SerializeField] private float holograms_patrol_range;
    [Space]

    [Header("Activation data")]
    [SerializeField] private bool activate_on_trigger;
    [SerializeField] private HologramsActivationWin win_activator;
    [SerializeField] private float check_group_status_time_interval;

    //state
    private bool activated = false;
    private bool group_dead = false;
    private bool checking_group_status = false;
    private List<GameObject> activated_holograms = new List<GameObject>();

    private void Update()
    {
        if(activated && !group_dead && !checking_group_status)
        {
            StartCoroutine("CheckHologramsStatus");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(activate_on_trigger) ActivateHolograms();
    }

    private void ActivateGroup()
    {
        System.Random random_generator = new System.Random(MicroScenesSystem.SeedGenerator.current_seed);
        int holograms_count = random_generator.Next(min_holograms_spawn, max_holograms_spawn + 1);

        RandomSkinSelector[] shuffled_hologram_points = holograms_points.OrderBy(n => random_generator.Next()).ToArray();

        for (int i = 0; i < holograms_count; i++)
        {
            GameObject hologram = shuffled_hologram_points[i].activate();
            hologram.GetComponent<HologramScript>().ChangePatrolConfig(holograms_patrol_center, holograms_patrol_range);
            activated_holograms.Add(hologram);
        }

        holograms_brain.SelectNewLeader();
    }

    public void ActivateHolograms()
    {
        if (!activated)
        {
            ActivateGroup();
            activated = true;
        }
    }

    public void SetHologramsPatrolConfig(Vector3 center, float range)
    {
        this.holograms_patrol_center = center;
        this.holograms_patrol_range = range;
    }

    public bool HologramsGroupAlive()
    {
        int count = 0;
        foreach (GameObject obj in activated_holograms)
        {
            if (obj != null) count++;
        }

        return count > 0;
    }

    IEnumerator CheckHologramsStatus()
    {
        if (!HologramsGroupAlive())
        {
            group_dead = true;
            if (win_activator != null)
            {
                win_activator.Execute();
                StopCoroutine("CheckHologramsStatus");
            }
        }
        else
        {
            yield return new WaitForSeconds(check_group_status_time_interval);
            StartCoroutine("CheckHologramsStatus");
        }
    }
}
