using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene3x3_Maze
{
    public class BlockEnterTrigger : MonoBehaviour
    {
        [SerializeField] private Block block;

        private bool activated = false;

        private void OnTriggerEnter(Collider other)
        {
            if(!activated)
            {
                activated = true;
                block.NotifyAboutEnter();
            }
        }

        public void Deactivate()
        {
            activated = false;
        }
    }
}
