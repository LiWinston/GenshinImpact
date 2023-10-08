using UnityEngine;

namespace SavedSettings.Test
{
    public class TestKeysMove : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] PlayerKeyBindings _Bindings;
#pragma warning restore 649

        // Basic movement using the default key bindings.
        void Update()
        {
            transform.position += new Vector3(_Bindings.Keys["Right"].AxisInput(_Bindings.Keys["Left"]),
                                                _Bindings.Keys["Up"].AxisInput(_Bindings.Keys["Down"]),
                                                0f)
                                                * Time.deltaTime;

            if (_Bindings.Keys["Jump"].Down)
            {
                Debug.Log("Jump Pressed");
            }
        }
    }
}
