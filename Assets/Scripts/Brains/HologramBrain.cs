using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brains
{
    public class HologramBrain : MonoBehaviour
    {
        [SerializeField] private float leader_hp_multiplier = 2;
        [SerializeField] private float leader_speed_multiplier = 1.5f;
        [SerializeField] private float leader_fov_angle_multiplier = 1.25f;
        [SerializeField] private float leader_fov_distance_multiplier = 1.5f;
        [SerializeField] private RandomSkinSelector[] holograms_spawn_points = { };
        [SerializeField] private bool activate_on_start;

        private void Start()
        {
            if(activate_on_start)
            {
                foreach (RandomSkinSelector spawner in holograms_spawn_points)
                {
                    spawner.activate();
                }
                this.SelectNewLeader();
            }
        }

        public void SelectNewLeader()
        {
            HologramScript[] holograms = FindObjectsOfType<HologramScript>();
            if(holograms.Length > 1)
            {
                int leader_index = UnityEngine.Random.Range(0, holograms.Length);

                holograms[leader_index].MakeLeader(
                    leader_hp_multiplier,
                    leader_speed_multiplier,
                    leader_fov_angle_multiplier,
                    leader_fov_distance_multiplier
                );
            }
        }

        public void notifyAboutPlayer(Vector3 position)
        {
            HologramScript[] holograms = FindObjectsOfType<HologramScript>();
            foreach(HologramScript hologram in holograms)
            {
                hologram.notifyAboutPlayer(position);
            }
        }

        public bool SomeoneSeesPlayer()
        {
            bool result = false;

            HologramScript[] holograms = FindObjectsOfType<HologramScript>();
            foreach (HologramScript hologram in holograms)
            {
                if (hologram.CanSeePlayer()) result = true;
            }

            return result;
        }
    }
}
