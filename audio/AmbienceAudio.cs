using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Yuusha.Audio
{
    public class AmbienceAudio : GameComponent
    {
        public static float FadeSpeed = .01f;

        public string Name
        { get { return Track.Name; } }
        public Song Track
        { get; set; }
        public float Volume
        { get; set; } = 1.0f;
        public bool IsSecondarySong
        { get; set; }
        public bool FadeIn
        { get; set; } = false;
        public bool FadeOut
        { get; set; } = false;


        public AmbienceAudio(Song song, bool fadeIn, float volume) : base(Program.Client)
        {
            Track = song;
            FadeIn = fadeIn;
            Volume = volume;
        }

        public AmbienceAudio(Song song) : base(Program.Client)
        {
            Track = song;
        }

        public override void Update(GameTime gameTime)
        {
            if(!Client.UserSettings.AudioEnabled)
            {
                if(!MediaPlayer.IsMuted) MediaPlayer.IsMuted = true;
                return;
            }

            //if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null)
            //{
            //    if ((MediaPlayer.Queue.ActiveSong.Name != Track.Name) || (MediaPlayer.Queue.ActiveSong.Name == Track.Name && MediaPlayer.State != MediaState.Playing))
            //    {
            //        if (IsSecondarySong)
            //            AudioManager.SecondaryMusicPlaying = false;

            //        AudioManager.CurrentlyPlayingAmbience.Remove(this);
            //        return;
            //    }
            //}

            base.Update(gameTime);

            if (FadeIn && MediaPlayer.Volume < Volume)
            {
                MediaPlayer.Volume += FadeSpeed;
                if (MediaPlayer.Volume >= 1f) FadeIn = false;
            }
            else if (FadeOut)
            {
                MediaPlayer.Volume -= FadeSpeed;
            }

            if (MediaPlayer.Volume > Volume)
                MediaPlayer.Volume = Volume;

            if (FadeOut && MediaPlayer.Volume < .1f)
            {
                FadeOut = false;
                MediaPlayer.Volume = AudioManager.HardSetMediaPlayerVolume;
                if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue.ActiveSong == Track)
                    MediaPlayer.Stop();
            }
        }
    }
}
