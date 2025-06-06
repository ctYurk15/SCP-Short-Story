using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem
{
    public class SceneAudioPart : MonoBehaviour
    {
        [SerializeField] protected SceneController scene_switcher;
        [SerializeField] protected bool stop_previous_audio;

        protected AudioSource audio_clip;

        //state
        protected bool is_activated = false;

        private void Start()
        {
            audio_clip = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == scene_switcher.getPlayerTag() && !is_activated)
            {
                is_activated = true;
                scene_switcher.AddAudioToQueue(this, stop_previous_audio);
            }
        }

        public virtual float Play()
        {
            this.audio_clip.Play();
            return audio_clip.clip.length;
        }

        public void Stop()
        {
            this.audio_clip.Stop();
        }
    }

}
