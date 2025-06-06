using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverOutlineController : MonoBehaviour
{
    [SerializeField] private float outline_selected_width;
    [SerializeField] private string player_raycast_tagname;
    [SerializeField] private Outline[] outline_objects;

    private bool outline_stopped = false;

    private void Update()
    {
        Outline outliner = GetComponent<Outline>();
        if (outliner != null || outline_objects.Length > 0)
        {
            GameObject player_raycast = GameObject.FindGameObjectWithTag(player_raycast_tagname);

            if (
                player_raycast != null
                && !outline_stopped
                && player_raycast.GetComponent<RaycastElement>().getElement() != null
                && player_raycast.GetComponent<RaycastElement>().getElement().transform.gameObject == this.transform.gameObject
            )
            {
                ShowOutline();
            }
            else HideOutline();

        }
    }

    private void OnMouseExit()
    {
        //HideOutline();
    }

    private void ShowOutline()
    {
        Outline outliner = GetComponent<Outline>();

        if (outliner != null)
        {
            outliner.enabled = true;
            outliner.OutlineWidth = outline_selected_width;
        }

        if (outline_objects.Length > 0)
        {
            foreach(Outline outline_object in outline_objects)
            {
                outline_object.enabled = true;
                outline_object.OutlineWidth = outline_selected_width;
            }
        }
    }

    private void HideOutline()
    {
        Outline outliner = GetComponent<Outline>();
        if (outliner != null)
        {
            outliner.OutlineWidth = 0;
        }

        if (outline_objects.Length > 0)
        {
            foreach (Outline outline_object in outline_objects)
            {
                outline_object.OutlineWidth = 0;
            }
        }
    }

    public void StopOutline()
    {
        outline_stopped = true;
        HideOutline();
    }

    public void StartOutline()
    {
        outline_stopped = false;
    }
}
