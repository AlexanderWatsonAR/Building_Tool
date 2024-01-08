using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(TransformCurve))/*, CanEditMultipleObjects*/]
public class TransformCurveEditor : Editor
{
    //private SerializedProperty m_TransformCurveDataArray;
    //private bool showCurveList;
    //private bool[] foldoutArray;

    TransformCurve transformCurve;
    Extrudable extruder;

    //private void OnEnable()
    //{
        

    //    //transformCurve ??= new TransformCurve();

    //    //foldoutArray = new bool[transformCurve.TransformCurveData.Count];
    //    //m_TransformCurveDataArray = serializedObject.FindProperty("m_TransformCurveData");
        
    //}

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        transformCurve = (TransformCurve)target;

        //transformCurve = serializedObject.targetObject as TransformCurve;
        extruder = transformCurve.GetComponent<Extrudable>();
        ReshapeButton();

        ////if (serializedObject.targetObject.GetComponent<Transform>().position !=
        ////    transformCurve.transform.position)
        ////{
        ////    OnEnable();
        ////}
        {
            //    serializedObject.Update();

            //    EditorGUILayout.BeginHorizontal();
            //    showCurveList = EditorGUILayout.Foldout(showCurveList, "Transform Curve List");
            //    var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
            //    EditorGUILayout.LabelField("Size", style);
            //    int size = int.Parse(EditorGUILayout.TextArea(transformCurve.TransformCurveData.Count.ToString()));

            //    if(transformCurve.ResizeCurveDataList(size))
            //    {
            //        OnEnable();
            //    }

            //    EditorGUILayout.EndHorizontal();

            //    if (!showCurveList)
            //    {
            //        ReshapeButton();
            //        return;
            //    }

            //    serializedObject.Update();

            //    for (int i = 0; i < m_TransformCurveDataArray.arraySize; i++)
            //    {
            //        foldoutArray[i] = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutArray[i], transformCurve.TransformCurveData[i].Type + " Curve " + i.ToString());

            //        if (!foldoutArray[i])
            //        {
            //            EditorGUILayout.EndFoldoutHeaderGroup();
            //            continue;
            //        }

            //        SerializedProperty dataElement = m_TransformCurveDataArray.GetArrayElementAtIndex(i);

            //        SerializedProperty iterator = dataElement.Copy();
            //        iterator.NextVisible(true);
            //        SerializedProperty animationCurve = iterator.Copy();
            //        EditorGUILayout.PropertyField(animationCurve, false);
            //        //EditorGUILayout.CurveField(transformCurve.TransformCurveData[i].TransformCurve);

            //        iterator.NextVisible(true);
            //        SerializedProperty multiplier = iterator.Copy();
            //        EditorGUILayout.PropertyField(multiplier, false);

            //        iterator.NextVisible(true);
            //        SerializedProperty transformType = iterator.Copy();
            //        EditorGUILayout.PropertyField(transformType, false);
            //        TransformType transformTypeEnum = transformType.GetEnumValue<TransformType>();

            //        switch(transformTypeEnum)
            //        {
            //            case TransformType.Translation:
            //                iterator.NextVisible(true);
            //                iterator.NextVisible(true);
            //                SerializedProperty translationAxis = iterator.Copy();
            //                EditorGUILayout.PropertyField(translationAxis, false);
            //                break;
            //            case TransformType.Rotation:
            //                iterator.NextVisible(true);
            //                SerializedProperty rotationAxis = iterator.Copy();
            //                EditorGUILayout.PropertyField(rotationAxis, false);
            //                iterator.NextVisible(true);
            //                iterator.NextVisible(true);
            //                SerializedProperty transformPoint = iterator.Copy();
            //                EditorGUILayout.PropertyField(transformPoint, false);
            //                break;
            //            case TransformType.Scale:
            //                iterator.NextVisible(true);
            //                iterator.NextVisible(true);
            //                iterator.NextVisible(true);
            //                SerializedProperty transPoint = iterator.Copy();
            //                EditorGUILayout.PropertyField(transPoint, false);
            //                break;
            //        }

            //        EditorGUILayout.EndFoldoutHeaderGroup();
            //    }
            //    ReshapeButton();

            //    serializedObject.ApplyModifiedProperties();
        }
    }

    private void ReshapeButton()
    {
        if (extruder != null)
        {
            if (GUILayout.Button("Reshape Mesh"))
            {
                transformCurve.Reshape(true);
            }
        }

    }

    //while (iterator.NextVisible(true))
    //{
    //    EditorGUILayout.PropertyField(iterator, false);
    //}
}
