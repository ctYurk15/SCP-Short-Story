using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public float effect_time = 1f;

    public static void Create(SoundEffect prefab, Vector3 position)
    {
        GameObject new_sound_source = Instantiate(prefab.gameObject, position, Quaternion.identity);
        new_sound_source.GetComponent<SoundEffect>().PlayEffect();
    }

    public void PlayEffect()
    {
        StartCoroutine("PlaySoundEffect");
    }

    IEnumerator PlaySoundEffect()
    {
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(effect_time);
        Destroy(this.gameObject);
    }
}
