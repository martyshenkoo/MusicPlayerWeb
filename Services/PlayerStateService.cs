namespace MusicPlayerWeb.Services
{
    public class PlayerStateService
    {
        private static PlayerStateService? _instance;
        private static readonly object _lock = new();

        public string? CurrentTrackTitle { get; private set; }
        public string? CurrentTrackUrl { get; private set; }

        private PlayerStateService() { }

        public static PlayerStateService Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new PlayerStateService();
                }
            }
        }

        public void SetTrack(string title, string url)
        {
            CurrentTrackTitle = title;
            CurrentTrackUrl = url;
        }

        public void Clear()
        {
            CurrentTrackTitle = null;
            CurrentTrackUrl = null;
        }
    }
}
