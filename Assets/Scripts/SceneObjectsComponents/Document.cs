using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneObjectsInteractable
{
    public class Document : SceneObjectInteractable
    {
        [SerializeField] protected int document_index;
        [SerializeField] protected GameObject[] objects_to_hide;
        [SerializeField] protected float pickup_action_delay_time;
        [SerializeField] protected AudioSource pickup_sound;

        public override void InteractAction()
        {
            player_interactor.GetDocumentsInteractor().PickUpDocument(document_index);

            //hide visible part
            foreach (GameObject obj in objects_to_hide) obj.SetActive(false);
            if (outline_controller != null) outline_controller.StopOutline();

            StartCoroutine("PickupActionDelay");
        }

        private IEnumerator PickupActionDelay()
        {
            pickup_sound.Play();
            yield return new WaitForSeconds(pickup_action_delay_time);
            PickupAction();
        }

        public virtual void PickupAction()
        {
            Debug.Log("Yeah, this document is secret");
        }
    }
}
