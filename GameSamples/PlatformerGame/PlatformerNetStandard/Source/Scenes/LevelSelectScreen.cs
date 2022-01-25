using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class LevelSelectScreen : AbstractScene
    {
        public LevelSelectScreen() : base("LevelSelect", preload: true)
        {

        }

        public override ICollection<object> ExportData()
        {
            return null;
        }

        public override ISceneTransitionEffect GetTransitionEffect()
        {
            return null;
        }

        public override void ImportData(ICollection<object> state)
        {

        }

        public override void Load()
        {
            float scale = 3f;
            Logger.Debug("Loading level select scene UI...");
            Texture2D texture = Assets.GetTexture2D("Level1Base");
            GameButton level1Button = new GameButton(texture, new Vector2(50, 25), scale: scale, null, true);
            level1Button.HoverSoundEffectName = "MenuHover";
            level1Button.SelectSoundEffectName = "MenuSelect";

            level1Button.OnClick = () =>
            {
                SceneManager.LoadScene("Level_1");
            };

            texture = Assets.GetTexture2D("Level2Base");
            GameButton level2Button = new GameButton(texture, new Vector2(50, 75), scale: scale, null, true);
            level2Button.HoverSoundEffectName = "MenuHover";
            level2Button.SelectSoundEffectName = "MenuSelect";

            level2Button.OnClick = () =>
            {
                SceneManager.LoadScene("Level_2");
            };

            GameButton back = new GameButton(Assets.GetTexture2D("HUDBackBase"), new Vector2(15, 5), scale: scale / 2.5f);
            back.HoverSoundEffectName = "MenuHover";
            back.SelectSoundEffectName = "MenuSelect";

            back.OnClick = () =>
            {
                SceneManager.StartScene("MainMenu");
            };

            texture = Assets.GetTexture2D("ForestBG");
            Vector2 pos = MonolithGame.Platform.IsDesktop() ? new Vector2(-80, -60) : new Vector2(-57, -37);
            Image forestBg = new Image(texture, null, pos, new Rectangle(0, (int)texture.Height / 2, (int)texture.Width, (int)texture.Height / 2));
            UI.AddUIElement(forestBg);

            texture = Assets.GetTexture2D("IcyBG");
            Image icyBg = new Image(texture, null, new Vector2(0, 45), new Rectangle(0, (int)texture.Height / 2, (int)texture.Width, (int)texture.Height / 2));
            UI.AddUIElement(icyBg);

            UI.AddUIElement(level1Button);
            UI.AddUIElement(level2Button);
            UI.AddUIElement(back);
        }

        public override void OnEnd()
        {

        }

        public override void OnStart()
        {
            PlatformerGame.Paused = true;
        }

        public override void OnFinished()
        {

        }
    }
}
