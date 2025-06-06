using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MicroScenesSystem;
using SceneObjectsInteractable;
using Scene3x3_Maze;
using UnityEngine.SceneManagement;

public class PlayerSceneInteractor : MonoBehaviour
{
    [Header("Switch scene animation")]
    [SerializeField] private GameObject switch_scene_animation_object;
    [SerializeField] private TextMeshProUGUI scene_name_text;
    [SerializeField] private string switch_scene_animation_name = "";
    [SerializeField] private float switch_scene_animation_time = 0f;
    [SerializeField] private string on_start_switch_scene_animation_name = "";
    [SerializeField] private float on_start_switch_scene_animation_time = 0f;
    [Space]

    [Header("Memetic effects")]
    [SerializeField] private Animator memetic_effect_blink_object;
    [SerializeField] private string memetic_effect_blink_anim_name;
    [SerializeField] private float memetic_effect_blink_anim_time;
    [SerializeField] private AudioSource memetic_effect_blink_audio;
    [Space]

    [Header("Other")]
    [SerializeField] private RaycastElement raycast;
    [SerializeField] private Canvas canvas;

    private MouseLookScript mouse_look_script;

    private void Start()
    {
        mouse_look_script = GetComponent<MouseLookScript>();
    }

    private void Update()
    {
        //activate scene switch trigger
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject obj = raycast.getElement();
            if (obj != null)
            {
                SceneSwitchTrigger component = obj.GetComponent<SceneSwitchTrigger>();
                if (component != null && component.ActivatesOnClick())
                {
                    component.Activate();
                }

                Activator maze_component = obj.GetComponent<Activator>();
                if(maze_component != null)
                {
                    Inventory player_inventory = GetComponent<Inventory>();
                    GunInventory player_gun_inventory = GetComponent<GunInventory>();
                    maze_component.StartProcess(player_inventory, player_gun_inventory);
                }

                SceneObjectInteractable interactable = obj.GetComponent<SceneObjectInteractable>();
                if(interactable != null)
                {
                    interactable.Interact(this);
                }
            }
        }

        //toggle canvas
        if (Input.GetKeyDown(KeyCode.H))
        {
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
            mouse_look_script.showFps = !mouse_look_script.showFps;
        }

        //toggle gun
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject current_gun = GetComponent<GunInventory>().currentGun;
            if(current_gun != null)
            {
                current_gun.SetActive(!current_gun.gameObject.activeSelf);
            }
        }
    }

    public float SwitchSceneAnimation(string scene_name, bool is_start = false)
    {
        scene_name_text.text = scene_name;
        switch_scene_animation_object.GetComponent<Animator>().Play(is_start ? on_start_switch_scene_animation_name : switch_scene_animation_name);
        return is_start ? switch_scene_animation_time : on_start_switch_scene_animation_time;
    }

    public PlayerDocumentsInteractor GetDocumentsInteractor()
    {
        return GetComponent<PlayerDocumentsInteractor>();
    }

    public float MemeticEffectBlink()
    {
        memetic_effect_blink_object.Play(memetic_effect_blink_anim_name);
        memetic_effect_blink_audio.Play();

        return memetic_effect_blink_anim_time;
    }

    private IEnumerator PlaySwitchSceneAnimation(bool is_start)
    {
        GetComponent<PlayerMovementScript>().enabled = false;

        switch_scene_animation_object.GetComponent<Animator>().Play(is_start ? on_start_switch_scene_animation_name : switch_scene_animation_name);
        yield return new WaitForSeconds(is_start ? switch_scene_animation_time: on_start_switch_scene_animation_time);

        GetComponent<PlayerMovementScript>().enabled = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
