using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HP
{
    public class HPComponent : MonoBehaviour
    {
        public float hp = 100;
        public DeathController death_controller;

        public virtual void Damage(float amount)
        {
            hp -= amount;
            if (hp <= 0)
            {
                Death();
            }
        }

        public void Death()
        {
            death_controller.Death();
        }
    }
}


