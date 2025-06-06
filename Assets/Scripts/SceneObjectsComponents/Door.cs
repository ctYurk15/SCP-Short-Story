using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneObjectsInteractable
{
    public class Door : SceneObjectInteractable
    {
        [Header("Animation")]
        [SerializeField] private string open_animation_name;
        [SerializeField] private string idle_animation_name;
        [Space]

        [Header("Other")]
        [SerializeField] private AudioSource[] open_sounds;

        public override void InteractAction()
        {
            Animator animator = GetComponent<Animator>();
            animator.Play(open_animation_name);

            int index = UnityEngine.Random.Range(0, open_sounds.Length);
            open_sounds[index].Play();
        }

        public override void ResetState()
        {
            base.ResetState();

            Animator animator = GetComponent<Animator>();
            animator.Play(idle_animation_name);
        }
    }
}
