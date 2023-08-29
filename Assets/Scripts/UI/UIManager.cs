using UnityEngine;

namespace UI
{
    public static class UIManager
    {
        private static Messager FindUIMessage1()
        {
            Messager UIMessage_1MSG = GameObject.Find("UIMessage_1")?.GetComponent<Messager>();

            if (UIMessage_1MSG == null)
            {
                throw new System.Exception("UIMessage_1 not found or Message component missing!");
            }

            return UIMessage_1MSG;
        }

        public static void ShowMessage1(string message)
        {
            Messager UIMessage_1MSG = FindUIMessage1();
            UIMessage_1MSG.ShowMessage(message);
        }
    }
}