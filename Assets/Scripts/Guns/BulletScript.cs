using UnityEngine;
using System.Collections;
using System.Linq;
using HP;

public class BulletScript : MonoBehaviour
{

    [Tooltip("Furthest distance bullet will look for target")]
    public float maxDistance = 1000000;
    RaycastHit hit;
    //[Tooltip("Prefab of wall damange hit")]
    //public GameObject decalHitWall;
    [Tooltip("Decal will need to be sligtly infront of the wall so it doesnt cause rendeing problems so for best feel put from 0.01-0.1.")]
    //public float floatInfrontOfWall;
    public float min_distance_from_wall = 0.02f;
    public float max_distance_from_wall = 0.04f;
   /* [Tooltip("Blood prefab particle this bullet will create upoon hitting enemy")]
    public GameObject bloodEffect;*/
    [Tooltip("Put Weapon layer and Player layer to ignore bullet raycast.")]
    public LayerMask ignoreLayer;
    [Tooltip("Impact force on rigid-bodies")]
    public float impactForce;
    [Tooltip("Damage")]
    public float damage;

    [Header("Bullet holes")]
    public GameObject wooden_bullet_hole;
    public GameObject metal_bullet_hole;
    public GameObject concrete_bullet_hole;
    public GameObject road_bullet_hole;
    public GameObject sand_bullet_hole;
    public GameObject glass_bullet_hole;
    public GameObject bricks_bullet_hole;
    public GameObject default_bullet_hole;
    public string[] tags_to_avoid = { };

    [Space]

    [Header("Hit Effects")]
    public GameObject wooden_hit_effect;
    public GameObject metal_hit_effect;
    public GameObject concrete_hit_effect;
    public GameObject road_hit_effect;
    public GameObject sand_hit_effect;
    public GameObject glass_hit_effect;
    public GameObject bricks_hit_effect;
    public GameObject default_hit_effect;

    [Space]

    [Header("Hit Sounds")]
    public GameObject wooden_hit_sound_effect;
    public GameObject metal_hit_sound_effect;
    public GameObject concrete_hit_sound_effect;
    public GameObject road_hit_sound_effect;
    public GameObject sand_hit_sound_effect;
    public GameObject glass_hit_sound_effect;
    public GameObject bricks_hit_sound_effect;
    public GameObject default_hit_sound_effect;

    /*
	* Uppon bullet creation with this script attatched,
	* bullet creates a raycast which searches for corresponding tags.
	* If raycast finds somethig it will create a decal of corresponding tag.
	*/
    void Update()
    {

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
        {
            //if (decalHitWall)
            {
                GameObject bullet_hole_object = null;
                GameObject hit_effect_object = null;
                GameObject hit_sound_effect_object = null;

                string object_material = "";
                if(hit.transform.GetComponent<BulletHoleConfig>() != null)
                {
                    object_material = hit.transform.GetComponent<BulletHoleConfig>().ElementMaterial;
                }

                if(!tags_to_avoid.Contains(hit.transform.tag))
                {
                    switch (object_material)
                    {
                        case "Wood":
                            bullet_hole_object = wooden_bullet_hole;
                            hit_effect_object = wooden_hit_effect;
                            hit_sound_effect_object = wooden_hit_sound_effect;
                            break;
                        case "Sand":
                            bullet_hole_object = sand_bullet_hole;
                            hit_effect_object = sand_hit_effect;
                            hit_sound_effect_object = sand_hit_sound_effect;
                            break;
                        case "Metal":
                            bullet_hole_object = metal_bullet_hole;
                            hit_effect_object = metal_hit_effect;
                            hit_sound_effect_object = metal_hit_sound_effect;
                            break;
                        case "Concrete":
                            bullet_hole_object = concrete_bullet_hole;
                            hit_effect_object = concrete_hit_effect;
                            hit_sound_effect_object = concrete_hit_sound_effect;
                            break;
                        case "Road":
                            bullet_hole_object = road_bullet_hole;
                            hit_effect_object = road_hit_effect;
                            hit_sound_effect_object = road_hit_sound_effect;
                            break;
                        case "Glass":
                            bullet_hole_object = glass_bullet_hole;
                            hit_effect_object = glass_hit_effect;
                            hit_sound_effect_object = glass_hit_sound_effect;
                            break;
                        case "Bricks":
                            bullet_hole_object = bricks_bullet_hole;
                            hit_effect_object = bricks_hit_effect;
                            hit_sound_effect_object = bricks_hit_sound_effect;
                            break;
                        default:
                            bullet_hole_object = default_bullet_hole;
                            hit_effect_object = default_hit_effect;
                            hit_sound_effect_object = default_hit_sound_effect;
                            break;
                    }

                    this.CreateHole(bullet_hole_object, hit);
                    this.CreateHitEffect(hit_effect_object, hit);
                    this.CreateHitSoundEffect(hit_sound_effect_object, hit);
                }

                this.OtherInteractions(hit);
                
            }
            Destroy(gameObject);
        }
        Destroy(gameObject, 0.1f);
    }

    private void CreateHole(GameObject bullet_hole_object, RaycastHit hit)
    {
        if(bullet_hole_object != null)
        {
            float distance_from_wall = Random.Range(min_distance_from_wall, max_distance_from_wall);
            Vector3 bullet_hole_position = hit.point + hit.normal * distance_from_wall;

            GameObject bullet_damage = Instantiate(bullet_hole_object, bullet_hole_position, Quaternion.LookRotation(hit.normal));

            bullet_damage.transform.SetParent(hit.transform);
            bullet_damage.GetComponent<RandomSkinSelector>().activate();
        }
    }

    private void CreateHitEffect(GameObject hit_effect_object, RaycastHit hit)
    {
        if(hit_effect_object != null)
        {
            Instantiate(hit_effect_object, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    private void CreateHitSoundEffect(GameObject hit_sound_effect_object, RaycastHit hit)
    {
        if (hit_sound_effect_object != null)
        {
            Instantiate(hit_sound_effect_object, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    private void OtherInteractions(RaycastHit hit)
    {
        if (hit.transform.GetComponent<Rigidbody>() != null)
        {
            hit.transform.GetComponent<Rigidbody>().AddForce(transform.forward * this.impactForce);
        }
        if (hit.transform.GetComponent<DestructibleProp>() != null)
        {
            hit.transform.GetComponent<DestructibleProp>().Damage(damage);
        }
        if (hit.transform.GetComponent<HPComponent>() != null)
        {
            hit.transform.GetComponent<HPComponent>().Damage(damage);
        }
    }

}