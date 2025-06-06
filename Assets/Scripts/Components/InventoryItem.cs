using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [Header("Item settings")]
    [SerializeField] private string type = "";
    [SerializeField] private int quantity = 0;
    [Space]

    [Header("Outline settings")]
    [SerializeField] private float outline_selected_width = 3;

    public string Type{ get => type;}
    public int Quantity{ get => quantity; }

    private void OnMouseOver()
    {
        Outline outliner = GetComponent<Outline>();
        if (outliner != null)
        {
            GameObject player_raycast = GameObject.FindGameObjectWithTag("PlayerRaycast");
            if(player_raycast != null && Vector3.Distance(player_raycast.transform.position, transform.position) <= player_raycast.GetComponent<RaycastElement>().getMaxDistance())
            {
                outliner.OutlineWidth = outline_selected_width;
            }
            
        }
    }

    private void OnMouseExit()
    {
        Outline outliner = GetComponent<Outline>();
        if (outliner != null) outliner.OutlineWidth = 0;
    }

    public void pickUp()
    {
        Destroy(this.gameObject);
    }
}
