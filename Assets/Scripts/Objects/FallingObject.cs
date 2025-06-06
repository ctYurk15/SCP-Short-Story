using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FallingObject : MonoBehaviour
{
    public AudioSource default_hit_sound;
    public AudioSource sand_hit_sound;
    public AudioSource metallic_hit_sound;
    public AudioSource concrete_hit_sound;
    public AudioSource road_hit_sound;
    public AudioSource wood_hit_sound;
    public AudioSource glass_hit_sound;
    public AudioSource bricks_hit_sound;

    public string[] ignore_tags = { };

    private bool played_hit_sound = false;

    private void OnCollisionEnter(Collision collision)
    {
        string object_material = "";
        if (collision.transform.GetComponent<BulletHoleConfig>() != null)
        {
            object_material = collision.transform.GetComponent<BulletHoleConfig>().ElementMaterial;
        }

        //in order to avoid continious sound play
        if (!played_hit_sound && !ignore_tags.Contains(collision.transform.tag))
        {
            switch (object_material)
            {
                case "Sand":
                    sand_hit_sound.Play();
                    break;
                case "Metal":
                    metallic_hit_sound.Play();
                    break;
                case "Concrete":
                    concrete_hit_sound.Play();
                    break;
                case "Road":
                    road_hit_sound.Play();
                    break;
                case "Wood":
                    wood_hit_sound.Play();
                    break;
                case "Glass":
                    glass_hit_sound.Play();
                    break;
                case "Bricks":
                    bricks_hit_sound.Play();
                    break;
                default:
                    default_hit_sound.Play();
                    break;
            }

            played_hit_sound = true;
        }
        
    }
}
