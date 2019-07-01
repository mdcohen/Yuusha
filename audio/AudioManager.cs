using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Yuusha.Audio
{
    public partial class AudioManager : GameComponent
    {
        public static string CommonSoundClick1 = "0085";
        public static string CommonSoundClick2 = "0086";

        private static Dictionary<string, SoundEffect> m_soundEffects = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Song> m_songs = new Dictionary<string, Song>();
        public static List<AmbienceAudio> CurrentlyPlayingAmbience = new List<AmbienceAudio>();

        public AudioManager(Game game) : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (!Client.UserSettings.AudioEnabled)
            {
                if (!MediaPlayer.IsMuted) MediaPlayer.IsMuted = true;
                return;
            }
            else
            {
                if (!MediaPlayer.IsMuted) MediaPlayer.IsMuted = false;
            }

            foreach (AmbienceAudio ambience in CurrentlyPlayingAmbience)
            {
                ambience.Update(gameTime);
            }

            if (Client.GameState.ToString().EndsWith("Game"))
            {
                // Get sound by map ID -- then zone ID -- then Rectangle??
                if (Character.CurrentCharacter != null && Character.CurrentCharacter.m_mapID == 2)
                {
                    if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null)
                    {
                        if (MediaPlayer.Queue.ActiveSong.Name != "Wind-Moderate")
                            PlayAmbience("Wind-Moderate", true, true, .5f);
                    }
                    else if(MediaPlayer.Queue == null || MediaPlayer.State == MediaState.Stopped)
                        PlayAmbience("Wind-Moderate", true, true, .5f);
                        
                }
                else if(Character.CurrentCharacter != null && Character.CurrentCharacter.m_mapID == 0)
                {
                    if (Character.CurrentCharacter.Z < 0)
                    {
                        if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null)
                        {
                            if (MediaPlayer.Queue.ActiveSong.Name != "Kesmai-Dungeon-Background")
                                PlayAmbience("Kesmai-Dungeon-Background", true, true, .5f);
                        }
                        else if (MediaPlayer.Queue == null || MediaPlayer.State == MediaState.Stopped)
                            PlayAmbience("Kesmai-Dungeon-Background", true, true, .5f);
                    }
                    else MediaPlayer.Stop();
                }
            }
            else if (!gui.GameHUD.OverrideDisplayStates.Contains(Client.GameState)) MediaPlayer.Stop();

            base.Update(gameTime);
        }

        private static bool AmbienceExists(AmbienceAudio ambience)
        {
            foreach (AmbienceAudio ambAudio in CurrentlyPlayingAmbience)
                if (ambience.Name == ambAudio.Name) return true;

            return false;
        }
        /// <summary>
        /// Background AmbienceAudio object to be played based on the location of the CurrentCharacter.
        /// </summary>
        /// <param name="songName"></param>
        public static void PlayAmbience(string songName, bool repeating, bool fadeIn, float volume)
        {
            if (!Client.UserSettings.BackgroundAmbience)
                return;

            Song song;

            if (!m_songs.ContainsKey(songName))
            {
                song = Program.Client.Content.Load<Song>("Ambience/" + songName);
                m_songs.Add(songName, song);
            }
            else song = m_songs[songName];

            AmbienceAudio ambience = new AmbienceAudio(song, fadeIn, volume);

            if (!AmbienceExists(ambience))
                CurrentlyPlayingAmbience.Add(ambience);

            float playerVolume = fadeIn ? .1f : volume;
            MediaPlayer.IsRepeating = repeating;
            MediaPlayer.Volume = playerVolume;
            MediaPlayer.Play(song);
        }

        /// <summary>
        /// Sounds played from within the code logic.
        /// </summary>
        /// <param name="soundName"></param>
        public static void PlaySoundEffect(string soundName)
        {
            if (!Client.UserSettings.SoundEffects)
                return;

            SoundEffect soundEffect;

            if (!m_soundEffects.ContainsKey(soundName))
            {
                soundEffect = Program.Client.Content.Load<SoundEffect>(soundName);
                m_soundEffects.Add(soundName, soundEffect);
            }
            else soundEffect = m_soundEffects[soundName];

            SoundEffectInstance inst = soundEffect.CreateInstance();

            inst.Volume = 1.0f;
            inst.Play();
        }

        /// <summary>
        /// Sounds sent from the game server with direction and distance values.
        /// </summary>
        /// <param name="soundInfo"></param>
        public static void PlaySoundEffect(List<string> soundInfo)
        {
            if (!Client.UserSettings.SoundEffects)
                return;

            try
            {
                SoundEffect soundEffect;
                string soundName = soundInfo[0];

                // quick fix, sounds coming for existing KSND files are 4 characters in length
                if (soundInfo[0].Length == 4)
                    soundName = "KSND" + soundInfo[0];

                if (!m_soundEffects.ContainsKey(soundName))
                {
                    soundEffect = Program.Client.Content.Load<SoundEffect>("KSND" + soundInfo[0]);
                    m_soundEffects.Add(soundName, soundEffect);
                }
                else soundEffect = m_soundEffects[soundName];

                SoundEffectInstance inst = soundEffect.CreateInstance();

                float volume = 1.0f;
                float pan = 0.0f;

                switch (Convert.ToInt32(soundInfo[1]))
                {
                    case 1:
                        volume = .8f;
                        break;
                    case 2:
                        volume = .59f;
                        break;
                    case 3:
                        volume = .48f;
                        break;
                    case 4:
                        volume = .27f;
                        break;
                    case 5:
                        volume = .12f;
                        break;
                    case 6:
                        volume = .08f;
                        break;
                }

                //switch ((Map.Direction)Convert.ToInt32(soundInfo[2]))
                //{
                //    case Map.Direction.Northwest:
                //    case Map.Direction.West:
                //    case Map.Direction.Southwest:
                //        pan = -.5f;
                //        break;
                //    case Map.Direction.Northeast:
                //    case Map.Direction.East:
                //    case Map.Direction.Southeast:
                //        pan = .5f;
                //        break;
                //}

                inst.Volume = volume;
                //double pitch = new Random().NextDouble();
                float pitch = 0.0f;
                inst.Pitch = pitch;
                inst.Pan = pan;

                //gui.TextCue.AddClientInfoTextCue("[" + soundName + "] Volume: " + volume.ToString() + " Pan: " + pan.ToString() + " Pitch: " + pitch.ToString());

                inst.Play();
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                string soundExceptionInfo = "";
                foreach (string obj in soundInfo)
                    soundExceptionInfo = soundExceptionInfo + " " + obj;
                Utils.Log("SoundInfo: " + soundExceptionInfo);
            }
        }
    }
}
