using UnityEngine;

public class Jumpability : MonoBehaviour
{
    [SerializeField]
    private float jumpHeight = 0.7f;

    public float JumpHeight
    {
        get { return jumpHeight; }
        set { jumpHeight = value; }
    }
}
