using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaberTailor.Tweaks
{
    public class SaberLength : ITweak
    {
        public string Name => "SaberLength";
        public bool IsPreventingScoreSubmission => Math.Abs(Preferences.Length - 1.0f) > 0.01f;
        private SoloFreePlayFlowCoordinator _soloFreePlayFlowCoordinator;
        private PracticeViewController _practiceViewController;

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

            if (IsPreventingScoreSubmission)
            {
                // Check if practice mode is active
                // This part should probably be moved so it can be a shared function for multiple tweaks
                _soloFreePlayFlowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().FirstOrDefault();
                if (_soloFreePlayFlowCoordinator == null)
                {
                    this.Log("Couldn't find SoloFreePlayFlowCoordinator, bailing!");
                    return;
                }
                _practiceViewController = ReflectionUtil.GetPrivateField<PracticeViewController>(_soloFreePlayFlowCoordinator, "_practiceViewController");
                if (_practiceViewController == null)
                {
                    this.Log("Couldn't find PracticeViewController, bailing!");
                    return;
                }
                if (!_practiceViewController.isInViewControllerHierarchy)
                {
                    this.Log("Practice mode is not active. Not modifying sabers.");
                    return;
                }
            }

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
                ModifySaber(handControllers.Find("LeftSaber")?.GetComponent<Saber>());
                ModifySaber(handControllers.Find("RightSaber")?.GetComponent<Saber>());
            }
            catch (NullReferenceException)
            {
                this.Log("Couldn't modify sabers, likely that the game structure has changed.");
                return;
            }

            this.Log("Successfully modified sabers!");
        }
        void ModifySaber(Saber saber)
        {
            var length = Preferences.Length;
            var saberBlade = saber.transform.Find("Saber");
            var saberTop = ReflectionUtil.GetPrivateField<Transform>(saber, "_topPos");
            var saberBottom = ReflectionUtil.GetPrivateField<Transform>(saber, "_bottomPos");

            // In v0.12.0, blade and handle are not different Unity objects anymore
            var originalLength = saberBlade.localScale.z;

            saberBlade.localScale = new Vector3(saberBlade.localScale.x, saberBlade.localScale.y, saberBlade.localScale.z * length);

            saberTop.localPosition = new Vector3(saberTop.localPosition.x, saberTop.localPosition.y, originalLength * length);

        }
    }
}
