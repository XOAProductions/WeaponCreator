using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XOAProductions.WeaponDesigner
{
    [CustomEditor(typeof(WeaponPartCreator))]
    public class WeaponPartEditorInspector : Editor
    {
       

        

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            if (GUILayout.Button("TEST"))
            {

            }
        }







    }
}
