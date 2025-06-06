using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene3x3_Maze
{
    public class FinalDocument : SceneObjectsInteractable.Document
    {
        [SerializeField] private Transform teleport_point;
        [SerializeField] private PlayerDocumentsInteractor player;
        [SerializeField] private SceneObjectsInteractable.Door door;
        [SerializeField] private AudioSource[] talk_audio_parts;
        [SerializeField] private float delay_until_switch_scene;
        [SerializeField] private MicroScenesSystem.SceneController scenes_controller;
        [SerializeField] private int scene_index;

        private int current_talk_audio_index = 0;

        public override void PickupAction()
        {
            PlayerSceneInteractor scene_interactor = player.GetComponent<PlayerSceneInteractor>();
            float time_to_blink = scene_interactor.MemeticEffectBlink();

            player.transform.position = teleport_point.position;

            door.ResetState();
            door.Deactivate();

            StartCoroutine("PlayTalkAudio", time_to_blink);
        }

        private IEnumerator PlayTalkAudio(float wait_to_start_time = 0)
        {
            if (current_talk_audio_index < talk_audio_parts.Length)
            {
                if (wait_to_start_time > 0) yield return new WaitForSeconds(wait_to_start_time);

                AudioSource audio_part = talk_audio_parts[current_talk_audio_index];
                audio_part.Play();

                yield return new WaitForSeconds(audio_part.clip.length);
                current_talk_audio_index++;
                StartCoroutine("PlayTalkAudio", 0);
            }
            else
            {
                yield return new WaitForSeconds(delay_until_switch_scene);
                scenes_controller.SwitchScene(scene_index);
            }
        }
    }
}
