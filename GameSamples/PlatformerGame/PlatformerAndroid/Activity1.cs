using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using ForestPlatformerExample;
using Microsoft.Xna.Framework;
using MonolithEngine;

namespace PlatformerAndroid
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.ReverseLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Activity1 : AndroidGameActivity
    {
        private PlatformerGame _game;
        private View _view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new PlatformerGame(Platform.ANDROID);
            _view = _game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);
            _game.Run();
        }
    }
}
