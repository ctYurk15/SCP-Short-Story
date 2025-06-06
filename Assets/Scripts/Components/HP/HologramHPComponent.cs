using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HP
{
    public class HologramHPComponent : HPComponent
    {
        [Header("Damage:")]
        public SkinnedMeshRenderer[] healthy_parts;
        public SkinnedMeshRenderer[] damaged_parts;
        public float damage_effect_time;
        public SoundEffect damaged_sound;

        private HologramScript hologram_script;

        private void Start()
        {
            this.hologram_script = GetComponent<HologramScript>();
        }

        public void UpgradeHP(float multiplier)
        {
            this.hp *= multiplier;
        }

        public override void Damage(float amount)
        {
            if(!hologram_script.isInvisible())
            {
                base.Damage(amount);
                this.hologram_script.damaged();
                StartCoroutine("DamageEffect");
            }
        }

        private IEnumerator DamageEffect()
        {
            SoundEffect.Create(damaged_sound, transform.position);
            SwitchMeshes(healthy_parts, false);
            SwitchMeshes(damaged_parts, true);

            yield return new WaitForSeconds(damage_effect_time);

            SwitchMeshes(healthy_parts, true);
            SwitchMeshes(damaged_parts, false);
        }

        private void SwitchMeshes(SkinnedMeshRenderer[] array, bool active)
        {
            foreach (SkinnedMeshRenderer mesh in array)
            {
                if(mesh != null)
                {
                    mesh.transform.gameObject.SetActive(active);
                }
            }
        }
    }
}
