using System;
namespace GabriellaApp
{
    public interface IMusicPlayer
    {
        void LoadMedia(int resourceId);

        void Release();

        bool IsPlaying();

        void Play();

        void Reset();

        void Pause();

        void InitializeProgressCallback();

        void SeekTo(int position);
    }
}
