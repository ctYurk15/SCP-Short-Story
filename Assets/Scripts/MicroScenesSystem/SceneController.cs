using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem
{
    public class SceneController : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private GameObject[] scenes;
        [SerializeField] private Transform[] scenes_positions;
        [SerializeField] private string[] scenes_names;
        [SerializeField] private bool[] scenes_light_disabled;
        [SerializeField] private int start_scene = 0;
        [SerializeField] private float delay_between_switch = 0;
        [Space]

        /*[Header("Audio")]
        [SerializeField] private float delay_between_audio;
        [Space]*/

        [Header("Other")]
        [SerializeField] private GameObject player;
        [SerializeField] private new GameObject light;

        //state
        private int current_scene = 0;
        private Queue<SceneAudioPart> scene_audio_parts = new Queue<SceneAudioPart>();
        private bool is_processing_audio_queue = false;
        private SceneAudioPart current_audio_part;
        private SceneProgressSave progress_save;
        private CustomTimer timer;

        public int CurrentScene
        {
            get => current_scene;
        }

        private void Awake()
        {
            //progress_save = GetComponent<SceneProgressSave>();
            timer = GetComponent<CustomTimer>();
        }

        private void Start()
        {
            int last_scene = SceneProgressSave.GetSceneNumber();

            SceneProgressSave.LoadPlayerProgress(player);

            timer.StartCount();

            SwitchScene(last_scene != -1 ? last_scene : start_scene, true);
        }

        public void SwitchScene(int scene_number, bool is_start = false)
        {
            StopAudioQueue();

            if (!is_start)
            {
                // for progress, we save next scene, because we need to load progress
                SceneProgressSave.SaveProgress(player, scene_number);

                // for metrics, we save current scene, because we need to save current scene metrics
                SaveMetrics();

                // reset metrics tmp array
                player.GetComponent<Inventory>().ResetPickedUpItemsCount();
            }

            StopCoroutine("ProcessQueue");
            StartCoroutine(PlaySwitchScene(scene_number, is_start));
        }

        public void TriggerProcessQueue()
        {
            StopCoroutine("ProcessQueue");
        }

        public void AddAudioToQueue(SceneAudioPart part, bool clear_queue = false)
        {
            if (clear_queue)
            {
                StopAudioQueue();
                StopCoroutine("ProcessQueue");
            }

            scene_audio_parts.Enqueue(part);

            if(!is_processing_audio_queue) StartCoroutine("ProcessQueue");
        }

        private void StopAudioQueue()
        {
            scene_audio_parts.Clear();
            if (current_audio_part != null) current_audio_part.Stop();
            is_processing_audio_queue = false;
        }

        private IEnumerator ProcessQueue()
        {
            is_processing_audio_queue = true;

            current_audio_part = scene_audio_parts.Dequeue();
            SceneAudioPart tmp = current_audio_part;
            float audio_part_time = current_audio_part.Play();
            
            yield return new WaitForSeconds(audio_part_time);

            if (scene_audio_parts.Count > 0) StartCoroutine("ProcessQueue");
            else is_processing_audio_queue = false;
        }

        private IEnumerator PlaySwitchScene(int scene_number, bool is_start)
        {
            player.GetComponent<PlayerSceneInteractor>().SwitchSceneAnimation(scenes_names[scene_number], is_start);

            yield return new WaitForSeconds(delay_between_switch);

            foreach (GameObject scene in scenes)
            {
                scene.SetActive(false);
            }

            scenes[scene_number].SetActive(true);
            player.transform.position = scenes_positions[scene_number].position;

            light.SetActive(!scenes_light_disabled[scene_number]);

            current_scene = scene_number;

            timer.StartCount(); // calculate time only after scene loaded
        }

        public string getPlayerTag()
        {
            return player.transform.tag;
        }

        public void SaveMetrics()
        {
            timer.StopCount();
            PlayerMetricsSave.SaveSceneEndMetrics(player, current_scene, timer.TotalSeconds);
        }

        public void DataClear()
        {
            SceneProgressSave.Clear();
            PlayerMetricsSave.Clear();
        }
    }
}