using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace XOAProductions.WeaponDesigner
{
    /// <summary>
    /// Editor for weapon parts
    /// </summary>
    [CustomEditor(typeof(WeaponPart))]
    public class WeaponPartEditor : Editor
    {
        QuickEditorList AdaptorList;
        
        private void OnEnable()
        {
            
            AdaptorList = new QuickEditorList(false);   
        }

        public override void OnInspectorGUI()
        {
           
            WeaponPart part = (WeaponPart)target;
            if (part.Children == null)
                part.Children = new List<WeaponPart>();
            if (part.Adaptors == null)
                part.Adaptors = new List<Adaptor>();


            part.PartType = (WeaponPartType) EditorGUILayout.EnumPopup("Part Type:", part.PartType);
          
            AdaptorList.DisplayList<Adaptor>(ref part.Adaptors, "Adaptor", "Adaptors");
          
            GUI.enabled = false;
            EditorGUILayout.LabelField("Statistics:");
          
            EditorGUI.indentLevel++;
          
                 EditorGUILayout.TextField("Part Name:", part.PartName);
                 EditorGUILayout.TextField("Part ID:", part.PartID);
                 EditorGUILayout.ObjectField("Parent Part:", part.Parent, typeof(WeaponPart), true);
          
                 EditorGUILayout.Space();
                 EditorGUILayout.LabelField("Connected Children:");
                 EditorGUI.indentLevel++;
                    
                    foreach(WeaponPart child in part.Children)
                     {
                         EditorGUILayout.BeginHorizontal();
          
                         EditorGUILayout.LabelField("Name: " + child.PartName + "ID: " + child.PartID + "Connecting Adaptor: ");
                         Adaptor adaptor;
                         part.AdaptorConnections.TryGetValue(child, out adaptor);
                         EditorGUILayout.ObjectField(adaptor, typeof(Adaptor), true);
          
                         EditorGUILayout.EndHorizontal();
                     }
          
            GUI.enabled = true;
        }
    }
}