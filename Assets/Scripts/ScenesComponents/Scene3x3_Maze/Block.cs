using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene3x3_Maze
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private GameObject entrance_wall;
        [SerializeField] private Door[] doors;
        [SerializeField] private Controller controller;
        [SerializeField] private BlockEnterTrigger enter_trigger;

        public int index; 

        public void Close()
        {
            entrance_wall.SetActive(true);
        }

        public void Open()
        {
            entrance_wall.SetActive(false);

            foreach (Door door in doors)
            {
                door.ResetState();
            }

            enter_trigger.Deactivate();
        }

        public void NotifyAboutEnter()
        {
            controller.BlockEntered();
        }

        public void NotifyAboutDoorsOpen(Vector3 spawn_position, bool is_right)
        {
            foreach(Door door in doors)
            {
                door.Deactivate();
            }

            controller.CreateNext(spawn_position, is_right);
        }
    }
}
