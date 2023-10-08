using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

#if !UNITY_EDITOR
using System.Security.Cryptography;
#endif

namespace SavedSettings
{
    /// <summary>
    /// Abstract class for objects that want to saved when the scene saves.
    /// </summary>
    [System.Serializable]
    public abstract class Saved : MonoBehaviour
    {
        /// <summary>
        /// Saved the game object's data. You must use JsonUtility.ToJson() to convert your data to a string and return it.
        /// </summary>
        /// <returns>Saved data of the object.</returns>
        public abstract string Save();

        /// <summary>
        /// Loads the game object's data. You must use JsonUtility.FromJson() to convert the string to your loadable data.
        /// </summary>
        public abstract void Load(string data, float version);
    }

    /// <summary>
    /// Wrapper for json data to be saved or loaded, and the version of the application that data will/is saved in.
    /// </summary>
    public struct SaveData
    {
        public string data;
        public float version;

        public SaveData(string saveData, float saveVersion)
        {
            data = saveData;
            version = saveVersion;
        }
    }

    /// <summary>
    /// Acts as a helper class with public static functions for saving files to the game's appData folder.
    /// If attached to a gameobject in the scene, can be used to save and load scene gameobjects.
    /// </summary>
    public class SaveHelper : MonoBehaviour
    {
        /// <summary>
        /// Local gameobject to save
        /// </summary>
#pragma warning disable 649
        [SerializeField] Saved[] _ToSave;
#pragma warning restore 649

        //Password of the saved data when crypto'd
        const string PASSWORD = "sohmsakoh&WTAMGaga8sg89&(*&a09gLghklahklal/To";

#if UNITY_EDITOR
        public const string SAVE_FORMAT = "_Editor.json";
#else
    public const string SAVE_FORMAT = ".json";
#endif

        /// <summary>
        /// Saves all saveable gameobjects this component has reference to.
        /// </summary>
        /// <returns>If the save succeeded.</returns>
        public bool SaveScene(string subFolderName, float version)
        {
            string[] data = new string[_ToSave.Length];
            for (int i = 0; i < _ToSave.Length; ++i)
            {
                data[i] = _ToSave[i].Save();
            }
            return SaveIntoDirectory(subFolderName, SceneManager.GetActiveScene().name, JsonArray.arrayToJson<string>(data), version);
        }

        /// <summary>
        /// Loads the scene's savedata and sends it to all local saveable objects this component has reference to.
        /// </summary>
        public bool LoadScene(string subFolderName)
        {
            StringBuilder builder = new StringBuilder(subFolderName);
            builder.Append('/');
            builder.Append(SceneManager.GetActiveScene().name);
            SaveData saved = Load(builder.ToString());

            // If the scene data is missing, reset the scene save data.
            if (string.IsNullOrEmpty(saved.data))
            {
                return SaveScene(subFolderName, 0f);
            }

            try
            {
                string[] json = JsonArray.jsonToArray<string>(saved.data);
                for (int i = 0; i < json.Length; ++i)
                {
                    _ToSave[i].Load(json[i], saved.version);
                }
                return true;
            }
            catch
            {
#if UNITY_EDITOR
                Debug.LogError(SceneManager.GetActiveScene().name + " failed to load save data.");
#endif
                return false;
            }
        }

