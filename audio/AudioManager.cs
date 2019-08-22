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
        private static bool m_secondaryMusicPlaying = false;

        public enum SoundDirection { None, South, North, West, East, Southwest, Northwest, Southeast, Northeast }

        private static Dictionary<string, SoundEffect> m_soundEffects = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Song> m_songs = new Dictionary<string, Song>();
        private static List<AmbienceAudio> CurrentlyPlayingAmbience = new List<AmbienceAudio>();

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

            //foreach (AmbienceAudio ambience in new List<AmbienceAudio>(CurrentlyPlayingAmbience))
            //{
            //    if (MediaPlayer.Queue != null && (MediaPlayer.Queue.ActiveSong == null || MediaPlayer.Queue.ActiveSong != ambience.Track))
            //    {
            //        CurrentlyPlayingAmbience.Remove(ambience);
            //        ambience.Dispose();
            //    }
            //}

            foreach (AmbienceAudio ambience in new List<AmbienceAudio>(CurrentlyPlayingAmbience))
                ambience.Update(gameTime);

            if (Client.GameState.ToString().EndsWith("Game") && Character.CurrentCharacter != null && !Character.CurrentCharacter.IsDead)
            {
                // Get sound by map ID -- then zone ID -- then Rectangle??
                if (Character.CurrentCharacter != null && Character.CurrentCharacter.m_mapID == 2)
                {
                    if (Character.CurrentCharacter != null && (Character.CurrentCharacter.ZName.Contains("Vulcan") || Character.CurrentCharacter.ZName.Contains("Sloping")))
                    {
                        MediaPlayer.Stop();
                    }
                    else if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null)
                    {
                        if (Character.CurrentCharacter.ZName != "Caverns of Doom" && MediaPlayer.Queue.ActiveSong.Name != "Wind-Moderate")
                            PlayAmbience("Wind-Moderate", true, true, .5f);
                        else if (Character.CurrentCharacter.ZName == "Caverns of Doom" && MediaPlayer.Queue.ActiveSong.Name != "Cave-Ambience")
                            PlayAmbience("Cave-Ambience", true, true, .8f);
                    }
                    else if (MediaPlayer.Queue == null || MediaPlayer.State == MediaState.Stopped)
                    {
                        if (Character.CurrentCharacter.ZName != "Caverns of Doom")
                            PlayAmbience("Wind-Moderate", true, true, .5f);
                        else PlayAmbience("Cave-Ambience", true, true, .8f);
                    }

                }
                else if (Character.CurrentCharacter != null && Character.CurrentCharacter.m_mapID == 0)
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
                else MediaPlayer.Pause();
            }
            else if (Client.GameState == Enums.EGameState.Splash || Client.GameState == Enums.EGameState.Login || Client.GameState == Enums.EGameState.Menu || Client.GameState == Enums.EGameState.CharacterGeneration)
            {
                if (MediaPlayer.Queue != null && (MediaPlayer.Queue.ActiveSong == null || MediaPlayer.Queue.ActiveSong.Name != "Birth_of_a_Champion") || MediaPlayer.State != MediaState.Playing)
                {
                    if (MediaPlayer.Queue.ActiveSong != null && MediaPlayer.Queue.ActiveSong.Name == "Birth_of_a_Champion")
                        MediaPlayer.Resume();//.Play(MediaPlayer.Queue.ActiveSong);
                    else PlayAmbience("Birth_of_a_Champion", true, false, .4f);
                }
            }
            else if (!m_secondaryMusicPlaying && !gui.GameHUD.OverrideDisplayStates.Contains(Client.GameState))
            {
                MediaPlayer.Pause();
            }
            else if (m_secondaryMusicPlaying)
                MediaPlayer.Pause();

            //foreach (SoundEffectInstance inst in new List<SoundEffectInstance>(CurrentlyPlayingSoundEffects))
            //{
            //    if (inst.State == SoundState.Stopped)
            //    {
            //        CurrentlyPlayingSoundEffects.Remove(inst);
            //        inst.Dispose();
            //    }
            //}
            
            base.Update(gameTime);
        }

        private static bool AmbienceExists(AmbienceAudio ambience)
        {
            foreach (AmbienceAudio ambAudio in CurrentlyPlayingAmbience)
                if (ambience.Name == ambAudio.Name) return true;

            return false;
        }

        public static void PlaySecondarySong(string songName, bool repeating, bool fadeIn, float volume)
        {
            {
                if (!Client.UserSettings.BackgroundAmbience)
                    return;

                if (string.IsNullOrEmpty(songName))
                    return;

                if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();

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

                m_secondaryMusicPlaying = true;
                MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
                MediaPlayer.IsRepeating = repeating;
                MediaPlayer.Volume = playerVolume;
                MediaPlayer.Play(song);
            }
        }

        private static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (m_secondaryMusicPlaying && (MediaPlayer.State == MediaState.Stopped || MediaPlayer.State == MediaState.Paused))
                m_secondaryMusicPlaying = false;
        }

        /// <summary>
        /// Background AmbienceAudio object to be played based on the location of the CurrentCharacter.
        /// </summary>
        /// <param name="songName"></param>
        public static void PlayAmbience(string songName, bool repeating, bool fadeIn, float volume)
        {
            if (!Client.UserSettings.BackgroundAmbience)
                return;

            if (string.IsNullOrEmpty(songName))
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
            m_secondaryMusicPlaying = false;
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

            if (string.IsNullOrEmpty(soundName))
                return;

            if(soundName.StartsWith("Songs"))
            {
                PlaySong(soundName);
                return;
            }

            try
            {
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
            catch(Exception e)
            {
                Utils.LogException(e);
                Utils.Log("soundName: " + soundName);
            }
        }

        public static void PlaySong(string songName)
        {
            if (!Client.UserSettings.SoundEffects)
                return;

            if (string.IsNullOrEmpty(songName))
                return;

            try
            {
                Song song;

                if (!m_songs.ContainsKey(songName))
                {
                    song = Program.Client.Content.Load<Song>(songName);
                    m_songs.Add(songName, song);
                }
                else song = m_songs[songName];

                MediaPlayer.Play(song);
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                Utils.Log("songName: " + songName);
            }
        }

        /// <summary>
        /// Sounds played with information from the game server containing direction and distance values.
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
                int distance = Convert.ToInt32(soundInfo[1]);
                switch (distance)
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
                        volume = .20f;
                        break;
                    case 5:
                        volume = .12f;
                        break;
                    case 6:
                        volume = .08f;
                        break;
                }

                SoundDirection direction = (SoundDirection)Convert.ToInt32(soundInfo[2]);

                switch (direction)
                {
                    case SoundDirection.West:
                        pan = .5f;
                        break;
                    case SoundDirection.Northwest:
                    case SoundDirection.Southwest:
                        pan = .3f;
                        break;
                    case SoundDirection.East:
                        pan = -.5f;
                        break;
                    case SoundDirection.Northeast:
                    case SoundDirection.Southeast:
                        pan = -.3f;
                        break;
                }

                inst.Volume = volume;
                //double pitch = new Random().NextDouble();
                float pitch = 0.0f;
                inst.Pitch = pitch;
                inst.Pan = pan;

                //CurrentlyPlayingSoundEffects.Add(inst);

                if (distance > 3 || Client.ClientSettings.DisplaySoundIndicatorsNearby)
                    gui.SoundIndicatorLabel.CreateSoundIndicator(direction, distance);

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

        /// <summary>
        /// Stop all sound effect instances from playing. Typically called when exiting game mode.
        /// </summary>
        //public static void StopAllSounds()
        //{
        //    foreach (SoundEffectInstance inst in new List<SoundEffectInstance>(CurrentlyPlayingSoundEffects))
        //        inst.Stop();
        //}
    }
}
