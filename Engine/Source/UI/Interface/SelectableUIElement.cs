namespace MonolithEngine
{
    interface SelectableUIElement
    {
        public void OnClick();

        public void SetUserInterface(UserInterface userInterface);

        public void OnResolutionChanged();
    }
}
