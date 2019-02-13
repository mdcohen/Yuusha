using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public static class Sound
    {
        private static AudioEngine audioEngine;
        private static WaveBank waveBank;
        private static SoundBank soundBank;

        public static string CommonSoundClick1 = "0085";
        public static string CommonSoundClick2 = "0086";


        public static List<Cue> m_liveCues = new List<Cue>();

        public static bool IsNotPlaying(Cue cue)
        {
            return !cue.IsPlaying;
        }

        public static void Update(GameTime gameTime)
        {
            if (audioEngine != null)
                audioEngine.Update();
            if (m_liveCues.TrueForAll(IsNotPlaying))
                m_liveCues.Clear();
        }

        public static Cue GetCue(string sound)
        {
            try
            {
                Cue cue = soundBank.GetCue(sound);
                return cue;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="sound">Which sound to play</param>
        public static void Play(string sound)
        {
            try
            {
                Cue cue = soundBank.GetCue("KSND" + sound);
                m_liveCues.Add(cue);
                cue.Play();
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Plays a sound using sound information [0]=sound [1]=distance [2]=direction
        /// </summary>
        /// <param name="soundInfo"></param>
        public static void Play(string[] soundInfo)
        {
            //AudioEmitter audioEmitter = new AudioEmitter();
            //AudioListener audioListener = new AudioListener();

            //audioListener.Position = Vector3.Zero;

            //float x = 0;
            //float y = 0;
            //float z = 0;

            //if (soundInfo.Length > 2)
            //{
            //    int distance = Convert.ToInt32(soundInfo[1]);
            //    distance = distance * 10000;

            //    Map.Direction direction = (Map.Direction)(Convert.ToInt32(soundInfo[2]));

            //    switch (direction)
            //    {
            //        case Map.Direction.North:
            //            y = distance;
            //            break;
            //        case Map.Direction.South:
            //            y = -distance;
            //            break;
            //        case Map.Direction.East:
            //            x = distance;
            //            break;
            //        case Map.Direction.West:
            //            x = -distance;
            //            break;
            //        case Map.Direction.Northeast:
            //            x = distance;
            //            y = distance;
            //            break;
            //        case Map.Direction.Southeast:
            //            x = distance;
            //            y = -distance;
            //            break;
            //        case Map.Direction.Northwest:
            //            x = -distance;
            //            y = distance;
            //            break;
            //        case Map.Direction.Southwest:
            //            x = -distance;
            //            y = -distance;
            //            break;
            //        case Map.Direction.None:
            //        default:
            //            break;
            //    }
            //}

            //audioEmitter.Position = new Vector3(x, y, z);
            if (m_liveCues.Count > 5)
            {
                int cueCount = 0;
                foreach (Cue cue in m_liveCues)
                {
                    if (cue.Name == "KSND" + soundInfo[0])
                        cueCount++;
                    if (cueCount > 5)
                    {
                        //Utils.LogOnce("More than 5 cues are playing for sound: KSND" + soundInfo[0] + ". Cue not played.");
                        return;
                    }
                }
            }

            try
            {
                Cue cue = soundBank.GetCue("KSND" + soundInfo[0]);
                //Utils.Log("KSND" + soundInfo[0] + " soundInfo[1] = " + soundInfo[1] + ", soundInfo[2] = " + soundInfo[2]);
                //cue.Apply3D(audioListener, audioEmitter);
                try
                {
                    cue.SetVariable("Distance", (float)Convert.ToDouble(soundInfo[1]));
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }
                //Utils.Log("Distance variable = " + cue.GetVariable("Distance").ToString());
                m_liveCues.Add(cue);
                cue.Play();
                
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Stops a previously playing cue (the cue will be garbage collected).
        /// </summary>
        /// <param name="cue">The cue to stop that you got returned from Play(sound)</param>
        public static void Stop(Cue cue)
        {
            cue.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Pauses a playing cue (the cue will not be garbage collected).
        /// </summary>
        /// <param name="cue"></param>
        public static void Pause(Cue cue)
        {
            cue.Pause();
        }

        /// <summary>
        /// Starts up the sound code.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                
                //audioEngine = new AudioEngine((Utils.GetMediaFile("\\sounds\\Win\\Yuusha.xgs")));
                //waveBank = new WaveBank(audioEngine, Utils.GetMediaFile("\\sounds\\Win\\Wave Bank.xwb"));
                //waveBank = new WaveBank(audioEngine, Utils.GetMediaFile("\\sounds\\Wave Bank.xwb"), 0, 4);
                //soundBank = new SoundBank(audioEngine, Utils.GetMediaFile("\\sounds\\Win\\Sound Bank.xsb"));

                audioEngine = new AudioEngine("Content\\Yuusha.xgs");
                waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
                //waveBank = new WaveBank(audioEngine, Utils.GetMediaFile("\\sounds\\Wave Bank.xwb"), 0, 4);
                soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Shuts down the sound code tidily.
        /// </summary>
        public static void Shutdown()
        {
            if(soundBank != null)
                soundBank.Dispose();
            if(waveBank != null)
                waveBank.Dispose();
            if(audioEngine != null)
                audioEngine.Dispose();
        }
    }
}
