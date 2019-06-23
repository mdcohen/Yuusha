using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Yuusha.Audio
{
    public class AmbienceAudio : GameComponent
    {
        private bool m_fadeIn = false;

        public string Name
        { get { return Track.Name; } }
        public Song Track
        { get; set; }
        public float Volume
        { get; set; } = 1.0f;
        

        public AmbienceAudio(Song song, bool fadeIn, float volume) : base(Program.Client)
        {
            Track = song;
            m_fadeIn = fadeIn;
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

            if (MediaPlayer.Queue != null && MediaPlayer.Queue.ActiveSong != null)
                if (MediaPlayer.Queue.ActiveSong.Name != Track.Name) return;

            base.Update(gameTime);

            if (m_fadeIn && MediaPlayer.Volume < Volume)
                MediaPlayer.Volume += .1f;

            if (MediaPlayer.Volume > Volume)
                MediaPlayer.Volume = Volume;
        }
    }
}
