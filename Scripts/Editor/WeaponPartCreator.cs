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
            SetupPartInfo,
            AssignMeshes,
            AssignAdaptors,
            MoveAdaptors,
            ConfirmAndSave
        }

        public WeaponPartCreationState currentState = WeaponPartCreationState.SetupPartInfo;

        public GameObject partRoot;
         GameObject partMeshesRoot;
         WeaponPart weaponPart;

        public List<Mesh> AdaptorMeshes = new List<Mesh>();
        public List<GameObject> AdaptorPrefabs = new List<GameObject>();

        public string partName;
        public string partID;

        static string prefabPath = "Assets/Weapons/Resources/WeaponParts/";

        public WeaponPartType SelectedType = WeaponPartType.CosmeticAttachment;
        public  int prefabType;

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

        /// <summary>
        /// Inits this creator instance, always called after user clicks on menuitem
        /// </summary>
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
            currentState = WeaponPartCreationState.SetupPartInfo;
            Selection.activeGameObject = this.gameObject;

            AdaptorMeshes.Add(Resources.Load<Mesh>("AdaptorPreviewMeshes/BarrelAdaptorPreviewMesh"));
            AdaptorPrefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Weapons/BarrelAdaptor/BarrelAdaptor.prefab", typeof(GameObject)));
        }

        /// <summary>
        /// returns the correct prefab for the Adaptortype
        /// </summary>
        /// <param name="type">the type of adaptor</param>
        /// <returns></returns>
        public GameObject GetAdaptorPrefab(AdaptorTypes type)
        {
            
            return AdaptorPrefabs[(int)type];
        }

        /// <summary>
        /// checks if a prefab exists
        /// </summary>
        /// <param name="_partType">the type of the part</param>
        /// <param name="_partID">the id of the part</param>
        /// <returns></returns>
        public bool PrefabExists(int _partType, string _partID)
        {
           
           

            var go = Resources.Load( "WeaponParts/" + WeaponPartTypeStrings.WeaponPartTypes[_partType] + "s/" + _partID) as GameObject;
           
            if (go != null)
                return true;
            return false;
        }

        /// <summary>
        /// creates the prefab, deletes this gameobject
        /// </summary>
        /// <param name="overwrite">true if you want to overrite existing prefabs</param>
        /// <returns></returns>
        public bool CreatePrefab(bool overwrite)
        {
            CheckAllPrefabFoldersExistAndCreateMissing();

            if (PrefabExists(prefabType, partID))
                if(!overwrite)
                    return false;

            partRoot.name = partID;
            PrefabUtility.CreatePrefab(PathToPartFolder(prefabType) + "/" + partID + ".prefab", partRoot);

            DestroyImmediate(this.gameObject);
            return true;
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

        /// <summary>
        /// sets up the weapon part with name and id
        /// </summary>
        /// <param name="name">the name of the part, from items.json</param>
        /// <param name="id">the id of the part, from items.json</param>
        public void SetupWeaponPartInformation(string name, string id, WeaponPartType type)
        {
            partName = name;
            partID = id;
            prefabType = (int)type;

            weaponPart.PartName = name;
            weaponPart.PartID = id;
            weaponPart.PartType = type;
        }

        /// <summary>
        /// Draws the adaptor when in the AssignMeshes state
        /// </summary>
        public void OnDrawGizmos()
        {
            if(currentState == WeaponPartCreationState.AssignMeshes)
            {
                
                Gizmos.DrawMesh(AdaptorMeshes[(int)SelectedType]);
            }
        }

    }
}
