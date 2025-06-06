using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HologramsActivationsWin
{
    public class Scene3x1_FirstEncounter : HologramsActivationWin
    {
        [SerializeField] private MicroScenesSystem.SceneAudioPart[] audio_parts;
        [SerializeField] private MicroScenesSystem.SceneController scene_controller;
        
        private bool executed = false;

        public override void Execute()
        {
            if(!executed)
            {
                executed = true;

                foreach(MicroScenesSystem.SceneAudioPart part in audio_parts)
                {
                    scene_controller.AddAudioToQueue(part);
                }
            }
        }
    }
}
