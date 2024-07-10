using UnityEngine;

[RequireComponent(typeof(Camera))]
public class UICameraProvider : MonoBehaviour
{
    public Camera Camera;
    public string Tag;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }
}
