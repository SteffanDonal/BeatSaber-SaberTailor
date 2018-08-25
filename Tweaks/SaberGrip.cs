using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaberTweaks.Tweaks
{
    public class SaberGrip : ITweak
    {
        public string Name => "SaberGrip";
        public bool IsPreventingScoreSubmission => false;

        public void Load()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }
        public void Cleanup()
        {
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        void SceneManagerOnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            if (loadedScene.name != "GameCore") return;

            this.Log("Tweaking GameCore...");
            Preferences.Load();
            ApplyGameCoreModifications(loadedScene.GetRootGameObjects().First());
        }

        void ApplyGameCoreModifications(GameObject gameCore)
        {
            var handControllers = gameCore.transform
                .Find("Origin")
                ?.Find("VRGameCore")
                ?.Find("HandControllers");

            if (handControllers == null)
            {
                this.Log("Couldn't find HandControllers, bailing!");
                return;
            }

            try
            {
                ModifySaber(handControllers.Find("LeftHandController")?.Find("Saber"), Preferences.GripLeftPosition, Preferences.GripLeftRotation);
                ModifySaber(handControllers.Find("RightHandController")?.Find("Saber"), Preferences.GripRightPosition, Preferences.GripRightRotation);
            }
            catch (NullReferenceException)
            {
                this.Log("Couldn't modify sabers, likely that the game structure has changed.");
                return;
            }

            this.Log("Successfully modified saber grip!");
        }

        void ModifySaber(Transform saber, Vector3 position, Quaternion rotation)
        {
            saber.localPosition = position;
            saber.localRotation = rotation;
        }
    }
}
