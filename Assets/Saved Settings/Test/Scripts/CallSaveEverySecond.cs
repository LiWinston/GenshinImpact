using System.Collections;
using UnityEngine;

namespace SavedSettings.Test
{
    public class CallSaveEverySecond : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] SaveHelper saveload;
#pragma warning restore 649

        /// <summary>
        /// Loads the saved scene objects. Loads the rotation and local scale of this object.
        /// On windows 10 these files should save to C:\Users\Your Name\AppData\LocalLow\DefaultCompany
        /// </summary>
        void Start()
        {
            saveload.LoadScene("TestFolder");

            SaveData data = SaveHelper.Load("TestRotation");
            if (!string.IsNullOrEmpty(data.data))
            {
                transform.rotation = JsonUtility.FromJson<Quaternion>(data.data);
            }

            data = SaveHelper.Load("TestFolder2/TestScale");
            if (!string.IsNullOrEmpty(data.data))
            {
                transform.localScale = JsonUtility.FromJson<Vector3>(data.data);
            }

            StartCoroutine(SaveEverySecond());
        }

        /// <summary>
        /// Test the other save functions, such as deleting or making copies of files.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                SaveHelper.DeleteSceneFile("TestFolder");
                SaveHelper.DeleteFile("TestRotation");
                SaveHelper.DeleteFolder("TestFolder2");
                enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SaveHelper.CopyFile("TestRotation", "TestRotation2");
                SaveHelper.CopyFolder("TestFolder2", "TestFolder3");
                enabled = false;
            }
        }

        /// <summary>
        /// Saves the scene, the rotation of this object, and the localscale of this object every second.
        /// </summary>
        private IEnumerator SaveEverySecond()
        {
            //For this test, use the application version used in Android builds (set in player settings).
            float version = float.Parse(Application.version);

            while (enabled)
            {
                yield return new WaitForSeconds(1);

                saveload.SaveScene("TestFolder", version);

                SaveHelper.Save("TestRotation", JsonUtility.ToJson(transform.rotation), version);

                SaveHelper.SaveIntoDirectory("TestFolder2", "TestScale", JsonUtility.ToJson(transform.localScale), version);
            }
        }
    }
}
