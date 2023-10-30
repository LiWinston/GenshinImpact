using UnityEngine;

namespace UI
{
    public interface IIconControllable
    {
        void ShowOn();
        void ShowOff();
        void ShowKeyBinding(float time = 2.0f);
        KeyCode KeyBinding { set; }
        bool IsElapsing { set; }
    }
}