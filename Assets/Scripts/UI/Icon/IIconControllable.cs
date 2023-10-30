using UnityEngine;

namespace UI
{
    public interface IIconControllable
    {
        void ShowOn();
        void ShowOff();
        void ShowKeyBinding();
        KeyCode KeyBinding { set; }
    }
}