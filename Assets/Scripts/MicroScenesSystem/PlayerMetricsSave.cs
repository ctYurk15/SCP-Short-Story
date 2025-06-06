using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MicroScenesSystem
{
    public class PlayerMetricsSave : MonoBehaviour
    {
        private static Dictionary<int, IDictionary<string, object>> metrics;

        private static long last_timestamp;
        private static int seconds;
        private static PlayerMetricsSave metrics_obj;

        public static void SaveSceneEndMetrics(GameObject player, int scene_number, float scene_time)
        {
            if (metrics == null) metrics = new Dictionary<int, IDictionary<string, object>>();

            IDictionary<string, object> new_element = new Dictionary<string, object>();

            new_element["scene"] = scene_number.ToString();

            //basic player stats
            new_element["hp"] = player.GetComponent<HP.PlayerHealth>().HP.ToString();
            new_element["inventory"] = (int[])player.GetComponent<Inventory>().GetItemsQuantities().Clone();
            new_element["guns_magazines"] = player.GetComponent<GunInventory>().GetGunsAmmoState();
            new_element["picked_up_items"] = player.GetComponent<Inventory>().GetPickedUpItemsCount();

            //death count
            int death_count = 0;
            if(metrics.ContainsKey(scene_number))
            {
                IDictionary<string, object> old_data = metrics[scene_number];
                death_count = (int)old_data["death_count"];
            }
            new_element["death_count"] = death_count;

            //scene completion time
            new_element["scene_time"] = scene_time;

            metrics[scene_number] = new_element;
        }

        public static string Stringify()
        {
            string metrics_text = "";

            foreach (int scene_number in metrics.Keys)
            {
                IDictionary<string, object> element = metrics[scene_number];

                //combining inventory state into one string
                string inventory_line = "";
                foreach (int quantity in (int[])element["inventory"])
                {
                    inventory_line += quantity + "-";
                }

                //combining guns magazine ammo state into one string
                int[] mags = (int[])element["guns_magazines"];
                string mags_line = mags[0] + "-" + mags[1];

                //combining picked up items counts into one string
                string picked_up_items_line = "";
                foreach (int quantity in (List<int>)element["picked_up_items"])
                {
                    picked_up_items_line += quantity + "-";
                }
                if (picked_up_items_line == "") picked_up_items_line = "0-0-0-0-0-0";

                //combining whole scene metrics into one big string
                metrics_text += element["scene"] + ";"
                    + element["hp"] + ";"
                    + inventory_line + ";"
                    + mags_line + ";"
                    + picked_up_items_line + ";"
                    + element["death_count"] + ";"
                    + element["scene_time"] + "\n";
            }

            return metrics_text;
        }

        public static void OutputMetrics()
        {
            foreach (int sN in metrics.Keys)
            {
                IDictionary<string, object> element = metrics[sN];

                Debug.Log(element["scene"]);
                Debug.Log(element["hp"]);

                string inventory_line = "";
                foreach (int quantity in (int[])element["inventory"])
                {
                    inventory_line += quantity + ";";
                }
                Debug.Log(inventory_line);

                int[] mags = (int[])element["guns_magazines"];
                Debug.Log(mags[0] + " & " + mags[1]);

                string items_line = "";
                foreach (int quantity in (List<int>)element["picked_up_items"])
                {
                    items_line += quantity + ";";
                }
                Debug.Log(items_line);

                Debug.Log("Death count: " + element["death_count"]);
                Debug.Log("Scene time: " + element["scene_time"]);
            }
        }

        public static void AddToDeathCount(int scene_number)
        {
            IDictionary<string, object> scene_data;

            if (!metrics.ContainsKey(scene_number))
            {
                IDictionary<string, object> new_element = new Dictionary<string, object>();
                new_element["scene"] = scene_number.ToString();
                new_element["death_count"] = 0;
                metrics.Add(scene_number, new_element);

                scene_data = new_element;
            }
            else scene_data = metrics[scene_number];

            scene_data["death_count"] = (int) scene_data["death_count"] + 1;
        }

        public static Dictionary<int, IDictionary<string, object>> GetMetrics()
        {
            return metrics;
        }

        public static void Clear()
        {
            metrics = null;
        }
    }
}


