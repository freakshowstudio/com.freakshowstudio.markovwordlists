
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

using UnityEngine;


namespace FreakshowStudio.MarkovWordLists.Editor
{
    [CustomEditor(typeof(MarkovWordlistImporter))]
    public class MarkovWordlistImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var orderProperty =
                serializedObject.FindProperty("_order");

            var startCharacterProperty =
                serializedObject.FindProperty("_startCharacter");

            var endCharacterProperty =
                serializedObject.FindProperty("_endCharacter");


            var orderContent = new GUIContent("Order");

            var startCharacterContent = new GUIContent("Start Character");

            var endCharacterContent = new GUIContent("End Character");


            EditorGUILayout.PropertyField(
                orderProperty, orderContent);

            EditorGUILayout.PropertyField(
                startCharacterProperty, startCharacterContent);

            EditorGUILayout.PropertyField(
                endCharacterProperty, endCharacterContent);


            base.OnApplyRevertGUI();
        }
    }
}
