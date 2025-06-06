using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Brains
{
    public class MenuBrain : MonoBehaviour
    {
        //settings
        public static int language = 1;
        public static bool soundsAllowed = true;
        public static bool musicAllowed = true;

        public int maxLanguages = 3;

        private bool updatedMLtext = false;

        private void Start()
        {

        }

        private void Update()
        {
            //update text with textScript
            if (!updatedMLtext)
            {
                UpdateMLText();
                updatedMLtext = true;
            }
        }

        private void UpdateMLText()
        {
            TextScript[] texts = GameObject.FindObjectsOfType<TextScript>();

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].UpdateText();
            }
        }


        //buttons functions
        public void Switch(GameObject p)
        {
            p.SetActive(!p.activeSelf);
        }

        public void NextLanguage()
        {
            language++;
            if (language >= maxLanguages) language = 0;
            UpdateMLText();
        }

        public void GoToScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void StartGame(string scene_name)
        {
            MicroScenesSystem.PlayerMetricsSave.Clear();
            MicroScenesSystem.SceneProgressSave.Clear();

            GoToScene(scene_name);
        }

        public void GoToScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public bool GenerateBool(int truePossibilityPercents) //from 0 to 100
        {
            bool result = false;

            int number = Random.Range(0, 101);
            if (number <= truePossibilityPercents) result = true;

            return result;
        }

        public void ShowRandomBool(Text text)
        {
            text.text = "bool: " + GenerateBool(65);
        }
    }
}
