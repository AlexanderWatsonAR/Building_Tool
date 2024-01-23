using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using UnityEngine.Rendering;
using System.Linq;
using System.Buffers.Text;

[Overlay(typeof(SceneView), "Building", true)]
public class BuildingOverlay : Overlay, ITransientOverlay
{
    [SerializeField] private Building m_Building;

    public override VisualElement CreatePanelContent()
    {
        if (m_Building == null)
            return new VisualElement();

        minSize = Vector2.one * 200f;


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

        //VisualElement textureContainer = new VisualElement()
        //{
        //    style =
        //    {
        //        minHeight = 200,
        //        minWidth = 200,
        //        maxHeight = 400,
        //        maxWidth = 400,
        //        height = 200,
        //        width = 200,
        //        flexGrow = 0,
        //        flexShrink = 0,
        //        flexBasis = 0,
        //    }
        //};

        //textureContainer.style.alignItems = Align.Center;

        //VisualElement styleElement = new() { style = { height = 10, width = 10 } };

        Image image = new Image();
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/tex.png");
        image.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(200, 200)), Vector2.zero);
        root.Add(image);

        Vector3 min, max;
        Vector3[] polygon = m_Building.Data.Path.ControlPoints.GetPositions();
        Extensions.MinMax(polygon, out min, out max);
        Vector3 centre = Vector3.Lerp(min, max, 0.5f);

        polygon = Extensions.RotatePolygon(polygon, Vector3.up, Vector3.forward, centre);

        Extensions.MinMax(polygon, out min, out max);

        Vector3 offset = new Vector3(50, 50);

        for (int i = 0; i < m_Building.Data.Path.ControlPointCount; i++)
        {
            CircleVisualElement controlPoint = new CircleVisualElement()
            {
                style =
                {
                    position = Position.Absolute,
                    height = 10,
                    width = 10
                }
            };

            // Control Points need to be translated so that they are on the XY plane, currently that are on the XZ plane.
            // Control Points also need to be scaled so that they fit within the bound of the panel.

            controlPoint.transform.position = new Vector3(100, 100);
            controlPoint.MarkDirtyRepaint();

            root.Add(controlPoint);
        }

        //CircleVisualElement texturedElement = new CircleVisualElement()
        //{
        //    style =
        //    { 
        //        height = 10,
        //        width = 10
        //    }
        //};
        //texturedElement.transform.position = new Vector3(50, 50, 0);

        //texturedElement.MarkDirtyRepaint();

        //textureContainer.Add(texturedElement);

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

public class CircleVisualElement: VisualElement
{
    private float m_Size;
    private Color m_Colour;

    public CircleVisualElement()
    {
        //m_Size = size;
        m_Colour = Color.black;
        
        generateVisualContent += OnGenerateVisualContent;
        this.AddManipulator(new Clickable(() =>
        {
            m_Colour = Color.yellow;
            Debug.Log("Click");
            MarkDirtyRepaint();
        }));
    }

    // Make a circle
    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        mgc.painter2D.fillColor = m_Colour;

        Rect r = contentRect;

        mgc.painter2D.BeginPath();
        mgc.painter2D.Arc(r.center, r.width, 0, 360, ArcDirection.Clockwise);
        mgc.painter2D.ClosePath();
        mgc.painter2D.Fill();

    }
}

public class SquareVisualElement : VisualElement
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

public class TexturedElement : VisualElement
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

    private Texture2D m_Texture;
    //private Rect rect;

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        Rect r = contentRect;
        if (r.width < 0.01f || r.height < 0.01f)
            return;

        float left = 0;
        float right = r.height;
        float top = 0;
        float bottom = r.width;

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
