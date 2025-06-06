using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneObjectsInteractable
{
    public class CardAccessDoor : SceneObjectInteractable
    {
        [Header("Activation")]
        [SerializeField] private string card_inventory_item_name;
        [SerializeField] private GameObject enable_object;

        public override void InteractAction()
        {
            Inventory player_inventory = this.player_interactor.GetComponent<Inventory>();
            GunInventory player_gun_inventory = this.player_interactor.GetComponent<GunInventory>();
            StartCoroutine(Activation(player_inventory, player_gun_inventory));
        }

        IEnumerator Activation(Inventory player_inventory, GunInventory player_gun_inventory)
        {
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
        }
    }
}
