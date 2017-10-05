namespace GabriellaApp.Droid
{
    public abstract class PlaybackInfoListener
    {
        public static string ConvertStateToString(PlaybackState state)
        {
            var stateString = "N/A";
            switch (state)
            {
                case PlaybackState.Completed:
                    stateString = "COMPLETED";
                    break;
                case PlaybackState.Invalid:
                    stateString = "INVALID";
                    break;
                case PlaybackState.Paused:
                    stateString = "PAUSED";
                    break;
                case PlaybackState.Playing:
                    stateString = "PLAYING";
                    break;
                case PlaybackState.Reset:
                    stateString = "RESET";
                    break;
            }

            return stateString;
        }

        public abstract void OnLogUpdated(string formattedMessage);

        public abstract void OnDurationChanged(int duration);

        public abstract void OnPositionChanged(int position);

        public abstract void OnStateChanged(PlaybackState state);

        public abstract void OnPlaybackCompleted();
    }
}
