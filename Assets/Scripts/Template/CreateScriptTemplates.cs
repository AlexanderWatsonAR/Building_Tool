using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System;
using UnityEngine.Rendering.LookDev;

public static class CreateScriptTemplates
{

    [MenuItem("Assets/Create/Code/Buildable Set", priority = 40)]
    public static void CreateMonoBehaviourMenuItem()
    {
        CreateEditorScriptEndNameEditAction create = ScriptableObject.CreateInstance<CreateEditorScriptEndNameEditAction>();

        

        Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, "Assets/Scripts/Template/NewBuildable.cs", icon, null);

        //ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "NewScript.cs");
    }

    internal class CreateEditorScriptEndNameEditAction : EndNameEditAction
    {
        const string k_WordToReplace = "#SCRIPTNAME#";
        const string k_AnotherWordToReplace = "#SCRIPTLOWERCASENAME#";

        #region Templates
        const string k_DataTemplatePath = "Assets/Scripts/Template/Templates/DataScriptTemplate.cs.txt";
        const string k_BuildableTemplatePath = "Assets/Scripts/Template/Templates/BuildableScriptTemplate.cs.txt";
        const string k_DataScriptablePropertiesPath = "Assets/Scripts/Template/Templates/DataSerializedPropertiesScriptTemplate.cs.txt";

        const string k_DataPropDrawerTemplatePath = "Assets/Scripts/Template/Templates/DataPropertyDrawerScriptTemplate.cs.txt";
        const string k_OverlayTemplatePath = "Assets/Scripts/Template/Templates/OverlayScriptTemplate.cs.txt";
        const string k_EditorPropertiesPath = "Assets/Scripts/Template/Templates/BuildableEditorScriptTemplate.cs.txt";

        #endregion

        #region Destinations
        const string k_BuildablesPath = "Assets/Scripts/TownGenerator/Buildables";
        const string k_DataPath = "Assets/Scripts/TownGenerator/Data";
        const string k_SerializedPropertiesPath = "Assets/Scripts/TownGenerator/Data/Serialized Properties";

        const string k_OverlayPath = "Assets/Scripts/TownGenerator/Editor/Overlays";
        const string k_EditorPath = "Assets/Scripts/TownGenerator/Editor/Inspectors";
        const string k_DrawerPath = "Assets/Scripts/TownGenerator/Editor/Property Drawer";
        #endregion

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string name = Path.GetFileNameWithoutExtension(pathName);

            WriteScriptFromTemplate(k_DataTemplatePath, k_DataPath, name, "Data");
            WriteScriptFromTemplate(k_BuildableTemplatePath, k_BuildablesPath, name, "");
            WriteScriptFromTemplate(k_DataScriptablePropertiesPath, k_SerializedPropertiesPath, name, "DataSerializedProperties");

            WriteScriptFromTemplate(k_DataPropDrawerTemplatePath, k_DrawerPath, name, "DataDrawer");
            WriteScriptFromTemplate(k_OverlayTemplatePath, k_OverlayPath, name, "Overlay");
            WriteScriptFromTemplate(k_EditorPropertiesPath, k_EditorPath, name, "Editor");

            AssetDatabase.Refresh();
        }

        private void WriteScriptFromTemplate(string templatePath, string destinationPath, string scriptName, string type)
        {
            string fileContent = File.ReadAllText(templatePath);

            fileContent = fileContent.Replace(k_WordToReplace, scriptName);
            fileContent = fileContent.Replace(k_AnotherWordToReplace, scriptName.ToLower());

            File.WriteAllText(destinationPath + "/" + scriptName + type + ".cs", fileContent);
        }
    }
}
