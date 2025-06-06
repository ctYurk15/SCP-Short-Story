using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem.SceneAudioParts
{
    public class Scene3x1_AskForWay : SceneAudioPart
    {
        [SerializeField] private AudioSource win_audio;
        [SerializeField] private AudioSource evade_audio;
        [SerializeField] private HologramsGroupsActivator holograms_activator;

        public override float Play()
        {
            this.audio_clip = holograms_activator.HologramsGroupAlive() ? evade_audio : win_audio;
            return base.Play();
        }
    }
}
