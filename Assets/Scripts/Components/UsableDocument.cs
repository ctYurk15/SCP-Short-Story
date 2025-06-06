using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableDocument : MonoBehaviour
{
    [SerializeField] private string pick_up_animation_name;
    [SerializeField] private float pick_up_animation_time;

    public void PickUp()
    {
        StartCoroutine("PickUpAnim");
    }

    public float GetPickUpTime()
    {
        return pick_up_animation_time;
    }

    private IEnumerator PickUpAnim()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play(pick_up_animation_name);

        yield return new WaitForSeconds(pick_up_animation_time);

        this.gameObject.SetActive(false);
    }
}
