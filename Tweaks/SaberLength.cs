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
                ModifySaber(handControllers.Find("RightSaber")?.GetComponent<Saber>());
                ModifySaber(handControllers.Find("LeftSaber")?.GetComponent<Saber>());
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
            var originZ = (originalLength - saberBlade.localPosition.z) / 2f;

            //this.Log("Z-Axis values: originalLength=" + originalLength + " | originalZ=" + saberBlade.localPosition.z + " | originZ=" + originZ);
            this.Log("TopLocalPos=" + saberTop.localPosition.z + " | BottomsLocalPos=" + saberBottom.localPosition.z);
            //this.Log("Z-Axis new positions: localPosition=" + (originZ + length / 2f) + " | TopLocalPos=" + ((saberTop.localPosition.z - originZ) / originalLength * length) + " | BottomLocalPos=" + ((saberBottom.localPosition.z - originZ) / originalLength * length));

            //this.Log("Setting Blade Scale");
            saberBlade.localScale = new Vector3(saberBlade.localScale.x, saberBlade.localScale.y, saberBlade.localScale.z * length);
            //this.Log("Setting Blade Position");
            //saberBlade.localPosition = new Vector3(saberBlade.localPosition.x, saberBlade.localPosition.y, originZ + length / 2f);

            saberTop.localPosition = new Vector3(saberTop.localPosition.x, saberTop.localPosition.y, originalLength * length);
            this.Log("TopLocalPos=" + saberTop.localPosition.z + " | BottomsLocalPos=" + saberBottom.localPosition.z);
            //saberBottom.localPosition = new Vector3(saberBottom.localPosition.x, saberBottom.localPosition.y, (saberBottom.localPosition.z - originZ) / originalLength * length);
        }
    }
}
