using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XOAProductions.WeaponDesigner
{
    public class WeaponPartCreator : MonoBehaviour
    {
        public enum WeaponPartCreationState
        {
            AssignMeshes,
            MoveMeshes,
            AssignAdaptors,
            MoveAdaptors,
            PlaceOwnAdaptorConnection
        }

        public WeaponPartCreationState currentState;

        GameObject partRoot;
         GameObject partMeshesRoot;
         WeaponPart weaponPart;

        public static string partName;
        public static string partID;

        static string prefabPath = "Assets/Weapons/Resources/WeaponParts/";
        
        public static int prefabType;

        [MenuItem("Tools/XOAProductions/CreateWeaponPart")]
        private static void SpawnWeaponPartCreator()
        {
            var creator = GameObject.Find("Weapon Part Creator");
            if (creator != null)
                DestroyImmediate(creator);

            creator = new GameObject();
            creator.transform.position = Vector3.zero;
            creator.transform.rotation = Quaternion.Euler(0, 0, 0);
            creator.name = "Weapon Part Creator";

            

            creator.AddComponent<WeaponPartCreator>().Init();
           

        }

        private void Init()
        {
            partRoot = new GameObject();
            partRoot.name = "WEAPONPART";
            partRoot.transform.position = Vector3.zero;
            partRoot.transform.rotation = Quaternion.Euler(0, 0, 0);
            partRoot.transform.parent = this.transform;

            partMeshesRoot = new GameObject();
            partMeshesRoot.name = "Mesh";
            partMeshesRoot.transform.position = Vector3.zero;
            partMeshesRoot.transform.rotation = Quaternion.Euler(0, 0, 0);
            partMeshesRoot.transform.parent = partRoot.transform;

            weaponPart = partRoot.AddComponent<WeaponPart>();

            Selection.activeGameObject = this.gameObject;
        }

        private void CreatePrefab(bool overwrite)
        {
            CheckAllPrefabFoldersExistAndCreateMissing();

            if (AssetDatabase.LoadAssetAtPath(PathToPartFolder(prefabType) + "/" + partID, typeof(GameObject)) != null)
                if(!overwrite)
                    return;

            AssetDatabase.CreateAsset(partRoot, PathToPartFolder(prefabType) + "/" + partID);

            DestroyImmediate(this);
        }

        /// <summary>
        /// The path to the weaponpart prefab folders
        /// </summary>
        /// <param name="type">the type of the part, see WeaponPartType enum but as int</param>
        /// <returns>returns the path to the prefabs ("Assets/...")</returns>
        private static string PathToPartFolder(int type)
        {
            return prefabPath + WeaponPartTypeStrings.WeaponPartTypes[type] + "s";
        }

        /// <summary>
        /// Checks wether all folders for the prefabs exist, and if they are missing, creates them
        /// </summary>
        private static void CheckAllPrefabFoldersExistAndCreateMissing()
        {
            int i = 0;
            foreach(string WeaponPartType in WeaponPartTypeStrings.WeaponPartTypes)
            {
               if(!AssetDatabase.IsValidFolder(PathToPartFolder(i)))
                {
                    AssetDatabase.CreateFolder(prefabPath, WeaponPartType + "s");
                }

                i ++;
            }
        }

    }
}
