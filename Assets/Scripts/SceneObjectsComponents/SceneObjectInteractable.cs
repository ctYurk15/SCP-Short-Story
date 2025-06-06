using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneObjectsInteractable
{
    public class SceneObjectInteractable : MonoBehaviour
    {
        [SerializeField] private bool one_time;

        [SerializeField] private bool activated = false;
        private bool interaction_enabled = true;

        protected PlayerSceneInteractor player_interactor;
        protected HoverOutlineController outline_controller;

        protected void Start()
        {
            outline_controller = GetComponent<HoverOutlineController>();
        }

        public void Interact(PlayerSceneInteractor interactor)
        {
            if((!one_time || !activated) && interaction_enabled)
            {
                activated = true;
                player_interactor = interactor;

                InteractAction();
            }
        }

        public virtual void InteractAction()
        {
            Debug.Log("Default interaction");
        }

        public virtual void ResetState()
        {
            activated = false;
            Activate();
        }

        public void Deactivate()
        {
            interaction_enabled = false;

            if(outline_controller != null)
            {
                outline_controller.StopOutline();
            }
        }

        public void Activate()
        {
            interaction_enabled = true;

            if (outline_controller != null)
            {
                outline_controller.StartOutline();
            }
        }
    }
}
