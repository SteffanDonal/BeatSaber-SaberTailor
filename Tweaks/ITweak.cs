namespace SaberTailor.Tweaks
{
    public interface ITweak
    {
        string Name { get; }

        bool IsPreventingScoreSubmission { get; }

        void Load();
        void Cleanup();
    }

    public static class TweakExtensions
    {
        public static void Log(this ITweak tweak, string format, params object[] args)
        {
            Plugin.Log($"[{tweak.Name}] " + format, args);
        }
        public static void Log(this ITweak tweak, string message)
        {
            Plugin.Log($"[{tweak.Name}] " + message);
        }
    }
}
