using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Yuusha
{
    public static class Sound
    {
        public static string CommonSoundClick1 = "0085";
        public static string CommonSoundClick2 = "0086";

        private static Dictionary<string, SoundEffect> m_soundEffects = new Dictionary<string, SoundEffect>();

        public static void Play(string soundName)
        {
            soundName = "KSDN" + soundName;

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

        public static void Play(List<string> soundInfo)
        {
            SoundEffect soundEffect;
            string soundName = "KSND" + soundInfo[0];

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
    }
}
