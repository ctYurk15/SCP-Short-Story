using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDocumentsInteractor : MonoBehaviour
{
    [Header("Documents: ")]
    [SerializeField] private UsableDocument[] documents;
    [SerializeField] private float wait_before_pickup_document_time;

    public void PickUpDocument(int index)
    {
        StartCoroutine(PickUpAnimation(index));
    }

    private IEnumerator PickUpAnimation(int index)
    {
        GunInventory gun_inventory = GetComponent<GunInventory>();
        UsableDocument document = documents[index];

        float time_to_hide_gun = gun_inventory.TemporarlyHideGun(document.GetPickUpTime());
        yield return new WaitForSeconds(time_to_hide_gun + wait_before_pickup_document_time);

        document.transform.gameObject.SetActive(true);
        document.PickUp();
    }
}
