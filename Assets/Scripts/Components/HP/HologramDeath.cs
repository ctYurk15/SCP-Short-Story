using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Brains;

namespace HP
{
    public class HologramDeath : DeathController
    {
        [SerializeField] private string death_animation_name;
        [SerializeField] private float death_animation_time;
        [SerializeField] private SoundEffect death_sound;
        [SerializeField] private float death_sound_start_time;

        private Animator animator;
        private HologramScript script;

        private void Start()
        {
            animator = GetComponent<Animator>();//
            script = GetComponent<HologramScript>();
        }

        public override void Death()
        {
            StartCoroutine("DieEffect");
        }

        IEnumerator DieEffect()
        {
            script.Die();
            animator.Play(death_animation_name);

            yield return new WaitForSeconds(death_sound_start_time);
            SoundEffect.Create(death_sound, transform.position);
            yield return new WaitForSeconds(death_animation_time - death_sound_start_time);

            FindObjectOfType<HologramBrain>().SelectNewLeader();

            Destroy(this.gameObject);
        }
    }
}
