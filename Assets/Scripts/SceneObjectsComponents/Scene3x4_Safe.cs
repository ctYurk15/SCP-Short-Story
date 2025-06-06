using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneObjectsInteractable
{
    public class Scene3x4_Safe : SceneObjectInteractable
    {
        [Header("Safe activation")]
        [SerializeField] private GameObject bunker_trapdoors;
        [SerializeField] private string open_animation_name;
        [SerializeField] private float open_animation_time;
        [SerializeField] private AudioSource[] audio_parts;
        [SerializeField] private HologramsGroupsActivator holograms_activator;
        [Space]

        [Header("Reset config")]
        [SerializeField] private float time_to_reset;
        [SerializeField] private string idle_animation_name;

        private bool audio_and_holograms_spawned = false;

        private new void Start()
        {
            base.Start();
            Deactivate();
        }

        public override void InteractAction()
        {
            StartCoroutine("OpenAction");
        }

        public override void ResetState()
        {
            base.ResetState();
        }

        IEnumerator OpenAction()
        {
            Animator animator = GetComponent<Animator>();

            //open safe
            animator.Play(open_animation_name);
            yield return new WaitForSeconds(open_animation_time);
            
            //show bunker trapdoor
            bunker_trapdoors.SetActive(true);
            outline_controller.StopOutline();
            player_interactor.MemeticEffectBlink();

            //talk with operator & spawn holograms
            float audio_talk_time = 0;
            if(!audio_and_holograms_spawned)
            {
                audio_and_holograms_spawned = true;

                holograms_activator.ActivateHolograms();

                foreach (AudioSource audio_part in audio_parts)
                {
                    audio_part.Play();
                    audio_talk_time += audio_part.clip.length;
                    yield return new WaitForSeconds(audio_part.clip.length);
                }
            }

            //reset state
            yield return new WaitForSeconds(time_to_reset - audio_talk_time);
            animator.Play(idle_animation_name);
            bunker_trapdoors.SetActive(false);
            ResetState();
        }

    }
}

    
