using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.BulletHoles.SimpleController
{
    /// <summary>
    /// Player Hands behaviour
    /// </summary>
    public class Hands : MonoBehaviour
    {
        /// <summary>
        /// Hand weapons list.
        /// </summary>
        [Tooltip("Hand weapons list")] public Weapon[] Weapons;
        /// <summary>
        /// Player camera.
        /// </summary>
        [Tooltip("Player camera")] public Camera Cam;
        /// <summary>
        /// Keys array to select weapon.
        /// </summary>
        [Tooltip("Index array in circle weapon selector of each weapon")] public KeyCode[] keys;

        float startFov;

        void Start()
        {
            startFov = Cam.fieldOfView;
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].gameObject.SetActive(false);
            }
        }

        void Update()
        {
            int index = 0;
            foreach (Weapon weapon in Weapons)
            {
                if (Input.GetKeyDown(keys[index]))
                {
                    bool deployed = weapon.gameObject.activeSelf;

                    foreach (var w in Weapons)
                    {
                        w.gameObject.SetActive(false);
                    }

                    weapon.gameObject.SetActive(!deployed);
                }
                index++;
            }
            foreach (Weapon weapon in Weapons)
            {
                if (weapon.gameObject.activeSelf)
                {
                    Cam.fieldOfView = weapon.CurrentFov;
                    return;
                }
            }
            Cam.fieldOfView = startFov;
        }

        private void Deploy(int index)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                if (i == index)
                    continue;

                Weapons[i].gameObject.SetActive(false);
            }
            Weapons[index].gameObject.SetActive(true);
        }
    }
}