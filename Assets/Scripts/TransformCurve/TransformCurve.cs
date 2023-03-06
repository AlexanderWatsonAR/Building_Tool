using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class TransformCurve : MonoBehaviour
{
    [SerializeField] private List<TransformCurveData> m_TransformCurveData;

    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField, HideInInspector] private Extrudable m_Extrudable;
    [SerializeField, HideInInspector] private bool m_IsInitialized;

    public event EventHandler OnHasReshaped;

    public List<TransformCurveData> TransformCurveData { get { return m_TransformCurveData; } set { m_TransformCurveData = value; } }

    private void Reset()
    {
        m_TransformCurveData ??= new ();

        AnimationCurve defaultCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0, 1));

        TransformCurveData data = new();
        data.SetTransformMultiplier(1);
        data.SetAnimationCurve(defaultCurve);

        Initialize(new TransformCurveData[] { data });
    }

    public TransformCurve Initialize(IEnumerable<TransformCurveData> transformCurveData)
    {
        m_TransformCurveData = transformCurveData.ToList();
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Extrudable = GetComponent<Extrudable>();
        m_IsInitialized = true;
        return this;
    }

    public TransformCurve Reshape(bool extrudeFromDefault = false)
    {
        if(TransformCurveData == null | TransformCurveData.Count == 0)
            throw new Exception("No transform curve data available");

        if (!m_IsInitialized)
            Initialize(TransformCurveData);

        if (extrudeFromDefault)
            m_Extrudable.Extrude();

        int length = m_Extrudable.ExtrusionIndices.Count - 1;
        // Skip the first & last index.
        for (int i = 1; i < length; i++)
        {
            float t = i / (float)m_Extrudable.ExtrusionIndices.Count;
            Transformation(m_Extrudable.ExtrusionIndices[i], t);
        }

        OnHasReshaped?.Invoke(this, EventArgs.Empty);

        return this;
    }

    private void Transformation(IEnumerable<int> indices, float t)
    {
        foreach (TransformCurveData data in m_TransformCurveData)
        {
            data.Transformation(m_ProBuilderMesh, indices, t);
        }
    }
    /// <summary>
    /// Returns false if new size is the same as the old size.
    /// </summary>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public bool ResizeCurveDataList(int newSize)
    {
        if (newSize == m_TransformCurveData.Count)
            return false;

        int oldSize = m_TransformCurveData.Count;
        TransformCurveData[] transformCurves = m_TransformCurveData.ToArray();
        Array.Resize(ref transformCurves, newSize);
        m_TransformCurveData = transformCurves.ToList();

        if (newSize < oldSize)
            return true;

        for(int i = oldSize; i < newSize; i++)
        {
            m_TransformCurveData[i].SetTransformMultiplier(1);
        }

        return true;
    }
}

[System.Serializable]
public struct TransformCurveData
{
    [SerializeField] private AnimationCurve m_AnimationCurve;
    [SerializeField] private float m_TransformMultiplier;
    [SerializeField] private TransformType m_TransformType;
    [SerializeField] private Axis m_RotationAxis;
    [SerializeField] private Axis m_TranslationAxis;
    [SerializeField] private TransformPoint m_TransformPoint;

    public AnimationCurve AnimationCurve => m_AnimationCurve;
    public float TransformMultiplier => m_TransformMultiplier;
    public TransformType TransformType => m_TransformType;
    public Axis RotationAxis => m_RotationAxis;
    public Axis TranslationAxis => m_TranslationAxis; 
    public TransformPoint TransformPoint => m_TransformPoint;

    public string Type
    {
        get
        {
            string type = "";

            switch(m_TransformType)
            {
                case TransformType.Translation:
                    type = "Translation";
                    break;
                case TransformType.Rotation:
                    type = "Rotation";
                    break;
                case TransformType.Scale:
                    type = "Scale";
                    break;
            }
            return type;
        }
    }

    public void SetAnimationCurve(AnimationCurve animationCurve) => m_AnimationCurve = animationCurve;
    public void SetTransformMultiplier(float multi) => m_TransformMultiplier = multi;
    public void SetTransformType(TransformType type) => m_TransformType = type;
    public void SetRotationAxis(Axis axis) => m_RotationAxis = axis;
    public void SetTranslationAxis(Axis axis) => m_TranslationAxis = axis;
    public void SetTranformPoint(TransformPoint point) => m_TransformPoint = point;

    

    //private void InitializeCommonData(AnimationCurve animationCurve, float transformMulti)
    //{
    //    m_TransformCurve = animationCurve;
    //    m_TransformMultiplier = transformMulti;
    //}

    //public TransformCurveData(AnimationCurve transformCurve, float transMulti, TransformType transType, Axis rotAxis, Axis transAxis, TransformPoint transPoint)
    //{
    //    m_TransformCurve = transformCurve;
    //    m_TransformMultiplier = transMulti;
    //    m_TransformType = transType;
    //    m_RotationAxis = rotAxis;
    //    m_TranslationAxis = transAxis;
    //    m_TransformPoint = transPoint;
    //}

    //public TransformCurveData TranslationCurve(AnimationCurve transCurve, float transMulti, Axis transAxis)
    //{
    //    InitializeCommonData(transCurve, transMulti);
    //    m_TranslationAxis = transAxis;
    //    return this;
    //}

    //public TransformCurveData ScalingCurve(AnimationCurve animationCurve, float transformMulti, TransformPoint transformPoint, Axis rotAxis)
    //{
    //    InitializeCommonData(animationCurve, transformMulti);
    //    m_TransformPoint = transformPoint;
    //    m_RotationAxis = rotAxis;
    //    return this;
    //}

    //public TransformCurveData RotationCurve(AnimationCurve animationCurve, float transformMulti, TransformPoint transformPoint, Axis rotAxis)
    //{
    //    InitializeCommonData(animationCurve, transformMulti);
    //    m_TransformPoint = transformPoint;
    //    m_RotationAxis = rotAxis;
    //    return this;
    //}

    public void Transformation(ProBuilderMesh mesh, IEnumerable<int> indices, float t)
    {
        switch (m_TransformType)
        {
            case TransformType.Translation:
                Vector3 offset = m_AnimationCurve.Evaluate(t) * m_TransformMultiplier * m_TranslationAxis.CheckAxis();
                mesh.TranslateVertices(indices, offset);
                break;
            case TransformType.Scale:
                float temp = 1 + (m_AnimationCurve.Evaluate(t) * m_TransformMultiplier);
                mesh.ScaleVertices(indices, m_TransformPoint, temp);
                break;
            case TransformType.Rotation:
                Vector3 eulerAngle = m_AnimationCurve.Evaluate(t) * m_TransformMultiplier * m_RotationAxis.CheckAxis();
                mesh.RotateVertices(indices, m_TransformPoint, eulerAngle);
                break;
        }

        mesh.ToMesh();
        mesh.Refresh();
    }
}
