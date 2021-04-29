using System.Collections.Generic;

namespace MonolithEngine
{
    public interface IScene
    {
        public abstract void Load();

        public abstract void OnStart();

        public abstract void Finish();

        public abstract void OnEnd();

        public abstract void Unload();

        public abstract void OnFinished();

        public ISceneTransitionEffect GetTransitionEffect();

        public abstract ICollection<object> ExportData();

        public abstract void ImportData(ICollection<object> state);

        public void Update();

    }
}
