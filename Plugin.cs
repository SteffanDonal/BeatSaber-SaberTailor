using IllusionPlugin;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaberTailor
{
    [UsedImplicitly]
    public class Plugin : IPlugin
    {
        public const string Name = "SaberTailor";
        public const string Version = "2.0.0";

        string IPlugin.Name => Name;
        string IPlugin.Version => Version;

        readonly List<Tweaks.ITweak> _tweaks = new List<Tweaks.ITweak>
        {
            new Tweaks.SaberLength(),
            new Tweaks.SaberGrip(),
            new Tweaks.SaberTrail()
        };

        public void OnApplicationStart()
        {
            _tweaks.ForEach(tweak =>
            {
                try
                {
                    tweak.Load();
                    Log("Loaded tweak: {0}", tweak.Name);
                }
                catch (Exception ex)
                {
                    Log("Failed to load tweak: {0}. Exception: {1}", tweak.Name, ex);
                }
            });

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }
        public void OnApplicationQuit()
        {
            _tweaks.ForEach(tweak =>
            {
                try
                {
                    tweak.Cleanup();
                    Log("Unloaded tweak: {0}", tweak.Name);
                }
                catch (Exception ex)
                {
                    Log("Failed to unload tweak: {0}. Exception: {1}", tweak.Name, ex);
                }
            });

            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }


        void SceneManagerOnActiveSceneChanged(Scene previousScene, Scene currentScene)
        {
            Preferences.Load();

            EnsureMainGameSceneSetup();
        }

        StandardLevelSceneSetupDataSO _mainGameSceneSetupData;
        bool _justPreventedSubmission;
        void EnsureMainGameSceneSetup()
        {
            if (_mainGameSceneSetupData == null)
            {
                _mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault();

                if (_mainGameSceneSetupData == null) return;
                _mainGameSceneSetupData.didFinishEvent += OnMainGameSceneDidFinish;
            }

            if (_justPreventedSubmission)
            {
                var resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
                if (resultsViewController == null) return;

                resultsViewController.continueButtonPressedEvent += controller =>
                {
                    //PersistentSingleton<GameDataModel>.instance
                    //    .gameDynamicData.GetCurrentPlayerDynamicData()
                    //    .gameplayOptions.noEnergy = false;

                    _justPreventedSubmission = false;
                };
            }
        }
        void OnMainGameSceneDidFinish(StandardLevelSceneSetupDataSO setupData, LevelCompletionResults levelCompletionResults)
        {
            
            //if (!setupData.gameplayOptions.validForScoreUse) return; // NoFail active
            if (levelCompletionResults?.levelEndStateType != LevelCompletionResults.LevelEndStateType.Cleared) return;

            var submissionBlockers = _tweaks.Where(tweak => tweak.IsPreventingScoreSubmission).ToList();
            if (submissionBlockers.Count == 0) return;

            submissionBlockers.ForEach(tweak => Log("Score submission prevented by tweak: {0}", tweak.Name));

            //setupData.gameplayOptions.noEnergy = true;
            _justPreventedSubmission = true;
        }

        public static void Log(string format, params object[] args)
        {
            Console.WriteLine($"[{Name}] " + format, args);
        }
        public static void Log(string message)
        {
            Log(message, new object[] { });
        }

        #region Unused IPlugin Members

        void IPlugin.OnUpdate() { }
        void IPlugin.OnFixedUpdate() { }
        void IPlugin.OnLevelWasLoaded(int level) { }
        void IPlugin.OnLevelWasInitialized(int level) { }

        #endregion
    }
}
