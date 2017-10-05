using System;
using Android.Content;
using Android.Media;
using Java.Lang;
using Java.Util.Concurrent;

namespace GabriellaApp.Droid
{
    public class MusicPlayer : IMusicPlayer
    {
        public const int PLAYBACK_POSITION_REFRESH_INTERVAL_MS = 1000;

        private Context _context;
        private MediaPlayer _mediaPlayer;
        private int _resourceId;
        private IScheduledExecutorService mExecutor;
        private PlaybackInfoListener _playbackInfoListener;
        private Runnable mSeekbarPositionUpdateTask;

        private void InitializeMediaPlayer()
        {
            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer();
                var onCompletionListener = new OnCompletionListener();
                onCompletionListener.Completion += OnPlayerCompletionListener;
                if (_playbackInfoListener != null)
                {
                    _playbackInfoListener.OnStateChanged(PlaybackState.Completed);
                    _playbackInfoListener.OnPlaybackCompleted();
                }
            }
        }

        public void SetPlaybackInfoListener(PlaybackInfoListener listener)
        {
            _playbackInfoListener = listener;
        }

        private void OnPlayerCompletionListener(object sender, MediaPlayer mediaPlayer)
        {
            mExecutor = Executors.NewSingleThreadScheduledExecutor();
        }

        // Reports media playback position to mPlaybackProgressCallback.
        private void StopUpdatingCallbackWithPosition(bool resetUIPlaybackPosition)
        {
            if (mExecutor != null)
            {
                mExecutor.ShutdownNow();
                mExecutor = null;
                mSeekbarPositionUpdateTask = null;
                if (resetUIPlaybackPosition && _playbackInfoListener != null)
                {
                    _playbackInfoListener.OnPositionChanged(0);
                }
            }
        }

        public void LoadMedia(int resourceId)
        {
            _resourceId = resourceId;

            InitializeMediaPlayer();

            var assetsFileDescriptor = _context.Resources.OpenRawResourceFd(_resourceId);
            try
            {
                _mediaPlayer.SetDataSource(assetsFileDescriptor);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Could not play: " + ex.Message);
            }

            try
            {
                _mediaPlayer.Prepare();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Could not play: " + ex.Message);
            }

            InitializeProgressCallback();
        }

        public void Release()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Release();
                _mediaPlayer = null;
            }
        }

        public bool IsPlaying()
        {
            return _mediaPlayer?.IsPlaying ?? false;
        }

        public void Play()
        {
            if (_mediaPlayer != null && IsPlaying() == false)
            {
                _mediaPlayer.Start();
                if (_playbackInfoListener != null)
                {
                    _playbackInfoListener.OnStateChanged(PlaybackState.Playing);
                }

                StartUpdatingCallbackWithPosition();
            }
        }

        public void Reset()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Reset();
                LoadMedia(_resourceId);
                if (_playbackInfoListener != null)
                {
                    _playbackInfoListener.OnStateChanged(PlaybackState.Reset);
                }
            }
        }

        public void Pause()
        {
            if (_mediaPlayer != null && IsPlaying())
            {
                _mediaPlayer.Pause();
                if (_playbackInfoListener  != null)
                {
                    _playbackInfoListener.OnStateChanged(PlaybackState.Paused);
                }
            }
        }

        public void InitializeProgressCallback()
        {
            throw new NotImplementedException();
        }

        public void SeekTo(int position)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.SeekTo(position);
            }
        }

        /**
         * Syncs the mMediaPlayer position with mPlaybackProgressCallback via recurring task.
         */
        private void StartUpdatingCallbackWithPosition()
        {
            if (mExecutor == null)
            {
                mExecutor = Executors.NewSingleThreadScheduledExecutor();
            }
            if (mSeekbarPositionUpdateTask == null)
            {
                mSeekbarPositionUpdateTask = new Runnable(UpdateProgressCallbackTask);
            };


            mExecutor.ScheduleAtFixedRate(
                    mSeekbarPositionUpdateTask,
                        0,
                        PLAYBACK_POSITION_REFRESH_INTERVAL_MS,
                TimeUnit.Milliseconds);
        }

        private void UpdateProgressCallbackTask()
        {
            if (_mediaPlayer != null && IsPlaying())
            {
                int currentPosition = _mediaPlayer.CurrentPosition;
                if (_playbackInfoListener != null)
                {
                    _playbackInfoListener.OnPositionChanged(currentPosition);
                }
            }
        }
    }
}
