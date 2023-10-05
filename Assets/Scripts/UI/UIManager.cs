using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance; // 单例引用器
        internal StreamMessager UIMessage_1MSG;
        internal StreamMessager UIMessage_2MSG;

        // 获取单例实例的静态属性
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManager>(); // 查找已存在的实例
                    if (instance == null)
                    {
                        // 如果没有现有实例，创建一个新的GameObject并附加UIManager组件
                        GameObject obj = new GameObject("UIManager");
                        instance = obj.AddComponent<UIManager>();
                    }
                }

                return instance;
            }
        }

        private static StreamMessager FindUIMessage1()
        {
            StreamMessager UIMessage_1MSG = GameObject.Find("UIMessage_1")?.GetComponent<StreamMessager>();

            if (UIMessage_1MSG == null)
            {
                throw new System.Exception("UIMessage_1 not found or Message component missing!");
            }

            return UIMessage_1MSG;
        }

        private static StreamMessager FindUIMessage2()
        {
            StreamMessager UIMessage_2MSG = GameObject.Find("UIMessage_2")?.GetComponent<StreamMessager>();

            if (UIMessage_2MSG == null)
            {
                throw new System.Exception("UIMessage_2 not found or Message component missing!");
            }

            return UIMessage_2MSG;
        }

        private void Awake()
        {
            // 确保只有一个实例存在，如果已经存在实例，则销毁新的实例
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // 保留实例在场景之间的切换中
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UIMessage_1MSG = FindUIMessage1();
            UIMessage_2MSG = FindUIMessage2();
        }

        public void ShowMessage1(string message)
        {
            if (UIMessage_1MSG)
            {
                UIMessage_1MSG.ShowMessage(message);
            }
        }


        public void ShowMessage2(string message)
        {
            if (UIMessage_2MSG)
            {
                UIMessage_2MSG.ShowMessage(message);
            }
        }
    }
}