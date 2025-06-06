using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene3x3_Maze
{
    public class Activator : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource[] audio_parts;
        [Space]

        [Header("Activation")]
        [SerializeField] private float max_distance_to_activate;
        [SerializeField] private float outline_selected_width;
        [SerializeField] private string card_inventory_item_name;
        [SerializeField] private HoverOutlineController outline_controller;
        [Space]

        [Header("Objects")]
        //[SerializeField] private GameObject disable_object;
        [SerializeField] private GameObject enable_object;

        private int current_audio_index = 0;
        private bool activated = false;

        private void Start()
        {
            StartCoroutine("PlayAudio");
        }

        public void StartProcess(Inventory player_inventory, GunInventory player_gun_inventory)
        {
            if (!activated)
            {
                activated = true;
                StartCoroutine(Activation(player_inventory, player_gun_inventory));
            }
        }

        IEnumerator PlayAudio()
        {
            if (current_audio_index < audio_parts.Length)
            {
                AudioSource audio = audio_parts[current_audio_index];
                audio.Play();
                yield return new WaitForSeconds(audio.clip.length);

                current_audio_index++;
                StartCoroutine("PlayAudio");
            }
        }

        IEnumerator Activation(Inventory player_inventory, GunInventory player_gun_inventory)
        {
            //start gun hide animation
            /* player_gun_inventory.getCurrentHandsAnimator().SetBool("changingWeapon", true);
             yield return new WaitForSeconds(player_gun_inventory.getCurrentGun().GetComponent<GunScript>().takeDownAnimationTime);*/

            //get card object
            UsableInventoryItem card = player_inventory.getItem(card_inventory_item_name);
            float usage_time = card.getUsageTime();
            float hiding_time = card.getUsageTime();

            //hide gun while using card
            float time_to_hide_gun = player_gun_inventory.TemporarlyHideGun(usage_time + hiding_time);
            yield return new WaitForSeconds(time_to_hide_gun);

            //take out card
            card.gameObject.SetActive(true);
            card.Use();
            yield return new WaitForSeconds(usage_time);

            //create doors
            //disable_object.SetActive(false);
            enable_object.SetActive(true);
            this.GetComponent<Collider>().enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;

            //hide card
            yield return new WaitForSeconds(hiding_time);
            card.gameObject.SetActive(false);
            outline_controller.StopOutline();

            //take out gun
           /* player_gun_inventory.getCurrentHandsAnimator().SetBool("changingWeapon", false);
            player_gun_inventory.getCurrentHandsAnimator().SetBool("pickingUp", true);
            yield return new WaitForSeconds(player_gun_inventory.getCurrentGun().GetComponent<GunScript>().takeOutAnimationTime);
            player_gun_inventory.getCurrentHandsAnimator().SetBool("pickingUp", false);*/
        }
    }
}
