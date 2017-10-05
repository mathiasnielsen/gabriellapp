using System;
using Android.Media;

namespace GabriellaApp.Droid
{
    public class OnCompletionListener : Java.Lang.Object, MediaPlayer.IOnCompletionListener
    {
        public event EventHandler<MediaPlayer> Completion;

        public void OnCompletion(MediaPlayer mediaPlayer)
        {
            Completion?.Invoke(this, mediaPlayer);
        }
    }
}
