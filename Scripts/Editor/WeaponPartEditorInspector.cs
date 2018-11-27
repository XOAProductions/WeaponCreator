using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XOAProductions.WeaponDesigner
{
    [CustomEditor(typeof(WeaponPartCreator))]
    public class WeaponPartEditorInspector : Editor
    {
       

        readonly string[] WeaponPartItemListNames =  {"TriggerList", "LoadingMechanismList", "FiringMechanismList", "EnergyContainerList", "BarrelList", "AmmoClipList", "StockList", "BarrelAttachmentList", "ScopeList", "CosmeticAttachmentList"};
        QuickEditorList WeaponPartsInJsonList;
        QuickEditorList AdaptorTypeList;
        List<AdaptorTypes> adaptorTypes = new List<AdaptorTypes>();

        private void OnEnable()
        {
           WeaponPartsInJsonList =  new QuickEditorList(false);
            AdaptorTypeList = new QuickEditorList(false);
        }

        /// <summary>
        /// loads a json file from resources
        /// </summary>
        /// <param name="path">path with name of file except .json ending relative to resources folder</param>
        /// <returns></returns>
        public static string LoadResourceTextfile(string path)
        {

            string filePath = "Assets/Weapons/Resources/" + path.Replace(".json", "");

            TextAsset targetFile = Resources.Load<TextAsset>(path.Replace(".json", ""));

            return targetFile.text;
        }
        /// <summary>
        /// advances the weaponpartcreator to the next state 
        /// </summary>
        /// <param name="c"></param>
        private void AdvanceToNextStep(WeaponPartCreator c)
        {
            c.currentState++; 
        }
        /// <summary>
        /// returns back to the previous state of the weaponpartcreator so the user can make changes
        /// </summary>
        /// <param name="c"></param>
        private void ReturnToPreviousStep(WeaponPartCreator c)
        {
            c.currentState--;
           
        }

        private void InstantiateAdaptors(WeaponPartCreator c)
        {
            c.partRoot.GetComponent<WeaponPart>().Adaptors = new List<Adaptor>();

            foreach (AdaptorTypes type in adaptorTypes)
            {
                GameObject go = Instantiate(c.GetAdaptorPrefab(type), c.partRoot.transform, false) as GameObject;
                go.transform.localPosition = Vector3.zero;
                c.partRoot.GetComponent<WeaponPart>().Adaptors.Add(go.GetComponent<Adaptor>());
            }
        }

        public override void OnInspectorGUI()
        {
            WeaponPartCreator creator = (WeaponPartCreator)target;

            GUILayout.Label("Step " + ((int)creator.currentState + 1) + " of 5");

            JObject jsonItems = JObject.Parse(LoadResourceTextfile("Items"));



            //Let's the user choose which part he's creating from Items.json
            if (creator.currentState == WeaponPartCreator.WeaponPartCreationState.SetupPartInfo)
            {
                EditorGUILayout.HelpBox("Select the part from Items.json that you want to create. Parts that don't already exist will be green, those that exist will be red", MessageType.Info);

                creator.SelectedType = (WeaponPartType)EditorGUILayout.EnumPopup("Part Type", creator.SelectedType);
                int selectedType = (int)creator.SelectedType;
               

                List<string> itemsOfType = new List<string>();
                List<Color> itemsExistColor = new List<Color>();
                
                foreach (JToken value in jsonItems.GetValue(WeaponPartItemListNames[(int)creator.SelectedType]))
                {
                    itemsOfType.Add("Name: " + value.Value<string>("name") + " ID: " + value.Value<string>("id"));
                    selectedType = (int)creator.SelectedType;
                    
                   
                    if (creator.PrefabExists(selectedType, value.Value<string>("id")))
                        itemsExistColor.Add(Color.red);
                    else
                        itemsExistColor.Add(Color.green);

                }

                var selected = WeaponPartsInJsonList.DisplayListStringSelectButtonColorMarkings(itemsOfType, itemsExistColor, "Part", WeaponPartItemListNames[selectedType]);

                if (selected.Contains("ID:"))
                {
                    var splitID = selected.Split(new string[] { "ID: " }, System.StringSplitOptions.None);
                    var splitName = splitID[0].Split(new string[] { "Name: " }, System.StringSplitOptions.None);

                    if (creator.PrefabExists(selectedType, splitID[1])) //selected part exists
                    {
                        //ask user if he really wants to contine
                        if (EditorUtility.DisplayDialog("Part already exists", "A Part with ID " + splitID[1] + " already exists. Do you want to continue or select another one?", "Continue"))
                        {
                            creator.SetupWeaponPartInformation(splitName[1], splitID[1], creator.SelectedType);
                            AdvanceToNextStep(creator);
                        }
                    }
                    else //part doesn't yet exist
                    {
                        creator.SetupWeaponPartInformation(splitName[1], splitID[1], creator.SelectedType);
                        AdvanceToNextStep(creator);
                    }
                }


            }

            //Let's the user choose which meshes will visualy represent the weaponpart
            if (creator.currentState == WeaponPartCreator.WeaponPartCreationState.AssignMeshes)
            {
                EditorGUILayout.HelpBox("Drop all the mesh-parts under the 'Mesh' gameobject. /n Align them with the adaptor that is displayed so it looks like the part is connected to the adaptor.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Step"))
                {
                    ReturnToPreviousStep(creator);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("NEXT STEP"))
                {
                    AdvanceToNextStep(creator);
                }

                EditorGUILayout.EndHorizontal();
            }

            //Let's the user choose how many adaptors he wants and of which type the should be
            //Instantiates the adaptors when user clicks next step
            if (creator.currentState == WeaponPartCreator.WeaponPartCreationState.AssignAdaptors)
            {
                EditorGUILayout.HelpBox("Add All the types of adaptors to the list that this weapon part should offer, /n and they'll be instantiated after clicking on 'NEXT STEP'", MessageType.Info);

                AdaptorTypeList.DisplayListEnumDropdown(ref adaptorTypes, "Adaptor", "AdaptorTypes");

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Step"))
                {
                    ReturnToPreviousStep(creator);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("NEXT STEP"))
                {
                    InstantiateAdaptors(creator);

                    AdvanceToNextStep(creator);
                }

                EditorGUILayout.EndHorizontal();
            }

            //Let's the user move around the instantiated subadaptors
            if(creator.currentState == WeaponPartCreator.WeaponPartCreationState.MoveAdaptors)
            {
                EditorGUILayout.HelpBox("Move all instantiated Adaptors around and scale them as you /n like. Click on 'NEXT STEP' once you're finished.", MessageType.Info);


                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Step"))
                {
                    ReturnToPreviousStep(creator);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("NEXT STEP"))
                {
                    

                    AdvanceToNextStep(creator);
                }

                EditorGUILayout.EndHorizontal();
            }

            //Tells the user to check all the settings and gives him the final button to save the part as a prefab
            if(creator.currentState == WeaponPartCreator.WeaponPartCreationState.ConfirmAndSave)
            {
                EditorGUILayout.HelpBox("Check if all Scripts, GameObjects and Meshes are looking correct and press 'SAVE PART' if you're happy.", MessageType.Info);


                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Step"))
                {
                    ReturnToPreviousStep(creator);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("SAVE PART"))
                {


                    if (!creator.CreatePrefab(false))
                    {
                       if( EditorUtility.DisplayDialog("Part already exists", "A Part with ID " + creator.partID + " already exists. Do you want to overwrite the existing part?", "Overwrite"))
                        {
                            creator.CreatePrefab(true);
                          
                        }
                    }
                    
                }

                EditorGUILayout.EndHorizontal();
            }
        }


       




    }
}
