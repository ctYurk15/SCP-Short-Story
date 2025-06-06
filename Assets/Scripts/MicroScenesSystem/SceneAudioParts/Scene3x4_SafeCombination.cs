using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem.SceneAudioParts
{
    public class Scene3x4_SafeCombination : SceneAudioPart
    {
        [SerializeField] private SceneObjectsInteractable.Scene3x4_Safe safe;

        public override float Play()
        {
            StartCoroutine("UnlockSafe");
            return base.Play();
        }

        IEnumerator UnlockSafe()
        {
            yield return new WaitForSeconds(audio_clip.clip.length);
            safe.Activate();
        }
    }
}

