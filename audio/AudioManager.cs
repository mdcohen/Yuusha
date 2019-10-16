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
        public static bool SecondaryMusicPlaying = false;

        public const string AMB_FORESTCALM = "Forest-Calm";
        public const string AMB_DESERTWINDLIGHT = "Desert-Wind-Light";
        public const string AMB_WINDMODERATE = "Wind-Moderate";
        public const string AMB_CAVEAMBIENCE = "Cave-Ambience";
        public const string AMB_DUNGEONAMBIENCE = "Dungeon-Ambience";
        public const string AMB_ISLANDFOREST = "Island-Forest";
        public const string AMB_WAVESSMALL = "Waves-Small";
        public const string AMB_DARKEMPTINESSDRONE = "Dark-Emptiness-Drone";
        public const string AMB_EERIEECHOES = "Eerie-Echoes";
        public const string AMB_CREEPYDRONE = "Creepy-Drone";
        public const string AMB_JUNGLENIGHT = "Jungle-Night";
        public const string AMB_TROPICALTHUNDER = "Tropical-Thunder";

        public const string SONG_HEROICKINDDOM = "Heroic_Kingdom";
        public const string SONG_BIRTHOFACHAMPION = "Birth_of_a_Champion";
        public const string SONG_NOTHINGMATTERSEVERYTHINGDIES = "Nothing_Matters_Everything_Dies";
        public const string SONG_PANDEMONIUM = "Pandemonium";
        public const string SONG_SERENITY = "Serenity";
        public const string SONG_AMBIENTSOFASIA = "Ambients_of_Asia";

        public enum SoundDirection { None, South, North, West, East, Southwest, Northwest, Southeast, Northeast }

        private static Dictionary<string, SoundEffect> m_soundEffects = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Song> m_songs = new Dictionary<string, Song>();
        public static List<AmbienceAudio> CurrentlyPlayingAmbience = new List<AmbienceAudio>();

        public static float HardSetMediaPlayerVolume
        { get; set; } = 1f;

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

            foreach (AmbienceAudio ambience in new List<AmbienceAudio>(CurrentlyPlayingAmbience))
            {
                if (MediaPlayer.Queue != null && (MediaPlayer.Queue.ActiveSong == null || MediaPlayer.Queue.ActiveSong != ambience.Track))
                {
                    CurrentlyPlayingAmbience.Remove(ambience);
                    ambience.Dispose();
                }
            }

            foreach (AmbienceAudio ambience in new List<AmbienceAudio>(CurrentlyPlayingAmbience))
                ambience.Update(gameTime);

            // Game, CurrentCharacter is not dead.
            if (Client.InGame && Character.CurrentCharacter != null && !Character.CurrentCharacter.IsDead)
            {
                switch (Character.CurrentCharacter.MapID)
                {
                    case (int)World.MapID.Annwn:
                        #region Annwn
                        if (Character.CurrentCharacter.Z >= 0)
                            PlayAmbience(AMB_FORESTCALM, true, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.Z == -20)
                            PlayAmbience(SONG_SERENITY, false, true, HardSetMediaPlayerVolume); // Annwn Town
                        else if (Character.CurrentCharacter.Z < -20)
                            PlayAmbience(AMB_DUNGEONAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else MediaPlayer.Pause();
                        break;
                        #endregion
                    case (int)World.MapID.Axe_Glacier:
                        #region Axe Glacier
                        if (Character.CurrentCharacter.ZName.Contains("Vulcan") || Character.CurrentCharacter.ZName.Contains("Sloping"))
                            MediaPlayer.Pause();
                        else if (Character.CurrentCharacter.ZName != "Caverns of Doom")
                            PlayAmbience(AMB_WINDMODERATE, true, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.ZName == "Caverns of Doom")
                            PlayAmbience(AMB_CAVEAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else MediaPlayer.Pause();
                        break;
                    #endregion
                    case (int)World.MapID.Deep_Kesmai:
                        #region Deep Kesmai
                        if (Character.CurrentCharacter.Z < 0)
                            PlayAmbience(AMB_DUNGEONAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else MediaPlayer.Pause();
                        break;
                    #endregion
                    case (int)World.MapID.Hell:
                        break;
                    case (int)World.MapID.Innkadi:
                        #region Innkadi
                        if (new List<int> { 0, 70, 150 }.Contains(Character.CurrentCharacter.Z))
                            PlayAmbience(AMB_FORESTCALM, true, true, HardSetMediaPlayerVolume);
                        else if (new List<int> { -20, -30, -80 }.Contains(Character.CurrentCharacter.Z))
                            PlayAmbience(AMB_CAVEAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else if (new List<int> { -1530, -1550, -1570 }.Contains(Character.CurrentCharacter.Z))
                            PlayAmbience(AMB_EERIEECHOES, true, true, HardSetMediaPlayerVolume);
                        else MediaPlayer.Pause();
                        break;
                    #endregion
                    case (int)World.MapID.Island_of_Kesmai:
                        #region Island of Kesmai
                        if (Character.CurrentCharacter.Z < 0)
                            PlayAmbience(AMB_DUNGEONAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.Z == 0)
                        {
                            Rectangle docks = new Rectangle(38, 32, 11, 17);

                            if (new Rectangle(Character.CurrentCharacter.X, Character.CurrentCharacter.Y, 1, 1).Intersects(docks))
                                PlayAmbience(AMB_WAVESSMALL, true, true, HardSetMediaPlayerVolume);
                            else
                            {
                                Rectangle thievesGuild = new Rectangle(50, 28, 7, 4);

                                if (!new Rectangle(Character.CurrentCharacter.X, Character.CurrentCharacter.Y, 1, 1).Intersects(thievesGuild))
                                    PlayAmbience(AMB_ISLANDFOREST, true, true, HardSetMediaPlayerVolume);
                                else MediaPlayer.Pause();
                            }
                        }
                        else MediaPlayer.Pause(); // quiet in Kesmai lockers and on Ornic Outpost 2nd level
                        break; 
                    #endregion
                    case (int)World.MapID.Leng:
                        #region Leng
                        if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null)
                        {
                            if (new List<int>() { 0, 15, 25, 80, 100, 130, 200 }.Contains(Character.CurrentCharacter.Z))
                                PlayAmbience(AMB_DESERTWINDLIGHT, true, true, HardSetMediaPlayerVolume);
                            else if(new List<int>() { 115, 125, 135, 145, 180 }.Contains(Character.CurrentCharacter.Z)) // Tower of Darkness, Autocrat's Dungeon
                                PlayAmbience(AMB_DARKEMPTINESSDRONE, true, true, HardSetMediaPlayerVolume);
                            else if(new List<int>() { 390, 420, 430, }.Contains(Character.CurrentCharacter.Z))
                                PlayAmbience(AMB_WINDMODERATE, true, true, HardSetMediaPlayerVolume);
                            else if(Character.CurrentCharacter.Z == 350)
                                PlayAmbience(AMB_FORESTCALM, true, true, HardSetMediaPlayerVolume);
                            else MediaPlayer.Pause();
                        }
                        break; 
                    #endregion
                    case (int)World.MapID.Oakvael:
                        #region Oakvael
                        if (Character.CurrentCharacter.Z >= 0)
                            PlayAmbience(AMB_FORESTCALM, true, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.Z == -280) // temple of the undead
                            PlayAmbience(AMB_CREEPYDRONE, true, true, HardSetMediaPlayerVolume);
                        else if(Character.CurrentCharacter.Z == -150 || Character.CurrentCharacter.Z == -190)
                            PlayAmbience(AMB_EERIEECHOES, true, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.Z == -50 || Character.CurrentCharacter.Z == -170)
                            PlayAmbience(AMB_CAVEAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else PlayAmbience(AMB_DUNGEONAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        break;
                    #endregion
                    case (int)World.MapID.Rift_Glacier:
                        MediaPlayer.Pause();
                        //PlayAmbience(AMB_WINDMODERATE, true, true, HardSetMediaPlayerVolume);
                        break;
                    case (int)World.MapID.Shukumei:
                        if (Math.Abs(Character.CurrentCharacter.Z) == 350) // -350 and 350
                            PlayAmbience(AMB_TROPICALTHUNDER, true, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.Z == 50 || Character.CurrentCharacter.Z == 100)
                            PlayAmbience(SONG_AMBIENTSOFASIA, false, true, HardSetMediaPlayerVolume);
                        else if (Character.CurrentCharacter.Z == 20 || Character.CurrentCharacter.Z == 400)
                            PlayAmbience(AMB_CAVEAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        else MediaPlayer.Pause();
                        break;
                    case (int)World.MapID.Torii:
                        #region Torii
                        if (Character.CurrentCharacter.Z >= 0)
                            PlayAmbience(SONG_NOTHINGMATTERSEVERYTHINGDIES, false, true, HardSetMediaPlayerVolume);
                        else PlayAmbience(AMB_CAVEAMBIENCE, false, true, HardSetMediaPlayerVolume);
                        break;
                    #endregion
                    case (int)World.MapID.Underkingdom:
                        #region Underkingdom
                        if (Character.CurrentCharacter.Z == 0)
                        {
                            Rectangle UnderkingdomTown = new Rectangle(15, 8, 16, 18);
                            if (new Rectangle(Character.CurrentCharacter.X, Character.CurrentCharacter.Y, 1, 1).Intersects(UnderkingdomTown))
                                PlayAmbience(SONG_HEROICKINDDOM, false, true, HardSetMediaPlayerVolume);
                            else PlayAmbience(AMB_DESERTWINDLIGHT, true, true, HardSetMediaPlayerVolume);
                        }
                        else if(Character.CurrentCharacter.Z == -100 || Character.CurrentCharacter.Z == -200)
                            PlayAmbience(AMB_JUNGLENIGHT, true, true, HardSetMediaPlayerVolume);
                        else PlayAmbience(AMB_CAVEAMBIENCE, true, true, HardSetMediaPlayerVolume);
                        break;
                        #endregion
                    default:
                        MediaPlayer.Pause();
                        break;

                }
            }
            else if (Client.GameState == Enums.EGameState.Splash || Client.GameState == Enums.EGameState.Login || Client.GameState == Enums.EGameState.Menu || Client.GameState == Enums.EGameState.CharacterGeneration)
            {
                if (MediaPlayer.Queue != null && (MediaPlayer.Queue.ActiveSong == null || MediaPlayer.Queue.ActiveSong.Name != SONG_BIRTHOFACHAMPION) || MediaPlayer.State != MediaState.Playing)
                {
                    if (MediaPlayer.Queue.ActiveSong != null && MediaPlayer.Queue.ActiveSong.Name == SONG_BIRTHOFACHAMPION)
                        MediaPlayer.Play(MediaPlayer.Queue.ActiveSong);
                    else PlayAmbience(SONG_BIRTHOFACHAMPION, true, true, HardSetMediaPlayerVolume);
                }
            }
            else if (!SecondaryMusicPlaying && !gui.GameHUD.OverrideDisplayStates.Contains(Client.GameState))
            {
                if(MediaPlayer.State == MediaState.Playing)
                {
                    if(CurrentlyPlayingAmbience.Find(a => a.Track == MediaPlayer.Queue.ActiveSong) is AmbienceAudio aa)
                    {
                        aa.FadeIn = false;
                        aa.FadeOut = true;
                    }
                    else MediaPlayer.Pause();
                }
            }

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
            if (!Client.UserSettings.BackgroundAmbience)
                return;

            if (string.IsNullOrEmpty(songName))
                return;

            //if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null && MediaPlayer.Queue.ActiveSong.Name == songName)
            //{
            //    return;
            //}

            //if (MediaPlayer.State != MediaState.Playing && MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null && MediaPlayer.Queue.ActiveSong.Name == songName)
            //{
            //    MediaPlayer.Resume();
            //    return;
            //}

            if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong.Name != songName)
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong.Name == songName)
            {
                return;
            }

            Song song;

            if (!m_songs.ContainsKey(songName))
            {
                song = Program.Client.Content.Load<Song>("Ambience/" + songName);
                m_songs.Add(songName, song);
            }
            else song = m_songs[songName];

            AmbienceAudio ambience = new AmbienceAudio(song, fadeIn, volume);
            ambience.IsSecondarySong = true;

            if (!AmbienceExists(ambience))
                CurrentlyPlayingAmbience.Add(ambience);

            float playerVolume = fadeIn ? .1f : volume;

            SecondaryMusicPlaying = true;
            //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.IsRepeating = repeating;
            MediaPlayer.Volume = playerVolume;
            MediaPlayer.Play(song);
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

            if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null && MediaPlayer.Queue.ActiveSong.Name == songName)
            {
                return;
            }

            if (MediaPlayer.State != MediaState.Playing && MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null && MediaPlayer.Queue.ActiveSong.Name == songName)
            {
                if(repeating)
                    MediaPlayer.Play(MediaPlayer.Queue.ActiveSong);

                return;
            }

            try
            {
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
                else
                {
                    CurrentlyPlayingAmbience.Remove(CurrentlyPlayingAmbience.Find(p => p.Track == song));
                    CurrentlyPlayingAmbience.Add(ambience);
                }

                MediaPlayer.IsRepeating = repeating;
                MediaPlayer.Volume = fadeIn ? .1f : 1f;
                SecondaryMusicPlaying = false;
                MediaPlayer.Play(song);
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                Utils.Log("songName: " + songName);
            }
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
