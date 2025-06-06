using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkinSelector : MonoBehaviour
{
    public GameObject[] skins;
    public bool start_on_awake = false;

    private void Awake()
    {
        if(start_on_awake)
        {
            activate();
        }
    }

    public GameObject activate()
    {
        for(int i = 0; i < skins.Length; i++) this.skins[i].SetActive(false);
        int index = UnityEngine.Random.Range(0, skins.Length);

        this.skins[index].SetActive(true);
        return this.skins[index];
    }

}
