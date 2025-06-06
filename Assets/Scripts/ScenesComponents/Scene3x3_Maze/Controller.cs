using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Scene3x3_Maze
{
    public class Controller : MonoBehaviour
    {
        [Header("Blocks objects")]
        [SerializeField] private Block[] blocks;
        [SerializeField] private Block start_block;
        [SerializeField] private Block final_block;
        [Space]

        [Header("Blocks spawn config")]
        [SerializeField] private int min_blocks_count;
        [SerializeField] private int max_blocks_count;
        [Space]

        [Header("Audio")]
        [SerializeField] private AudioSource[] left_path_audio_parts;
        [SerializeField] private AudioSource[] right_path_audio_parts;
        [SerializeField] private AudioSource[] player_question_audio_parts;
        [SerializeField] private AudioSource[] wrong_turn_audio_parts;

        private Block[] shuffled_blocks;
        [SerializeField] private bool[] blocks_turn_is_right;

        private int current_block_index = 0;
        private Block last_block;

        private void Start()
        {
            this.ShuffleBlocks();
        }

        public void CreateNext(Vector3 position, bool is_right)
        {
            if (current_block_index < shuffled_blocks.Length-1)
            {
                //if opened wrong doors, return to beginning
                if (blocks_turn_is_right[current_block_index] != is_right)
                {
                    current_block_index = 0;

                    AudioSource wrong_turn_audio = GetRandomAudio(wrong_turn_audio_parts);
                    wrong_turn_audio.Play();
                }
                //if opened correct doors, get to next block
                else
                {
                    current_block_index++;
                }

                //create & init new block
                Block new_block = shuffled_blocks[current_block_index];
                new_block.gameObject.SetActive(true);
                new_block.Open();
                new_block.transform.position = position;
            }
            else
            {
                Debug.Log("Final");
            }

        }

        public void BlockEntered()
        {
            shuffled_blocks[current_block_index].Close();

            //using last_block instead of current_block_index-1 helps to deactivate every block, even if it is initial
            if (last_block != null)
            {
                last_block.gameObject.SetActive(false);
            }
            last_block = shuffled_blocks[current_block_index];

            //tell next turn, except on first & last bocks
            if(current_block_index != 0 && current_block_index != shuffled_blocks.Length-1)
            {
                /* AudioSource audio = GetRandomAudio(blocks_turn_is_right[current_block_index] ? right_path_audio_parts : left_path_audio_parts);
                 audio.Play();*/
                StartCoroutine("PlayTurnAudio");
            }
        }

        private IEnumerator PlayTurnAudio()
        {
            AudioSource question_audio = GetRandomAudio(player_question_audio_parts);
            question_audio.Play();

            yield return new WaitForSeconds(question_audio.clip.length);

            AudioSource answer_audio = GetRandomAudio(blocks_turn_is_right[current_block_index] ? right_path_audio_parts : left_path_audio_parts);
            answer_audio.Play();
        }

        private void ShuffleBlocks()
        {
            //initiate random generator with seed
            System.Random random_generator = new System.Random(MicroScenesSystem.SeedGenerator.current_seed);

            //generate basic objects
            int blocks_count = random_generator.Next(min_blocks_count, max_blocks_count + 1);
            Block[] shuffled_blocks_arr = blocks.OrderBy(n => random_generator.Next()).ToArray();

            //instanstiate arrays
            shuffled_blocks = new Block[blocks_count+2];
            blocks_turn_is_right = new bool[blocks_count+2];

            //add first & last blocks, since they are always the same
            shuffled_blocks[0] = start_block;
            shuffled_blocks[blocks_count + 1] = final_block;

            blocks_turn_is_right[0] = true; //first room will have only one door
            blocks_turn_is_right[blocks_count + 1] = true;

            //fill blocks queue
            for (int i = 1; i < blocks_count+1; i++)
            {
                Block block = shuffled_blocks_arr[i];
                block.index = i;

                shuffled_blocks[i] = block;
                blocks_turn_is_right[i] = random_generator.Next(0, 2) == 0;
            }
        }

        private AudioSource GetRandomAudio(AudioSource[] audio_parts)
        {
            int index = UnityEngine.Random.Range(0, audio_parts.Length);
            return audio_parts[index];
        }
    }
}
