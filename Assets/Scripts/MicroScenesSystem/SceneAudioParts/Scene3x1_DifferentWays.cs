using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem.SceneAudioParts
{
    public class Scene3x1_DifferentWays : SceneAudioPart
    {
        [SerializeField] private AudioSource left_path_audio;
        [SerializeField] private AudioSource right_path_audio;
        [SerializeField] private Scene3x1_PathsMixer paths_mixer;

        public override float Play()
        {
            this.audio_clip = paths_mixer.safePathIsLeft() ? left_path_audio : right_path_audio;
            return base.Play();
        }
    }
}
