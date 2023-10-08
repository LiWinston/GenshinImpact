using UnityEngine.SceneManagement;

namespace SavedSettings
{
    /// <summary>
    /// Contains public static helper functions for changing scenes.
    /// </summary>
    public static class SceneHelper
    {

        /// <summary>
        /// Reloads the current scene.
        /// </summary>
        public static void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Loads the given level.
        /// Checks if the level exists.
        /// </summary>
        public static void LoadScene(int level)
        {
            if (level >= 0 && level < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(level);
            }
        }

        /// <summary>
        /// Loads the final scene in build settings.
        /// </summary>
        public static void FinalScene()
        {
            if (SceneManager.sceneCountInBuildSettings > 0)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }
        }
    }
}