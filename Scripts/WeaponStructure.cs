using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOAProductions.WeaponDesigner {

    /// <summary>
    /// Contains the weapon-structure tree.
    /// </summary>
    public class WeaponStructure : MonoBehaviour
    {

        /// <summary>
        /// the trigger of the weapon, which is also the first part in the 
        /// weapon-tree
        /// </summary>
        public WeaponPart trigger;

        /// <summary>
        /// constructs this structure
        /// </summary>
        /// <param name="_trigger">the trigger, aka the first part of the weaponstructure tree</param>
        public WeaponStructure(WeaponPart _trigger)
        {
            trigger = _trigger;
        }

        /// <summary>
        /// recursively searches the part-tree and returns the first match, or null if no match is found
        /// </summary>
        /// <param name="match">the predicate that describes the searched part</param>
        /// <param name="startingPart">where to start. leave at null to start with the first part in the tree, otherwise specifiy a part in the tree to check
        /// it and its children for a match</param>
        /// <returns>the weapon part that has been found or null if none has been found</returns>
        public WeaponPart FindWeaponPartRecursive(System.Predicate<WeaponPart> match, WeaponPart startingPart = null)
        {
            if (startingPart == null)
                startingPart = trigger;

            if (match(startingPart))
                return startingPart;
            else
            {
                foreach (WeaponPart child in startingPart.Children)
                {
                    var result = FindWeaponPartRecursive(match, child);
                    if (result != null)
                        return result;
                }

            }

            return null;
        }

        /// <summary>
        /// finds all parts of the specified type
        /// </summary>
        /// <param name="type">the type of parts to find</param>
        /// <param name="startingPart">what part to start searching at, leave empty for starting at the first part</param>
        /// <returns>list of weapon parts of type or empy list if none are found</returns>
        public List<WeaponPart> FindAllWeaponPartsOfTypeRecursively(WeaponPartType type, WeaponPart startingPart = null)
        {
            List<WeaponPart> result = new List<WeaponPart>();

            if (trigger == null)
                return result;

            if (startingPart == null)
                startingPart = trigger;


            if (startingPart.PartType == type)
            {
                result.Add(startingPart);
            }

            foreach (WeaponPart child in startingPart.Children)
            {
                result.AddRange(FindAllWeaponPartsOfTypeRecursively(type, child));
            }

            return result;
        }

        /// <summary>
        /// Replaces a weapon part by another of same type, trying to keep the sub-hierachy intact
        /// </summary>
        /// <param name="partToReplace">the part that will be replaced</param>
        /// <param name="replacementPart">the part that it will be replaced by</param>
        public void ReplaceWeaponPart(WeaponPart partToReplace, WeaponPart replacementPart)
        {
            if (trigger == null)
                return;

            //save children
            var children = new List<WeaponPart>(partToReplace.Children);

            if (partToReplace != trigger)
            {
                //save adaptor that part is connected to
                Adaptor connectingAdaptor = partToReplace.Parent.AdaptorConnections[partToReplace];
                //save parent part
                WeaponPart parent = partToReplace.Parent;


                //all children transform parent to null
                partToReplace.DetachAllChildren();

                //remove from parent
                if (partToReplace.Parent != null)
                    parent.RemoveChild(partToReplace);



                //add new part into hierachy

                parent.AddChild(replacementPart);
                parent.ConnectChildToAdaptor(connectingAdaptor, replacementPart);
            }
            else //is trigger aka toplevel weaponpart
            {
                replacementPart.transform.parent = partToReplace.transform.parent;
                //all children transform parent to null
                partToReplace.DetachAllChildren();
                partToReplace.transform.parent = null;
            }

            List<WeaponPart> excessParts = new List<WeaponPart>();
            List<WeaponPart> childrenCopy = new List<WeaponPart>(children);
            int i = 0;
            //reconnect children
            foreach(Adaptor adaptor in replacementPart.Adaptors)
            {
                var child = childrenCopy.Find(x => x.PartType == adaptor.WeaponTypeOfAdaptor);
                if (child == null)
                    continue;
                    
                replacementPart.AddChild(child);
                replacementPart.ConnectChildToAdaptor(adaptor, child);

                childrenCopy.Remove(child);

                i++;
            }

            excessParts = childrenCopy;

            //destroy all parts that could not be reconnected
            foreach (WeaponPart excess in excessParts)
                Destroy(excess.gameObject);


            //destroy the actual part
            Destroy(partToReplace.gameObject);

        }

        /// <summary>
        /// Removes a weapon part completely, including the sub-hierachy
        /// </summary>
        /// <param name="partToRemove">the part to Remove</param>
        public void RemoveWeaponPart(WeaponPart partToRemove)
        {
            if(partToRemove.Parent != null)
            {
                partToRemove.Parent.RemoveChild(partToRemove);
            }

            Destroy(partToRemove.gameObject);

           

        }

        /// <summary>
        /// Adds a weapon part to the specified adaptor
        /// </summary>
        /// <param name="adaptor">the adaptor to which to add the part</param>
        /// <param name="partToAdd">the part to add</param>
        public void AddWeaponPart(Adaptor adaptor, WeaponPart partToAdd)
        {
            WeaponPart adaptorPart = FindWeaponPartRecursive(x => x.Adaptors.Contains(adaptor));
            if (adaptor == null)
                return;

            
            adaptorPart.AddChild(partToAdd);
            adaptorPart.ConnectChildToAdaptor(adaptor, partToAdd);
        }

        /// <summary>
        /// finds all adaptors that are not yet connected to a child weaponpart
        /// </summary>
        /// <param name="startingPart">the part to start from, leave empty to start from the beginning</param>
        /// <returns>List of all unconnected adaptors</returns>
        public List<Adaptor> FindUnconnectedAdaptorsRecursively(WeaponPart startingPart = null)
        {
            List<Adaptor> result = new List<Adaptor>();


            if (trigger == null)
                return result;

            if (startingPart == null)
                startingPart = trigger;

            if (startingPart.AdaptorConnections == null)
                startingPart.AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();

            if (startingPart.AdaptorConnections.Count < startingPart.Adaptors.Count)
            {
                foreach (Adaptor adaptor in startingPart.Adaptors)
                    if (!startingPart.AdaptorConnections.ContainsValue(adaptor))
                        result.Add(adaptor);
            }

            foreach (WeaponPart child in startingPart.Children)
            {
                result.AddRange(FindUnconnectedAdaptorsRecursively(child));
            }

            return result;
        }
    }
}
