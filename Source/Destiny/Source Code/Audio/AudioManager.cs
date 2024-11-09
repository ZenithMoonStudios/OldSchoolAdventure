using Microsoft.Xna.Framework.Audio;
using System;

namespace Destiny
{
    /// <summary>
    /// Audio manager class.
    /// </summary>
    public class AudioManager
    {
        private String MusicCueName;
        private SoundEffectInstance m_MusicCue;
        AudioConfiguration SoundLibrary = new AudioConfiguration();

        /// <summary>
        /// Constructor.
        /// </summary>
        public AudioManager(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            SoundLibrary = Content.Load<AudioConfiguration>(@"Audio\SoundLibrary");
            foreach (AudioFile file in SoundLibrary)
            {
                file.soundEffect = Content.Load<SoundEffect>(file.FileName);
                if (file.RequiresInstance) file.soundEffectInstance = file.soundEffect.CreateInstance();
                if (file.isLooped && file.RequiresInstance) file.soundEffectInstance.IsLooped = true;
                if (file.AutoStart && file.RequiresInstance) file.soundEffectInstance.Play();
            }

        }

        /// <summary>
        /// Whether game is active.
        /// </summary>
        private bool m_IsGameActive = true;
        public bool IsGameActive
        {
            set
            {
                m_IsGameActive = value;
                this.UpdateActiveState();
            }
        }

        /// <summary>
        /// Whether game is paused.
        /// </summary>
        private bool m_IsGamePaused = false;
        public bool IsGamePaused
        {
            set
            {
                m_IsGamePaused = value;
                this.UpdateActiveState();
            }
        }

        /// <summary>
        /// Whether audio is active.
        /// </summary>
        private bool m_IsActive = true;
        private void UpdateActiveState()
        {
            bool isActive = m_IsGameActive && !m_IsGamePaused;
            if (isActive != m_IsActive)
            {
                // Is active value has changed
                m_IsActive = isActive;
                if (m_MusicCue != null)
                {
                    if (m_IsActive && MusicState(SoundState.Paused))
                    {
                        // Active - resume music
                        m_MusicCue.Resume();
                    }
                    else if (!m_IsActive && MusicState(SoundState.Playing))
                    {
                        // Not active - pause music
                        m_MusicCue.Pause();
                    }
                }
            }
        }

        /// <summary>
        /// Play the specified cue.
        /// </summary>
        public void PlayCue(string p_Cue)
        {
            AudioFile cue = new AudioFile() { Cuename = p_Cue };
            if (SoundLibrary.Contains(cue))
                SoundLibrary[SoundLibrary.IndexOf(cue)].soundEffect.Play();
        }

        /// <summary>
        /// Play the specified music cue.
        /// </summary>
        public void PlayMusic(string p_Cue)
        {
            this.PlayMusic(p_Cue, false);
        }

        /// <summary>
        /// Play the specified music cue.
        /// </summary>
        public void PlayMusic(string p_Cue, bool p_RestartIfSameCue)
        {
            if (p_RestartIfSameCue || m_MusicCue == null || MusicCueName != p_Cue)
            {
                AudioFile cue = new AudioFile() { Cuename = p_Cue };
                if (SoundLibrary.Contains(cue))
                {
                    m_MusicCue = SoundLibrary[SoundLibrary.IndexOf(cue)].soundEffectInstance;
                    m_MusicCue.Play();
                    MusicCueName = p_Cue;
                }
            }
        }

        /// <summary>
        /// Stop music.
        /// </summary>
        public void StopMusic()
        {
            if (!String.IsNullOrEmpty(MusicCueName))
            {
                m_MusicCue.Stop();
                m_MusicCue = null;
                MusicCueName = null;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.StopMusic();
        }

        public bool MusicState(SoundState state)
        {
            return m_MusicCue.State == state;
        }
    }
}