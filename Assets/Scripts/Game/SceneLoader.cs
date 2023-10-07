// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
//
// namespace Game
// {
//     public class SceneLoader : MonoBehaviour
//     {
//         private Dictionary<string, AsyncOperation> loadedScenes = new Dictionary<string, AsyncOperation>();
//
//         private static SceneLoader instance;
//
//         public static SceneLoader Instance
//         {
//             get
//             {
//             if (instance == null)
//             {
//                 GameObject loaderObject = new GameObject("SceneLoader");
//                 instance = loaderObject.AddComponent<SceneLoader>();
//                 DontDestroyOnLoad(loaderObject);
//             }
//             return instance;
//             }
//         }
//
//         private void Awake()
//         {
//             if (instance != null && instance != this)
//             {
//                 Destroy(this.gameObject);
//             }
//             else
//             {
//                 instance = this;
//                 DontDestroyOnLoad(this.gameObject);
//             }
//         }
//
//         public void PreloadScenes(string[] sceneNames)
//         {
//             foreach (string sceneName in sceneNames)
//             {
//                 AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
//                 asyncOperation.allowSceneActivation = false;
//                 loadedScenes.Add(sceneName, asyncOperation);
//             }
//         }
//
//         public void ActivateScene(string sceneName)
//         {
//             if (loadedScenes.ContainsKey(sceneName))
//             {
//                 AsyncOperation asyncOperation = loadedScenes[sceneName];
//                 asyncOperation.allowSceneActivation = true;
//             }
//         }
//
//         private void InitializeScene(string sceneName)
//         {
//             // Add any initialization logic here.
//             // This can include resetting game object states, clearing data, etc.
//         }
//
//         private void SceneActivated(string sceneName)
//         {
//             // Add scene-specific initialization logic here.
//             // This can include setting up game objects, initializing variables, etc.
//         }
//     }
// }