using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem
{
    public class SceneSwitchTrigger : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private SceneController scene_switcher;
        [SerializeField] private int scene_number = 0;
        [Space]

        [Header("Activation type")]
        [SerializeField] private bool activate_on_trigger = false;
        [SerializeField] private bool activate_on_click = false;
        [Space]

        [Header("Activation types properties")]
        [SerializeField] private GameObject activate_on_click_object;
        [SerializeField] private float outline_selected_width = 3;
        [SerializeField] private bool switch_on_click = false;
        [Space]

        [Header("Animtion")]
        [SerializeField] private string click_activation_animation_name = "";
        [SerializeField] private float click_activation_animation_time = 0;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(activate_on_trigger && other.tag == scene_switcher.getPlayerTag())
            {
                scene_switcher.SwitchScene(scene_number);
            }
        }

        private void OnMouseOver()
        {
            if(activate_on_click)
            {
                Outline outliner = GetComponent<Outline>();
                if (outliner != null)
                {
                    GameObject player_raycast = GameObject.FindGameObjectWithTag("SceneInteractionsRaycast");
                    if (player_raycast != null && Vector3.Distance(player_raycast.transform.position, transform.position) <= player_raycast.GetComponent<RaycastElement>().getMaxDistance())
                    {
                        outliner.OutlineWidth = outline_selected_width;
                    }

                }
            }
        }

        private void OnMouseExit()
        {
            if(activate_on_click) DeactivateOutline();
        }

        private void DeactivateOutline()
        {
            Outline outliner = GetComponent<Outline>();
            if (outliner != null) outliner.OutlineWidth = 0;
        }

        public bool ActivatesOnClick()
        {
            return activate_on_click;
        }

        public void Activate()
        {
            DeactivateOutline();
            StartCoroutine("Activation");
        }

        private IEnumerator Activation()
        {
            float wait_time = 0;
            if (click_activation_animation_name != "")
            {
                wait_time = click_activation_animation_time;
                animator.Play(click_activation_animation_name);
            }

            yield return new WaitForSeconds(wait_time);

            if (switch_on_click)
            {
                scene_switcher.SwitchScene(scene_number);
            }
            else
            {
                activate_on_click_object.SetActive(true);
            }
        }
    }
}
