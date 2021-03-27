using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.UI.Interface
{
    interface SelectableUIElement
    {
        public void OnClick();

        public void SetUserInterface(UserInterface userInterface);
    }
}
