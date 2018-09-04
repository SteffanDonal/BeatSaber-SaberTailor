using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xft;

namespace SaberTailor.Tweaks
{
    public class SaberTrail : ITweak
    {
        public string Name => "SaberTrail";
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
                ModifyTrail(handControllers.Find("LeftHandController")?.Find("Saber")?.GetComponent<XWeaponTrail>());
                ModifyTrail(handControllers.Find("RightHandController")?.Find("Saber")?.GetComponent<XWeaponTrail>());
            }
            catch (NullReferenceException)
            {
                this.Log("Couldn't modify trails, likely that the game structure has changed.");
                return;
            }

            this.Log("Successfully modified trails!");
        }
        void ModifyTrail(XWeaponTrail trail)
        {
            var length = Preferences.TrailLength;

            if (Preferences.IsTrailEnabled)
            {
                ReflectionUtil.SetPrivateField(trail, "_maxFrame", length);
                ReflectionUtil.SetPrivateField(trail, "_granularity", length * 3);
            }
            else
            {
                trail.enabled = false;
            }
        }
    }
}
