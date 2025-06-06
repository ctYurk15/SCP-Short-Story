using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HologramsActivationsWin
{
    public class Scene3x3 : HologramsActivationWin
    {
        [SerializeField] private GameObject fake_wall;
        [SerializeField] private GameObject wall_interaction;

        private bool executed = false;

        public override void Execute()
        {
            if (!executed)
            {
                executed = true;
                fake_wall.SetActive(false);
                wall_interaction.SetActive(true);
            }
        }
    }
}
