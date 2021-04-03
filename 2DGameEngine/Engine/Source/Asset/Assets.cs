using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonolithEngine.Engine.Source.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Asset
{
    public class Assets
    {
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        private static Dictionary<string, List<Texture2D>> textureGroups = new Dictionary<string, List<Texture2D>>();

        private static Dictionary<string, SoundEffectInstance> soundEffects = new Dictionary<string, SoundEffectInstance>();

        private static Dictionary<string, Song> songs = new Dictionary<string, Song>();

        public static void LoadTexture(string name, string path, bool flipVertical = false, bool flipHorizontal = false)
        {
            if (flipVertical || flipHorizontal)
            {
                textures.Add(name, AssetUtil.FlipTexture(AssetUtil.LoadTexture(path), flipVertical, flipHorizontal));
            } else
            {
                textures.Add(name, AssetUtil.LoadTexture(path));
            }
        }

        public static Texture2D GetTexture(string name)
        {
            return textures[name];
        }

        public static Texture2D LoadAndGetTexture(string name, string path)
        {
            LoadTexture(name, path);
            return textures[name];
        }

        public static List<Texture2D> GetTextureGroup(string name)
        {
            return textureGroups[name];
        }

        public static void LoadTextureGroup(string name, List<string> paths)
        {
            List<Texture2D> result = new List<Texture2D>();
            foreach (string path in paths)
            {
                result.Add(AssetUtil.LoadTexture(path));
            }
            textureGroups.Add(name, result);
        }

        public static List<Texture2D> LoadAndGetTextureGroup(string name, List<string> paths)
        {
            LoadTextureGroup(name, paths);
            return textureGroups[name];
        }

        public static void LoadSoundEffect(string name, string path, bool isLooped = false)
        {
            SoundEffectInstance instance = AssetUtil.LoadSoundEffect(path).CreateInstance();
            instance.IsLooped = isLooped;
            soundEffects[name] = instance;
        }

        public static void LoadSong(string name, string path)
        {
            songs[name] = AssetUtil.LoadSong(path);
        }

        public static SoundEffectInstance GetSoundEffect(string name)
        {
            return soundEffects[name];
        }

        public static void PlaySoundEffect(string name, bool waitForFinish = false)
        {
            if (waitForFinish)
            {
                soundEffects[name].Play();
            } else
            {
                SoundEffectInstance instance = soundEffects[name];
                SoundState currentState = instance.State;
                if (currentState == SoundState.Playing)
                {
                    instance.Stop();
                }
                instance.Play();
            }
        }

        public static Song GetSong(string name)
        {
            return songs[name];
        }

        public static Texture2D CreateRectangle(int size, Color color)
        {
            return AssetUtil.CreateRectangle(size, color);
        }

        public static Texture2D CreateCircle(int diameter, Color color, bool filled = false)
        {
            return AssetUtil.CreateCircle(diameter, color, filled);
        }
    }
}
