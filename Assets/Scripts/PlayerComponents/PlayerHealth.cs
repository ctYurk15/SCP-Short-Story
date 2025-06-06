using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CameraBleedEffect;

namespace HP
{
    public class PlayerHealth : HP.HPComponent
    {
        [Header("Basic stats")]
        [SerializeField] private float max_hp = 100;
        [SerializeField] private TextMeshProUGUI hp_text;
        [Space]

        [Header("Blood drops settings")]
        [SerializeField] private float damageBloodAmount = 3; //amount of blood when taking damage (relative to damage taken (relative to HP remaining))
        [SerializeField] private float maxBloodIndication = 0.5f; //max amount of blood when not taking damage (relative to HP lost)
        [Space]

        [Header("Pills effect")]
        [SerializeField] private Image pillsBlurImage;
        [SerializeField] private string pillsBlurUpAnimationName;
        [SerializeField] private string pillsBlurDownAnimationName;
        [Space]

        [Header("Health blink")]
        [SerializeField] private Image healthBlinkImage;
        [SerializeField] private string healthBlinkAnimationName;
        [SerializeField] private float healthBlinkAnimationTime;
        [SerializeField] private float minHealthBlinkHP = 40;
        [SerializeField] private float healthBlinkFrequency = 3;
        [Space]

        [Header("Death")]
        [SerializeField] private Image deathBlurImage;
        [SerializeField] private string deathBlur1AnimationName;
        [SerializeField] private float deathBlur1AnimationTime;
        [SerializeField] private string deathBlur2AnimationName;
        [SerializeField] private float deathBlur2AnimationTime;
        [SerializeField] private GameObject aliveCamera;
        [SerializeField] private GameObject deathCamera;
        [SerializeField] private float deathInitialTimeSlowAmount = 0.5f;
        [SerializeField] private float minDeathUITime = 0.5f;
        [SerializeField] private float maxDeathUITime = 2;
        [SerializeField] private GameObject deathUI;
        [SerializeField] private MicroScenesSystem.SceneController scene_controller;
        [SerializeField] private float coyote_time_for_death;

        public float HP
        {
            get { return hp; }
            set
            {
                hp = value <= max_hp ? value : max_hp;
                UpdateUI();
            }
        }

        public bool CanBeHealed
        {
            get { return can_be_healed; }
        }

        //state
        private float pain_killed_hp = 0;
        private bool is_health_blinking = false;
        private bool is_pills_effect = false;
        private bool death_started = false;
        private bool can_be_healed = true;

        private void Start()
        {
            CustomBleedBehaviour.BloodAmount = 0;
            CustomBleedBehaviour.minBloodAmount = 0;
        }

        private void Update()
        {
            if (this.hp <= this.minHealthBlinkHP && can_be_healed)
            {
                if (!is_health_blinking)
                {
                    StartCoroutine("HealthBlink");
                }
            }
            else
            {
                is_health_blinking = false;
            }
        }

        public override void Damage(float amount)
        {
            this.ChangeHP(-amount, false);
        }

        public void Damage(float amount, bool skip_blood_anim = false)
        {
            this.ChangeHP(-amount, skip_blood_anim);
        }

        public void Heal(float amount, bool skip_pain_heal = false)
        {
            if (this.pain_killed_hp > 0 && !skip_pain_heal)
            {
                if (this.pain_killed_hp > amount)
                {
                    this.pain_killed_hp -= amount;
                    amount = 0;
                }
                else
                {
                    float healed_amount = amount - this.pain_killed_hp;
                    this.pain_killed_hp = 0;
                    amount -= healed_amount;
                }
            }
            this.ChangeHP(amount);
        }

        public void StartRegeneration(float healing_amount, float effect_time)
        {
            StartCoroutine(Regeneration(healing_amount, effect_time));
        }

