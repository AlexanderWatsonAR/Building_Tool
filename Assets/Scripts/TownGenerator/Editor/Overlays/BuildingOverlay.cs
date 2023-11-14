using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using UnityEngine.Rendering;

[Overlay(typeof(SceneView), "Building", true)]
public class BuildingOverlay : Overlay, ITransientOverlay
{
    [SerializeField] private Building m_Building;

    public override VisualElement CreatePanelContent()
    {
        if (m_Building == null)
            return new VisualElement();

        var root = new VisualElement() { name = "Building Root"};

        //SquareVisualElement square = new SquareVisualElement()
        //{
        //    style = 
        //    {
        //        position = Position.Absolute,
        //        minHeight = 200, minWidth = 200,
        //        left = 20, top = 20, width = 200, height = 200,
        //    }
        //};

        TexturedElement texturedElement = new() { name = "Textured Element"};
        Image image = new Image();
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/tex.png");
        image.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(200, 200)), Vector2.zero);

        root.Add(image);
        //root.Add(new Label("This is a label visual element placed inside the square visual element"));

        
        return root;
    }

    public bool visible
    {
        get
        {
            if (Selection.activeGameObject != null)
            {
                bool isBuilding = Selection.activeGameObject.TryGetComponent(out Building building);

                if (isBuilding)
                {
                    m_Building = building;

                }
                return isBuilding;
            }
            return false;
        }
    }
}

class SquareVisualElement : VisualElement
{

    public SquareVisualElement()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        // Idea: Probuilder could generate the mesh data from the control points.

        MeshWriteData mesh = mgc.Allocate(4, 6);

        Color wireColour = Color.white;

        Vector3 bottomLeft = new Vector3(-50, -50, Vertex.nearZ);
        Vector3 topLeft = new Vector3(-50, 50, Vertex.nearZ);
        Vector3 topRight = new Vector3(50, 50, Vertex.nearZ);
        Vector3 bottomRight = new Vector3(50, -50, Vertex.nearZ);

        Vertex[] vertices = new Vertex[]
        {
            new Vertex() { position = bottomLeft, tint = wireColour },
            new Vertex() { position = topLeft, tint = wireColour },
            new Vertex() { position = topRight, tint = wireColour },
            new Vertex() { position = bottomRight, tint = wireColour }
        };

        ushort[] indices = new ushort[]
        {
            0, 1, 2,
            0, 2, 3
        };

        mesh.SetAllVertices(vertices);
        mesh.SetAllIndices(indices);
    }
}

class TexturedElement : VisualElement
{
    static readonly Vertex[] k_Vertices = new Vertex[4];
    static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };

    static TexturedElement()
    {
        k_Vertices[0].tint = Color.white;
        k_Vertices[1].tint = Color.white;
        k_Vertices[2].tint = Color.white;
        k_Vertices[3].tint = Color.white;
    }

    public TexturedElement()
    {
        generateVisualContent += OnGenerateVisualContent;
        m_Texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/tex.png");
    }

    Texture2D m_Texture;

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        Rect r = contentRect;
        if (r.width < 0.01f || r.height < 0.01f)
            return; // Skip rendering when too small.

        float left = 0;
        float right = r.width;
        float top = 0;
        float bottom = r.height;

        k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
        k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
        k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
        k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

        MeshWriteData mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length, m_Texture);

        // Since the texture may be stored in an atlas, the UV coordinates need to be
        // adjusted. Simply rescale them in the provided uvRegion.
        Rect uvRegion = mwd.uvRegion;
        k_Vertices[0].uv = new Vector2(0, 0) * uvRegion.size + uvRegion.min;
        k_Vertices[1].uv = new Vector2(0, 1) * uvRegion.size + uvRegion.min;
        k_Vertices[2].uv = new Vector2(1, 1) * uvRegion.size + uvRegion.min;
        k_Vertices[3].uv = new Vector2(1, 0) * uvRegion.size + uvRegion.min;

        mwd.SetAllVertices(k_Vertices);
        mwd.SetAllIndices(k_Indices);

        
    }
}
