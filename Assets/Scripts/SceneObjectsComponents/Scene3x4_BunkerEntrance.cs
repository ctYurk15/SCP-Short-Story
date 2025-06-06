using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneObjectsInteractable
{
    public class Scene3x4_BunkerEntrance : Door
    {
        [SerializeField] private float open_animation_time;
        [SerializeField] private string scene_name;
        [SerializeField] private PlayerSceneInteractor scene_interactor;
        [SerializeField] private MicroScenesSystem.SceneController scene_controller;

        public override void InteractAction()
        {
            base.InteractAction();

            StartCoroutine("EndScene");
        }

        IEnumerator EndScene()
        {
            yield return new WaitForSeconds(open_animation_time);

            float switch_scene_effect = scene_interactor.SwitchSceneAnimation("End");
            yield return new WaitForSeconds(switch_scene_effect);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            scene_controller.SaveMetrics();

            SceneManager.LoadScene(scene_name);
        }
    }
}
