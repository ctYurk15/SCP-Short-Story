using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Scene3x1_PathsMixer : MonoBehaviour
{
    [Header("Path objects:")]
    [SerializeField] private GameObject safe_path;
    [SerializeField] private GameObject danger_path;
    [Space]

    [Header("Path positions")]
    [SerializeField] private Transform safe_path_left_transform;
    [SerializeField] private Transform safe_path_right_transform;
    [SerializeField] private Transform danger_path_left_transform;
    [SerializeField] private Transform danger_path_right_transform;
    [Space]

    [Header("Holograms navmesh configs")]
    [SerializeField] private Vector3 left_path_hologram_patrol_center;
    [SerializeField] private Vector3 right_path_hologram_patrol_center;
    [SerializeField] private float left_path_patrol_points_range;
    [SerializeField] private float right_path_patrol_points_range;
    [Space]

    [Header("Other")]
    [SerializeField] private HologramsGroupsActivator holograms_activator;

    //there are two positions & two object, so the are only two possible scenarious
    private bool safe_path_is_left = false;

    private void Start()
    {
        Mix();
    }

    private void Mix()
    {
        System.Random random_generator = new System.Random(MicroScenesSystem.SeedGenerator.current_seed);
        safe_path_is_left = (int)random_generator.Next(0, 2) == 0;

        safe_path.transform.position = safe_path_is_left ? safe_path_left_transform.transform.position : safe_path_right_transform.position;
        safe_path.transform.rotation = safe_path_is_left ? safe_path_left_transform.transform.rotation : safe_path_right_transform.rotation;

        danger_path.transform.position = safe_path_is_left ? danger_path_right_transform.transform.position : danger_path_left_transform.position;
        danger_path.transform.rotation = safe_path_is_left ? danger_path_right_transform.transform.rotation : danger_path_left_transform.rotation;

        if(safe_path_is_left) holograms_activator.SetHologramsPatrolConfig(right_path_hologram_patrol_center, right_path_patrol_points_range);
        else holograms_activator.SetHologramsPatrolConfig(left_path_hologram_patrol_center, left_path_patrol_points_range);
    }

    // 1 or 2
    public bool safePathIsLeft()
    {
        return safe_path_is_left;
    }
}
