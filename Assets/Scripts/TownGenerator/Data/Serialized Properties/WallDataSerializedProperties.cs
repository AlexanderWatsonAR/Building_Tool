using UnityEditor;

public class WallDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Height = "m_Height";
    const string k_Depth = "m_Depth";
    const string k_Columns = "m_Columns";
    const string k_Rows = "m_Rows";
    #endregion

    #region Accessors
    public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
    public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
    public SerializedProperty Columns => m_Data.FindPropertyRelative(k_Columns);
    public SerializedProperty Rows => m_Data.FindPropertyRelative(k_Rows);
    #endregion

    public WallDataSerializedProperties(SerializedProperty wallData) : base(wallData)
    {
    }

}
