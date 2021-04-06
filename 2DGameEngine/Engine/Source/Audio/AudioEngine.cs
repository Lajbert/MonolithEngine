using MonolithEngine.Engine.Source.Audio;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace MonolithEngine.Engine.Source.Audio
{
    public class AudioEngine
    {

        private static Dictionary<string, AudioEntry> audioCache = new Dictionary<string, AudioEntry>();

        internal static void OnAudioConfigChanged()
        {

        }

        public static void AddSound(string name, string path, bool isLooped = false, AudioTag audioTag = AudioTag.SOUND_EFFECT, bool isMuted = false, float maxVolume = 1)
        {
            if (maxVolume < 0 || maxVolume > 1)
            {
                throw new Exception("Max volume should be a value between 0 and 1");
            }
            SoundEffectInstance audio = AssetUtil.LoadSoundEffect(path).CreateInstance();
            audio.IsLooped = isLooped;
            audioCache.Add(name, new AudioEntry(audio, audioTag, isMuted, maxVolume));
        }

        public static void Pause(string name)
        {
            audioCache[name].SoundEffect.Pause();
        }

        public static void Play(string name, bool waitForFinish = false)
        {
            if (audioCache[name].IsMuted)
            {
                return;
            }

            SoundEffectInstance soundEffect = audioCache[name].SoundEffect;

            if (waitForFinish)
            {
                soundEffect.Play();
            }
            else
            {
                SoundState currentState = soundEffect.State;
                if (currentState == SoundState.Playing)
                {
                    soundEffect.Stop();
                }
                soundEffect.Play();
            }
        }

        public static void Stop(string name)
        {
            audioCache[name].SoundEffect.Stop();
        }

        public static void SetVolume(AudioTag tag, float volume)
        {
            foreach (AudioEntry audioEntry in audioCache.Values)
            {
                if (audioEntry.Tag == tag)
                {
                    audioEntry.SoundEffect.Volume = volume * audioEntry.MaxVolume;
                }
            }
        }

        public static void MuteAll()
        {
            foreach (AudioTag tag in Enum.GetValues(typeof(AudioTag)))
            {
                ToggleMuteWithTag(tag, true);
            }
        }

        public static void ToggleMuteWithTag(AudioTag tag, bool muted)
        {
            foreach (AudioEntry audioEntry in audioCache.Values)
            {
                if (audioEntry.Tag == tag)
                {
                    audioEntry.IsMuted = muted;
                }
            }
        }

        public static void UnMuteAll()
        {
            foreach (AudioTag tag in Enum.GetValues(typeof(AudioTag)))
            {
                ToggleMuteWithTag(tag, false);
            }
        }

        private class AudioEntry
        {

            public string Name;

            public AudioTag Tag;

            public SoundEffectInstance SoundEffect;

            public bool IsMuted = false;

            public float MaxVolume;

            public AudioEntry(SoundEffectInstance soundEffect, AudioTag tag, bool isMuted, float maxVolume)
            {
                Tag = tag;
                SoundEffect = soundEffect;
                IsMuted = isMuted;
                MaxVolume = maxVolume;
            }
        }
    }
}
