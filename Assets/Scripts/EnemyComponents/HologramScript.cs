using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Brains;

public class HologramScript : MonoBehaviour
{
    //[SerializeField] private GameObject[] points_to_move;
    [Header("Patrol")]
    [SerializeField] private Vector3 patrol_points_center;
    [SerializeField] private float patrol_points_range;
    [SerializeField] private float min_distance_for_point = 0.1f;
    [SerializeField] private float min_distance_from_leader;
    [SerializeField] private float max_distance_from_leader;
    [Range(0f, 1f)]
    [SerializeField] private float patrol_idle_probability;
    [SerializeField] private float patrol_idle_duration;
    [Space]

    [Header("Animations & movement")]
    [SerializeField] private string[] move_animations;
    [SerializeField] private float[] move_animations_duration;
    [SerializeField] private float[] move_animations_speed_multiplier;
    [SerializeField] private string invisible_animation_name;
    [SerializeField] private float initial_speed = 4;
    [SerializeField] private float walk_animation_speed = 5;
    [SerializeField] private SoundEffect teleport_effect;
    [Space]

    [Header("Animations & damage")]
    [SerializeField] private string damage_animation_name;
    [SerializeField] private float damage_animation_time;
    [SerializeField] private float damage_amount;
    [SerializeField] private float min_distance_for_damage = 1;
    [SerializeField] private float damage_cooldown;
    [SerializeField] private SoundEffect damage_effect;
    [Space]

    [Header("Retreating")]
    [SerializeField] private bool retreat_enabled = false;
    [SerializeField] private Vector3 retreat_center;
    [SerializeField] private float retreat_range;
    [SerializeField] private float retreat_min_distance;
    [SerializeField] private float retreat_speed_multiplier = 1;
    [Space]

    [Header("Other")]
    [SerializeField] private GameObject player;
    [SerializeField] private bool is_leader;
    public bool isLeader {
        get { return is_leader; }
        //set { is_leader = value; }
    }

    private NavMeshAgent agent;
    private Animator animator;
    private FieldOfView fov;
    private HologramBrain hologram_brain;

    [SerializeField] private Vector3 current_point;
    private int current_animation = 0;

    private bool is_damaging = false;
    private bool damage_cool_down = false;
    private bool is_invisible = false;
    private bool is_patrol_idle;

    private Vector3 retreat_position;
    private bool is_retreating = false;

    [SerializeField] private Vector3 last_player_position;
    [SerializeField] private Vector3 last_player_position_on_damage; //when player damaged hologram
    [SerializeField] private Vector3 tmp_last_player_position;
    [SerializeField] private bool can_see_player_g = false;
    [SerializeField] private bool can_reach_player_g = false;

    private float leader_speed_multiplied = 0;

    private bool is_dead = false;

    private void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();
        this.animator = GetComponent<Animator>();
        this.fov = GetComponent<FieldOfView>();
        this.hologram_brain = FindObjectOfType<HologramBrain>();

        this.fov.playerRef = this.player;
        this.animator.SetFloat("walk_speed", walk_animation_speed);

