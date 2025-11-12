using System;
using System.IO;
using NAudio.Wave;
using System.Windows.Forms;

namespace StreetFighter2
{
    public class AudioManager : IDisposable
    {
        // ============================================================
        // ENCAPSULATION: Private fields
        // ============================================================
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;
        private bool isLooping;
        private float currentVolume;
        private bool isDisposed;

        // ============================================================
        // ENCAPSULATION: Properties with validation
        // ============================================================
        public bool IsPlaying => waveOutDevice?.PlaybackState == PlaybackState.Playing;
        public bool IsPaused => waveOutDevice?.PlaybackState == PlaybackState.Paused;
        public float Volume
        {
            get => currentVolume;
            set => SetVolume(value);
        }

        public AudioManager()
        {
            currentVolume = 0.5f;
            isDisposed = false;
        }

        public void PlayMusic(string filePath, bool loop = false)
        {
            // ============================================================
            // ENCAPSULATION: Validation before operation
            // ============================================================
            if (!File.Exists(filePath))
            {
                ShowError($"Audio file not found: {filePath}");
                return;
            }

            try
            {
                StopMusic();

                isLooping = loop;

                audioFileReader = new AudioFileReader(filePath);
                audioFileReader.Volume = currentVolume;
                
                waveOutDevice = new WaveOutEvent();
                waveOutDevice.Init(audioFileReader);

                if (loop)
                {
                    waveOutDevice.PlaybackStopped += OnPlaybackStopped;
                }

                waveOutDevice.Play();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to play audio:\n{filePath}\n\nError: {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            if (isLooping && audioFileReader != null && waveOutDevice != null)
            {
                audioFileReader.Position = 0;
                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Play();
            }
        }

        public void StopMusic()
        {
            isLooping = false;

            if (waveOutDevice != null)
            {
                waveOutDevice.PlaybackStopped -= OnPlaybackStopped;
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
        }

        public void SetVolume(float volume)
        {
            currentVolume = Math.Clamp(volume, 0f, 1f);
            
            if (audioFileReader != null)
            {
                audioFileReader.Volume = currentVolume;
            }
        }

        public void PauseMusic()
        {
            if (IsPlaying)
            {
                waveOutDevice?.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (IsPaused)
            {
                waveOutDevice?.Play();
            }
        }

        // ============================================================
        // ENCAPSULATION: Private helper method for error display
        // ============================================================
        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Audio Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        // ============================================================
        // ENCAPSULATION: Implement IDisposable pattern
        // ============================================================
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    StopMusic();
                }
                isDisposed = true;
            }
        }

        ~AudioManager()
        {
            Dispose(false);
        }
    }
}
