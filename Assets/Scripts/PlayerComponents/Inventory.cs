using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private List<string> items_names_positions = new List<string>();
    [SerializeField] private RaycastElement raycast;
    [SerializeField] private UsableInventoryItem[] inventory_items_animators;
    [SerializeField] private float hide_pickup_delay;
    [Space]

    [Header("Items description")]
    [SerializeField] private List<string> items_gun_ammo_types = new List<string>();
    [SerializeField] private int medkit_item_index = 5;
    [SerializeField] private int pills_item_index = 4;
    [Space]

    [Header("UI elements")]
    [SerializeField] private GameObject inventory_ui;
    [SerializeField] private GameObject[] active_items_icons = { };
    [SerializeField] private GameObject[] inactive_items_icons = { };
    [SerializeField] private TextMeshProUGUI[] items_quantities_texts = { };
    [Space]

    [Header("Inventory state")]
    [SerializeField] private int[] items_quantities = { };
    [SerializeField] private int[] max_items_quantities = { };
    [Space]

    [Header("Sounds")]
    [SerializeField] private AudioSource open_inventory_sound;
    [SerializeField] private AudioSource close_inventory_sound;
    [SerializeField] private AudioSource full_inventory_sound;

    //state
    private bool is_picking_up = false;
    private bool is_healing = false;

    private List<int> picked_up_items_count;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        //show/hide UI
        if(Input.GetKeyDown(KeyCode.I) && !is_picking_up)
        {
            SwitchInventoryUI();
        }

        //pick-up item
        if(Input.GetKeyDown(KeyCode.E) && !is_picking_up)
        {
            GameObject obj = raycast.getElement();
            if(obj != null && obj.GetComponent<InventoryItem>() != null)
            {
                InventoryItem item = obj.GetComponent<InventoryItem>();

                int index = getTypeIndex(item.Type);

                //add only to max. amount
                if(items_quantities[index] < max_items_quantities[index])
                {
                    is_picking_up = true;

                    items_quantities[index] += item.Quantity;
                    if (items_quantities[index] > max_items_quantities[index])
                    {
                        items_quantities[index] = max_items_quantities[index];
                        full_inventory_sound.Play();
                    }

                    item.pickUp();
                    StartCoroutine("pickUpInventoryItem", index);
                    UpdateUI();

                    UpdatePickedUpItemsCount(index);
                }
                else
                {
                    full_inventory_sound.Play();
                }
            }
        }

        //use Medkit
        if(Input.GetKeyDown(KeyCode.F1) && !is_healing && this.GetComponent<HP.PlayerHealth>().CanUseHeal())
        {
            if(items_quantities[medkit_item_index] > 0)
            {
                Debug.Log("Used Medkit");
                StartCoroutine(useHealingItem(medkit_item_index, true));
            }
            else
            {
                Debug.Log("Out of medkits");
            }
        }

        //use Pills
        if(Input.GetKeyDown(KeyCode.F2) && !is_healing && this.GetComponent<HP.PlayerHealth>().CanUseHeal())
        {
            if (items_quantities[pills_item_index] > 0)
            {
                Debug.Log("Used Pills");
                StartCoroutine(useHealingItem(pills_item_index, false));
            }
            else
            {
                Debug.Log("Out of Pills");
            }
        }
    }

    private void SwitchInventoryUI()
    {
        inventory_ui.SetActive(!inventory_ui.activeSelf);

        if (inventory_ui.activeSelf)
        {
            open_inventory_sound.Play();
        }
        else
        {
            close_inventory_sound.Play();
        }
    }

    IEnumerator pickUpInventoryItem(int index)
    {
        UsableInventoryItem item = inventory_items_animators[index];

        //start gun hide animation
        GunInventory gun_inventory = GetComponent<GunInventory>();
        gun_inventory.getCurrentHandsAnimator().SetBool("changingWeapon", true);
        gun_inventory.getCurrentHandsAnimator().SetBool("sceneInteraction", true);
        yield return new WaitForSeconds(gun_inventory.getCurrentGun().GetComponent<GunScript>().takeDownAnimationTime);

        yield return new WaitForSeconds(hide_pickup_delay);

        //after gun is hidden, start item pick-up animation
        item.gameObject.SetActive(true);
        //is_picking_up = true;
        item.pickUp();

        //hide item
        yield return new WaitForSeconds(item.getPickUpAnimationTime());
        item.gameObject.SetActive(false);

        //take out gun
        gun_inventory.getCurrentHandsAnimator().SetBool("changingWeapon", false);
        gun_inventory.getCurrentHandsAnimator().SetBool("pickingUp", true);
        yield return new WaitForSeconds(gun_inventory.getCurrentGun().GetComponent<GunScript>().takeOutAnimationTime);
        gun_inventory.getCurrentHandsAnimator().SetBool("pickingUp", false);
        gun_inventory.getCurrentHandsAnimator().SetBool("sceneInteraction", false);

        is_picking_up = false;
    }

    IEnumerator useHealingItem(int index, bool immediate)
    {
        is_healing = true;

        //start gun hide animation
        GunInventory gun_inventory = GetComponent<GunInventory>();
        gun_inventory.getCurrentHandsAnimator().SetBool("changingWeapon", true);
        gun_inventory.getCurrentHandsAnimator().SetBool("sceneInteraction", true);
        yield return new WaitForSeconds(gun_inventory.getCurrentGun().GetComponent<GunScript>().takeDownAnimationTime);

        HP.PlayerHealth player_health = this.GetComponent<HP.PlayerHealth>();

        //start healing
        UsableInventoryItem item = inventory_items_animators[index];
        item.gameObject.SetActive(true);
        item.Use();

        //heal
        yield return new WaitForSeconds(item.HealthTime);
        if(player_health.CanBeHealed) // do not heal player after death
        {
            //restore some amount of hp immedietly
            if (immediate)
            {
                player_health.Heal(item.HealingAmount);
            }
            //start regeneration
            else
            {
                player_health.StartRegeneration(item.HealingAmount, item.HealthEffectTime);
            }
            yield return new WaitForSeconds(item.UsingTimeAmount - item.HealthTime);
            item.gameObject.SetActive(false);

            //reduce inventory
            items_quantities[index]--;

            //take out gun
            gun_inventory.getCurrentHandsAnimator().SetBool("changingWeapon", false);
            gun_inventory.getCurrentHandsAnimator().SetBool("pickingUp", true);
            yield return new WaitForSeconds(gun_inventory.getCurrentGun().GetComponent<GunScript>().takeOutAnimationTime);
            gun_inventory.getCurrentHandsAnimator().SetBool("pickingUp", false);
            gun_inventory.getCurrentHandsAnimator().SetBool("sceneInteraction", false);

            UpdateUI();

            is_healing = false;
        }
    }

    private int getTypeIndex(string type_name)
    {
        int item_index = items_names_positions.FindIndex(s => s.Contains(type_name));
        return item_index;
    }

    private void UpdatePickedUpItemsCount(int type_index)
    {
        if(picked_up_items_count == null)
        {
            picked_up_items_count = new List<int>();

            for (int i = 0; i < items_names_positions.Count; i++) picked_up_items_count.Add(0);
        }

        picked_up_items_count[type_index]++;
    }

    public void UpdateUI()
    {
        for(int i = 0; i< items_names_positions.Count; i++)
        {
            //update text
            if(items_quantities_texts[i] != null)
            {
                items_quantities_texts[i].text = items_quantities[i].ToString();
                items_quantities_texts[i].gameObject.SetActive(items_quantities[i] > 0);
            }

            //hide/show icon
            active_items_icons[i].SetActive(items_quantities[i] > 0);
            inactive_items_icons[i].SetActive(items_quantities[i] <= 0);
        }
    }

    //some items indexes refer to ammo type
    public int getGunAmmo(string ammo_type_name)
    {
        int item_index = items_gun_ammo_types.FindIndex(s => s.Contains(ammo_type_name));
        return items_quantities[item_index];
    }

    public void setGunAmmo(string ammo_type_name, int value)
    {
        int item_index = items_gun_ammo_types.FindIndex(s => s.Contains(ammo_type_name));
        items_quantities[item_index] = value;
        UpdateUI();
    }

    public UsableInventoryItem getItem(string item_name)
    {
        int index = getTypeIndex(item_name);

        return inventory_items_animators[index];
    }

    public int[] GetItemsQuantities()
    {
        return items_quantities;
    }

    public List<int> GetPickedUpItemsCount()
    {
        return picked_up_items_count != null ? picked_up_items_count : new List<int>();
    }

    public void ResetPickedUpItemsCount()
    {
        picked_up_items_count = null;
    }

    public void SetItemsQuantities(int[] new_items_quantities)
    {
        for(int i = 0; i < new_items_quantities.Length; i++)
        {
            items_quantities[i] = new_items_quantities[i] <= max_items_quantities[i]
                ? new_items_quantities[i]
                : max_items_quantities[i];
        }

        UpdateUI();
    }
}
