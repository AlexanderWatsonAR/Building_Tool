using UnityEngine;

public class Drawable : MonoBehaviour, IDrawable
{
    [SerializeField] PolygonPath m_Path;
    public PlanarPath Path => m_Path;

    Drawable()
    {
        m_Path = new PolygonPath(Vector3.up);
    }
}

public interface IDrawable
{
    PlanarPath Path { get; }
}
