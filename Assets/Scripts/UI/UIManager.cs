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
        private static Messager FindUIMessage2()
        {
            Messager UIMessage_2MSG = GameObject.Find("UIMessage_2")?.GetComponent<Messager>();

            if (UIMessage_2MSG == null)
            {
                throw new System.Exception("UIMessage_2 not found or Message component missing!");
            }

            return UIMessage_2MSG;
        }

        public static void ShowMessage1(string message)
        {
            Messager UIMessage_1MSG = FindUIMessage1();
            UIMessage_1MSG.ShowMessage(message);
        }
        
        
        public static void ShowMessage2(string message)
        {
            Messager UIMessage_1MSG = FindUIMessage1();
            UIMessage_1MSG.ShowMessage(message);
        }
    }
}