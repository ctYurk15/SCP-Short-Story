using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Brains
{
    public class MetricsSceneBrain : MenuBrain
    {
        [SerializeField] private TMP_InputField pro;
        
        private void Start()
        {
            /*Dictionary<int, IDictionary<string, object>> metrics = MicroScenesSystem.PlayerMetricsSave.GetMetrics();

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
            }*/

            pro.text = MicroScenesSystem.PlayerMetricsSave.Stringify();
        }
    }
}
