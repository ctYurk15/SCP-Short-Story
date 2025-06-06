using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneObjectsInteractable
{
    public class LoadMetricsScene : SceneObjectInteractable
    {
        [SerializeField] private string scene_name;
        [SerializeField] private bool unlock_cursor;
        [SerializeField] private bool save_metrics;
        [SerializeField] private MicroScenesSystem.SceneController scene_controller;

        public override void InteractAction()
        {
            scene_controller.SaveMetrics();

            Cursor.visible = unlock_cursor;
            Cursor.lockState = unlock_cursor ? CursorLockMode.None : CursorLockMode.Locked;

            SceneManager.LoadScene(scene_name);
        }
    }
}
