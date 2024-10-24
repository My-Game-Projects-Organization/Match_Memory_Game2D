using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShopSystemPackage
{

    public static class SceneController
    {
        static int mainScene = 0;

        public static void LoadMainScene()
        {
            SceneManager.LoadScene(mainScene);
        }

        public static void LoadNextScene()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            if (currentScene < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(currentScene + 1);
            }
        }

        public static void LoadPreviosScene()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            if (currentScene > 0)
            {
                SceneManager.LoadScene(currentScene - 1);
            }
        }

        public static void LoadScene(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(sceneIndex);
        }
    }

}