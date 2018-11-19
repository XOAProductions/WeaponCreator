using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XOAProductions.WeaponDesigner
{
    [CustomEditor(typeof(WeaponPart))]
    public class WeaponPartEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WeaponPart part = (WeaponPart)target;

            part.PartType = (WeaponPartType) EditorGUILayout.EnumPopup("Part Type:", part.PartType);

            EditorGUILayout.LabelField("Statistics:");
            EditorGUILayout.ObjectField("Parent Part:", part.Parent, typeof(WeaponPart), true);
        }
    }
}