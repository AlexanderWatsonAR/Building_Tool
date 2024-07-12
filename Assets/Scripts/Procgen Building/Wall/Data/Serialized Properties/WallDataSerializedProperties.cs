using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    public class WallDataSerializedProperties : SerializedPropertyGroup
    {
        readonly WallSectionDataSerializedProperties[] m_Sections;

        #region Constants
        const string k_Height = "m_Height";
        const string k_Depth = "m_Depth";
        const string k_Columns = "m_Columns";
        const string k_Rows = "m_Rows";
        const string k_Sections = "m_Sections";
        #endregion

        #region Accessors
        public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
        public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
        public SerializedProperty Columns => m_Data.FindPropertyRelative(k_Columns);
        public SerializedProperty Rows => m_Data.FindPropertyRelative(k_Rows);
        //public WallSectionDataSerializedProperties[] Sections => m_Sections;
        #endregion

        public WallDataSerializedProperties(SerializedProperty wallData) : base(wallData)
        {
            //SerializedProperty sections = m_Data.FindPropertyRelative(k_Sections);
            //m_Sections = new WallSectionDataSerializedProperties[sections.arraySize];

            //for (int i = 0; i < m_Sections.Length; i++)
            //{
            //    m_Sections[i] = new WallSectionDataSerializedProperties(sections.GetArrayElementAtIndex(i));
            //}

        }
    }
}
