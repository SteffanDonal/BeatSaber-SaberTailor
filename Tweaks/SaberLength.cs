using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaberTweaks.Tweaks
{
    public class SaberLength : ITweak
    {
        public string Name => "SaberLength";
        public bool IsPreventingScoreSubmission => Math.Abs(Preferences.Length - 1.0f) > 0.01f;

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
                ModifySaber(handControllers.Find("LeftHandController")?.Find("Saber")?.GetComponent<Saber>());
                ModifySaber(handControllers.Find("RightHandController")?.Find("Saber")?.GetComponent<Saber>());
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
            var saberBlade = saber.transform.Find("Blade");
            var saberTop = saber.saberBladeTopPosTransform;
            var saberBottom = saber.saberBladeBottomPosTransform;

            var originalLength = saberBlade.localScale.y;
            var originZ = saberBlade.localPosition.z - originalLength / 2f;

            saberBlade.localScale = new Vector3(saberBlade.localScale.x, length, saberBlade.localScale.z);
            saberBlade.localPosition = new Vector3(saberBlade.localPosition.x, saberBlade.localPosition.y, originZ + length / 2f);

            saberTop.localPosition = new Vector3(saberTop.localPosition.x, saberTop.localPosition.y, (saberTop.localPosition.z - originZ) / originalLength * length);
            saberBottom.localPosition = new Vector3(saberBottom.localPosition.x, saberBottom.localPosition.y, (saberBottom.localPosition.z - originZ) / originalLength * length);
        }
    }
}
