using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using NAudio.Wave;
using System.Windows.Forms;

namespace StreetFighter2
{
    class AudioManager
    {
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;
        bool isLooping;

        public void PlayMusic(string filePath, bool loop = false)
        {
            try
            {
                StopMusic();

                isLooping = loop;

                audioFileReader = new AudioFileReader(filePath);
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
                MessageBox.Show(
                $"Failed to play audio:" +
                $"\n" +
                $"{filePath}" +
                $"\n" +
                $"\n" +
                $"Error: {ex.Message}",
                "Audio Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
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
            if (audioFileReader != null)
            {
                audioFileReader.Volume = Math.Clamp(volume, 0f, 1f);
            }
        }

        public void PauseMusic()
        {
            waveOutDevice?.Pause();
        }

        public void ResumeMusice()
        {
            waveOutDevice?.Play();
        }
    }
}