        StartCoroutine("Animation");
    }

    private void Update()
    {
        //store player postion withot y, so distance would be calculated in x-z coordinates
        Vector3 aligned_player_position = this.alignPosition(player.transform.position);

        NavMeshHit hit;
        bool can_reach_player = false;
        if(this.fov.canSeePlayer || this.hologram_brain.SomeoneSeesPlayer())
        {
            can_reach_player = NavMesh.SamplePosition(aligned_player_position, out hit, 1.0f, NavMesh.AllAreas);
        }
        else
        {
            can_reach_player = NavMesh.SamplePosition(last_player_position, out hit, 1.0f, NavMesh.AllAreas);
        }
        this.can_see_player_g = this.fov.canSeePlayer;
        this.can_reach_player_g = can_reach_player;


        //go for player
        if (can_reach_player && this.fov.canSeePlayer && !is_retreating && !is_damaging)
        {
            last_player_position = aligned_player_position;

            this.hologram_brain.notifyAboutPlayer(player.transform.position);
            this.agent.SetDestination(player.transform.position);
        }
        //check last player position before patrol
        else if (can_reach_player && !this.fov.canSeePlayer && last_player_position != Vector3.zero && !is_retreating && !is_damaging)
        {
            if (Vector3.Distance(last_player_position, this.transform.position) <= min_distance_for_point) 
            {
                last_player_position = Vector3.zero;
            }
            else this.agent.SetDestination(last_player_position);
        }
        //retreat away
        else if(is_retreating)
        {
            last_player_position = Vector3.zero;
            if (Vector3.Distance(retreat_position, this.transform.position) <= min_distance_for_point)
            {
                stopRetreat();
            }
            else
            {
                this.agent.SetDestination(retreat_position);
            }
        }
        //return to patrol
        else if(!is_damaging)
        {
            last_player_position = Vector3.zero;

            //hologram arrived to patrol point
            if (Vector3.Distance(current_point, transform.position) <= min_distance_for_point || current_point == Vector3.zero)
            {
                float random_number = Random.Range(0f, 100f) / 100f;
                if(random_number < patrol_idle_probability && !is_patrol_idle)
                {
                    StartCoroutine("IDLEPatrol");
                }
                else
                {
                    current_point = GetRandomPatrolPoint();
                }
            }

            this.agent.SetDestination(current_point);
        }

        //check for player 'collision'
        bool on_damage_distance = Vector3.Distance(player.transform.position, this.transform.position) <= min_distance_for_damage;
        if (
             on_damage_distance
            && !is_damaging 
            && !damage_cool_down
            && !is_invisible
            && !is_retreating
            && !is_dead
        )
        {
            is_damaging = true;
            damage_cool_down = true;
            StartCoroutine("Damage");
        }

        //hologram should not move, if can damage, even if is on damage distance
        if (
             on_damage_distance
            && !is_retreating
        )
        {
            agent.speed = 0;
            agent.velocity = Vector3.zero;
        }
    }

    private Vector3 RandomRetreatPoint(Vector3 center, float range)
    {
        Vector3 result = Vector3.zero;

        while (true)
        {
            Vector3 point = RandomPoint(center, range); 
            if (Vector3.Distance(player.transform.position, point) > retreat_min_distance)
            {
                result = point;
                break;
            }
        }

        return result;
    }

    private Vector3 RandomPoint(Vector3 center, float range)
    {
        Vector3 result = Vector3.zero;

        while (true)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                break;
            }
        }

        return result;
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector3 point;
        HologramScript leader = FindLeader();

        while (true)
        {
            point = RandomPoint(patrol_points_center, patrol_points_range);
            if(leader == null)
            {
                break;
            }
            else
            {
                float distance_from_leader = Vector3.Distance(point, leader.gameObject.transform.position);
                if (
                    (distance_from_leader >= min_distance_for_damage && distance_from_leader <= max_distance_from_leader)
                    // if leader is out of range of patrol, do not follow him
                    || Vector3.Distance(patrol_points_center, leader.transform.position) > patrol_points_range)
                {
                    break;
                }
            }
        }

        return point;
    }

    //make y coordinate same as hologram to avoid distance calculation bugs
    private Vector3 alignPosition(Vector3 position)
    {
        return new Vector3(position.x, transform.position.y, position.z);
    }

    private void multiplyMoveAnimationsSpeed(float multiplier)
    {
        for (int i = 0; i < move_animations.Length; i++)
        {
            move_animations_speed_multiplier[i] *= multiplier;
        }
    }

    private void startRetreat()
    {
        is_retreating = true;
        tmp_last_player_position = last_player_position;
        retreat_position = this.RandomRetreatPoint(retreat_center, retreat_range);
    }

    private void stopRetreat()
    {
        is_retreating = false;
        last_player_position = tmp_last_player_position;
        tmp_last_player_position = Vector3.zero;
    }

    private IEnumerator Damage()
    {
        StopCoroutine("Animation");

        animator.Play(damage_animation_name);
        player.GetComponent<HP.HPComponent>().Damage(damage_amount);
        SoundEffect.Create(damage_effect, transform.position);
        
        yield return new WaitForSeconds(damage_animation_time);

        is_damaging = false;

        if(retreat_enabled) startRetreat();

        if(!is_dead)
        {
            StartCoroutine("Animation");
            StartCoroutine("CoolDownDamage");
        }
    }

    private IEnumerator CoolDownDamage()
    {
        yield return new WaitForSeconds(damage_cooldown - damage_animation_time);
        damage_cool_down = false;
    }

    private IEnumerator Animation()
    {
        if (current_animation >= move_animations.Length) current_animation = 0;

        string animation_name = move_animations[current_animation];
        float animation_time = move_animations_duration[current_animation];
        float speed_multiplier = move_animations_speed_multiplier[current_animation];

        is_invisible = animation_name == invisible_animation_name;
        
        animator.Play(animation_name);

        float new_speed = initial_speed * speed_multiplier;
        if (is_retreating) new_speed *= retreat_speed_multiplier;
        agent.speed = initial_speed * speed_multiplier;

        if(is_invisible) StartCoroutine(DelayedSoundEffect(teleport_effect, animation_time));

        if (speed_multiplier == 0) agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(animation_time);

        current_animation++;
        if (!is_damaging && !is_dead && !is_patrol_idle)
        {
            StartCoroutine("Animation");
        }
    }

    private IEnumerator IDLEPatrol()
    {
        is_patrol_idle = true;
        animator.speed = 0;
        agent.speed = 0;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(patrol_idle_duration);
        animator.speed = 1;

        is_patrol_idle = false;
        StartCoroutine("Animation");
    }

    private IEnumerator DelayedSoundEffect(SoundEffect sound_effect, float delay_time)
    {
        yield return new WaitForSeconds(delay_time);
        SoundEffect.Create(sound_effect, transform.position);
    }

    public void Die()
    {
        is_dead = true;
        agent.speed = 0;
    }

    public HologramScript FindLeader()
    {
        HologramScript[] holograms = FindObjectsOfType<HologramScript>();
        HologramScript result = null;

        foreach (HologramScript hologram in holograms)
        {
            if (hologram.isLeader)
            {
                result = hologram;
            }
        }

        return result;
    }

    public void MakeLeader(
        float hp_multiplier, 
        float leader_speed_multiplier,
        float leader_fov_angle_multiplier,
        float leader_fov_distance_multiplier
    )
    {
        if(!this.is_leader)
        {
            this.is_leader = true;

            //multiple hp
            this.GetComponent<HP.HologramHPComponent>().UpgradeHP(hp_multiplier);

            //multiply all animations agent speed
            this.multiplyMoveAnimationsSpeed(leader_speed_multiplier);

            //increase animations play speed
            this.animator.SetFloat("walk_speed", walk_animation_speed * leader_speed_multiplier);

            //multiply vision
            this.fov.horizontalAngle *= leader_fov_angle_multiplier;
            this.fov.radius *= leader_fov_distance_multiplier;

            //store values for further usage
            this.leader_speed_multiplied = leader_speed_multiplier;
        }
    }

    public void notifyAboutPlayer(Vector3 new_position)
    {
        if (last_player_position == Vector3.zero && !is_retreating) last_player_position = new_position;
    }

    public void damaged()
    {
        Vector3 aligned_player_pos = this.alignPosition(player.transform.position);

        this.last_player_position_on_damage = aligned_player_pos;
        this.notifyAboutPlayer(aligned_player_pos);
    }

    public bool isInvisible()
    {
        return is_invisible;
    }

    public void ChangePatrolConfig(Vector3 center, float range)
    {
        this.patrol_points_center = center;
        this.patrol_points_range = range;
    }

    public bool CanSeePlayer()
    {
        return this.fov.canSeePlayer ;
    }
}
