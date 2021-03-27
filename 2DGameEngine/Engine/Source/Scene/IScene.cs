using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine.Engine.Source.Scene.Transition;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MonolithEngine.Engine.Source.Scene
{
    public interface IScene
    {
        public abstract void Load();

        public abstract void OnStart();

        public abstract void Finish();

        public abstract void OnEnd();

        public abstract void Unload();

        public ISceneTransitionEffect GetTransitionEffect();

        public abstract ICollection<object> ExportData();

        public abstract void ImportData(ICollection<object> state);

        public void Update();

    }
}
