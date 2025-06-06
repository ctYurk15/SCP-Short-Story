using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem
{
    public class SceneProgressSave : MonoBehaviour
    {
        private static int last_scene = -1;
        private static float player_hp = -1f;
        private static int[] player_inventory_quantites;

        private static int primary_gun_magazine = -1;
        private static int secondary_gun_magazine = -1;

        private static bool saved_player_stats = false;

        public static int GetSceneNumber()
        {
            return last_scene;
        }

        public static void SaveProgress(GameObject player, int scene_number)
        {
            last_scene = scene_number;

            player_hp = player.GetComponent<HP.PlayerHealth>().HP;
            player_inventory_quantites = (int[]) player.GetComponent<Inventory>().GetItemsQuantities().Clone();

            int[] ammo_state = player.GetComponent<GunInventory>().GetGunsAmmoState();
            primary_gun_magazine = ammo_state[0];
            secondary_gun_magazine = ammo_state[1];

            saved_player_stats = true;
        }

        public static void LoadPlayerProgress(GameObject player)
        {
            if(saved_player_stats)
            {
                player.GetComponent<HP.PlayerHealth>().HP = player_hp;
                player.GetComponent<Inventory>().SetItemsQuantities(player_inventory_quantites);
                player.GetComponent<GunInventory>().SetGunsAmmoState(primary_gun_magazine, secondary_gun_magazine);
            }
        }

        public static void Clear()
        {
            last_scene = -1;
            player_hp = -1f;
            player_inventory_quantites = null;
            primary_gun_magazine = -1;
            secondary_gun_magazine = -1;
            saved_player_stats = false;
        }
    }
}
