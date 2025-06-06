using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene3x3_Maze
{
    public class Door : SceneObjectsInteractable.Door
    {
        //[SerializeField] private Controller maze_controller;
        [Header("Maze config")]
        [SerializeField] private Block block;
        [SerializeField] private Transform spawn_point;
        [SerializeField] private bool is_right = false;

        public override void InteractAction()
        {
            base.InteractAction();
            block.NotifyAboutDoorsOpen(spawn_point.position, is_right);
        }
    }
}
