using UnityEngine;

public class CellGraphicController : MonoBehaviour
{
    [SerializeField] private MeshRenderer Renderer;

    public Vector3 Position
    {
        get => this.transform.position;
        set => this.transform.position = value;
    }

    public void SetTargetFlash(bool on)
    {
        Renderer.material.SetInt("_Targeted", on ? 1 : 0);
    }
}