        /// <summary>
        /// Saves json data to a file in the game's file folder.
        /// The data provided must first be parsed using JsonUtility.ToJson() before being sent.
        /// 
        /// To reset a save file, you can overwrite the file's save data with an empty string.
        /// As long as new saves are handled correctly, the save file will be reset when that file is next loaded.
        /// </summary>
        /// <param name="path">Name of the file.</param>
        /// <param name="data">The data to be saved. The data provided must first be parsed using JsonUtility.ToJson() before being sent.</param>
        /// <param name="version">A version set by you that will remain with the saved data when loaded.</param>
        /// <returns>If the save succeeded.</returns>
        public static bool Save(string path, string data, float version)
        {
            // Change the file path to save/load a different version in the editor.
            StringBuilder builder = new StringBuilder(Application.persistentDataPath);
            builder.Append('/');
            builder.Append(path);
            builder.Append(SAVE_FORMAT);
            SaveData saveData = new SaveData(data, version);

            // Use cryptography to make the save file unreadable. Unless you are in the editor, then leave it readable.
#if UNITY_EDITOR
            try
            {
                File.WriteAllText(builder.ToString(), JsonUtility.ToJson(saveData));
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#else
        try
        {
            File.WriteAllText(builder.ToString(), Cryptography.CryptoHelper.Encrypt(JsonUtility.ToJson(saveData), PASSWORD));
            return true;
        }
        catch
        {
            return false;
        }
#endif
        }

        /// <summary>
        /// Checks if a directory exists and creates it if it does not.
        /// Then calls Save() - See this function for more information.
        /// </summary>
        /// <param name="directory">Name of the folder to save in under the game's folder.</param>
        /// <param name="path">Name of the file.</param>
        /// <param name="data">The data to be saved. The data provided must first be parsed using JsonUtility.ToJson() before being sent.</param>
        /// <param name="version">A version set by you that will remain with the saved data when loaded.</param>
        /// <returns>If the save succeeded.</returns>
        public static bool SaveIntoDirectory(string directory, string name, string data, float version)
        {
            StringBuilder builder = new StringBuilder(Application.persistentDataPath);
            builder.Append('/');
            builder.Append(directory);
            string temp = builder.ToString();
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            builder.Length = 0;
            builder.Append(directory);
            builder.Append('/');
            builder.Append(name);
            return Save(builder.ToString(), data, version);
        }

        /// <summary>
        /// Loads a saved file in the game's folder.
        /// You will have to use JsonUtility.FromJson() to convert the string to your saved data.
        /// 
        /// Can be used to load files within subfolders if a valid path is given.
        /// If the loading fails, an empty string and version 0 are returned.
        /// </summary>
        /// <param name="path">The path of the file within the game's save folder.</param>
        /// <returns>The loaded data.</returns>
        public static SaveData Load(string path)
        {
            // Change the file path to save/load a different version in the editor.
            StringBuilder builder = new StringBuilder(Application.persistentDataPath);
            builder.Append('/');
            builder.Append(path);
            builder.Append(SAVE_FORMAT);

            // Attempt to load the file
            try
            {
                string json = File.ReadAllText(builder.ToString());
                if (!string.IsNullOrEmpty(json))
                {
                    // Decrypt the data. If data was saved in the editor, then the data wasn't cryto'd.
#if UNITY_EDITOR
                    return JsonUtility.FromJson<SaveData>(json);
#else
                return JsonUtility.FromJson<SaveData>(Cryptography.CryptoHelper.Decrypt(json, PASSWORD));
#endif
                }

                // Reuturn empty save data if the data is empty.
                return new SaveData();
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                if (File.Exists(path))
                {
                    Debug.LogError("File failed to load at: " + path);
                    Debug.LogException(e);
                }

                // Return empty save data if the loading failed.
                return new SaveData();
            }
#else
        catch
        {
            // Return empty save data if the loading failed.
            return new SaveData();
        }
#endif
        }

        /// <summary>
        /// Delete's a scene save in a given subfolder.
        /// </summary>
        public static bool DeleteSceneFile(string subFolderName)
        {
            StringBuilder builder = new StringBuilder(subFolderName);
            builder.Append('/');
            builder.Append(SceneManager.GetActiveScene().name);
            return DeleteFile(builder.ToString());
        }

        /// <summary>
        /// Delete's the given file.
        /// </summary>
        public static bool DeleteFile(string path)
        {
            try
            {
                StringBuilder builder = new StringBuilder(Application.persistentDataPath);
                builder.Append('/');
                builder.Append(path);
                builder.Append(SAVE_FORMAT);
                File.Delete(builder.ToString());
                return true;
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#else
        catch
        {
            return false;
        }
#endif
        }

        /// <summary>
        /// Delete's the given folder and all files within it.
        /// </summary>
        public static bool DeleteFolder(string path)
        {
            StringBuilder builder = new StringBuilder(Application.persistentDataPath);
            builder.Append('/');
            builder.Append(path);
            path = builder.ToString();

            try
            {
                // Delete all files in the directory.
                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < files.Length; ++i)
                {
                    File.Delete(files[i]);
                }

                // Delete the directory.
                Directory.Delete(path);
                return true;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogException(e);
#endif
                // The directory either doesn't exist or deleting the directory failed.
                return false;
            }
        }

        /// <summary>
        /// Creates a copy of a file at a given path.
        /// </summary>
        /// <returns>If the copy succeeded.</returns>
        public static bool CopyFile(string fromPath, string toPath)
        {
            try
            {
                StringBuilder builder = new StringBuilder(Application.persistentDataPath);
                builder.Append('/');
                String temp = builder.ToString();
                File.Copy(temp + fromPath + SAVE_FORMAT,
                            temp + toPath + SAVE_FORMAT,
                            true);
                return true;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogException(e);
#endif
                return false;
            }
        }

        /// <summary>
        /// Creates a copy of a folder at a given path and copies all the files within.
        /// </summary>
        /// <returns>If the copy succeeded.</returns>
        public static bool CopyFolder(string fromPath, string toPath)
        {
            try
            {
                StringBuilder builder = new StringBuilder(Application.persistentDataPath);
                builder.Append('/');
                toPath = builder.ToString() + toPath;

                // Get all the files in the original folder.
                string[] files = Directory.GetFiles(builder.ToString() + fromPath);

                // Create the new folder
                if (!Directory.Exists(toPath))
                {
                    Directory.CreateDirectory(toPath);
                }
                toPath += '/';

                // Copy all the files in the old directory to the new one
                for (int i = 0; i < files.Length; ++i)
                {
                    File.Copy(files[i], toPath + Path.GetFileName(files[i]));
                }

                return true;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogException(e);
#endif
                // Copying failed
                return false;
            }
        }
    }
}