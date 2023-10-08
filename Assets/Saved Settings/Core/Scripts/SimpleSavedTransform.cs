using UnityEngine;

namespace SavedSettings
{
    /// <summary>
    /// A simple example class for using the SaveHelper in a scene to save/load a gameobject's position and rotation.
    /// </summary>
    public class SimpleSavedTransform : Saved
    {
        /// <summary>
        /// Saved the game object's data. You must use JsonUtility.ToJson() to convert your data to a string and return it.
        /// </summary>
        public override string Save()
        {
            return JsonUtility.ToJson(new savedTransform(transform.position, transform.rotation));
        }

        /// <summary>
        /// Loads the game object's data. You must use JsonUtility.FromJson() to convert the string to your loadable data.
        /// </summary>
        public override void Load(string data, float version)
        {
            savedTransform trans = JsonUtility.FromJson<savedTransform>(data);
            transform.SetPositionAndRotation(trans.pos, trans.rot);
        }

        /// <summary>
        /// The saved position and rotation.
        /// </summary>
        struct savedTransform
        {
            public Vector3 pos;
            public Quaternion rot;
            public savedTransform(Vector3 aPos, Quaternion aRot)
            {
                pos = aPos;
                rot = aRot;
            }
        }
    }
}