        public void DeathCameraFall()
        {
            StartCoroutine("ContinueDeath");
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public bool CanUseHeal()
        {
            return this.hp - this.pain_killed_hp != this.max_hp;
        }

        private void ChangeHP(float amount, bool skip_blood_anim = false)
        {
            this.hp += amount;
            if (this.hp <= 0) this.hp = 0;
            else if (this.hp > max_hp) this.hp = max_hp;

            if (amount < 0 && !skip_blood_anim)
            {
                CustomBleedBehaviour.BloodAmount += Mathf.Clamp01(damageBloodAmount * (amount * -1) / this.hp);
                //transform.GetComponent<SimplePlayer>().Damage((int)amount * -1);
            }

            this.UpdateUI();

            if (this.hp == 0 && !death_started)
            {
                death_started = true;
                StartCoroutine("StartDeath");
            }
        }

        IEnumerator Regeneration(float healing_amount, float effect_time)
        {
            pillsBlurImage.transform.GetComponent<Animator>().Play(pillsBlurUpAnimationName);
            is_pills_effect = true;

            float hp_per_second = healing_amount / effect_time;

            //each second, add small amount of health
            while (effect_time > 0)
            {
                yield return new WaitForSeconds(1);
                effect_time--;

                this.Heal(hp_per_second);
            }

            pillsBlurImage.transform.GetComponent<Animator>().Play(pillsBlurDownAnimationName);
            is_pills_effect = false;
        }

        private void UpdateUI()
        {
            hp_text.text = "HP: " + this.hp;

            CustomBleedBehaviour.minBloodAmount = maxBloodIndication * (this.max_hp - this.hp) / this.max_hp;
        }

        private void StopEffects()
        {
            if (is_pills_effect)
            {
                StopCoroutine("Regeneration");
                pillsBlurImage.transform.GetComponent<Animator>().Play(pillsBlurDownAnimationName);
            }

            StopCoroutine("HealthBlink");
        }

        IEnumerator HealthBlink()
        {
            is_health_blinking = true;

            yield return new WaitForSeconds(healthBlinkFrequency);
            healthBlinkImage.transform.GetComponent<Animator>().Play(healthBlinkAnimationName);
            yield return new WaitForSeconds(healthBlinkAnimationTime);

            is_health_blinking = false;
        }

        IEnumerator StartDeath()
        {
            //wait coyote time to give player a chance for health restore before death
            yield return new WaitForSeconds(coyote_time_for_death);

            //continue death process. cast to int is required, so pill won't give immunity for fog
            if((int) this.hp <= 0)
            {
                can_be_healed = false; //prevent heal after death started

                StopEffects();

                //disable components
                this.GetComponent<PlayerMovementScript>().enabled = false;
                this.GetComponent<MouseLookScript>().enabled = false;
                this.GetComponent<GunInventory>().enabled = false;
                this.GetComponent<Inventory>().enabled = false;

                //destroy gun
                Destroy(this.GetComponent<GunInventory>().currentGun.gameObject);

                //start bluring screen
                deathBlurImage.transform.GetComponent<Animator>().Play(deathBlur1AnimationName);
                yield return new WaitForSeconds(deathBlur1AnimationTime);

                //camera fall
                aliveCamera.SetActive(false);
                deathCamera.SetActive(true);

                //register death for metrics
                if(scene_controller != null) MicroScenesSystem.PlayerMetricsSave.AddToDeathCount(scene_controller.CurrentScene);

                //slow time
                Time.timeScale = deathInitialTimeSlowAmount;
            }
            else
            {
                //cancel death process
                death_started = false;
            }
        }

        IEnumerator ContinueDeath()
        {
            //start bluring screen
            deathBlurImage.transform.GetComponent<Animator>().Play(deathBlur2AnimationName);
            yield return new WaitForSeconds(deathBlur2AnimationTime);

            //pick random time for time stop
            float timeForDeathUI = Random.Range(minDeathUITime, maxDeathUITime);
            yield return new WaitForSeconds(timeForDeathUI);
            Time.timeScale = 0f;

            //show death UI
            deathUI.SetActive(true);

            //enable cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
