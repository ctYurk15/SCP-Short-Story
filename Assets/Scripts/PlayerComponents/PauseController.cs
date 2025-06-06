using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private MicroScenesSystem.SceneController scene_controller;
    [SerializeField] private GameObject pause_ui;
    [SerializeField] private GameObject are_you_sure_ui;
    [SerializeField] private string scene_menu_name;

    private bool is_pause = false;
    private bool confirmed_menu;
    private HP.PlayerHealth player_health;

    public bool isPause
    {
        get => is_pause;
    }

    private void Start()
    {
        player_health = GetComponent<HP.PlayerHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (player_health.CanBeHealed || is_pause))
        {
            SwitchPause();
        }
    }

    public void SwitchPause()
    {
        is_pause = !is_pause;

        //change time scale
        Time.timeScale = is_pause ? 0 : 1;

        //switch ui
        pause_ui.SetActive(is_pause);

        //enable / disable guns
        GetComponent<GunInventory>().ToggleGunUsable(!is_pause);

        //change cursor
        Cursor.visible = is_pause;
        Cursor.lockState = is_pause ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void RequestMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        are_you_sure_ui.SetActive(true);
    }

    public void HideRequestMenu()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        are_you_sure_ui.SetActive(false);
    }

    public void Menu()
    {
        scene_controller.DataClear();

        Time.timeScale = 1;

        SceneManager.LoadScene(scene_menu_name);
    }

    //buttons functions
    public void Switch(GameObject p)
    {
        p.SetActive(!p.activeSelf);
    }
}
