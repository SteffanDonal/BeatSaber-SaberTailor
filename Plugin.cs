using IllusionPlugin;

namespace SaberTweaks
{
    public class Plugin : IPlugin
    {
        public const string Name = "SaberTweaks";
        public const string Version = "0.0.1";

        string IPlugin.Name => Name;
        string IPlugin.Version => Version;

        public void OnApplicationStart() { }
        public void OnApplicationQuit() { }

        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }

        public void OnUpdate() { }

        public void OnFixedUpdate() { }
    }
}
