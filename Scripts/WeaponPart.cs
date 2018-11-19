using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XOAProductions.WeaponDesigner
{
    /// <summary>
    /// The types of parts that a weapon part can represent
    /// </summary>
    public enum WeaponPartType
    {
        Trigger,
        LoadingMechanism,
        FiringMechanism,
        EnergyContainer,
        Barrel,
        AmmoClip,
        Stock,
        BarrelAttachment,
        Scope,
        CosmeticAttachment
    }

    /// <summary>
    /// Base class for every weapon part that's connected in a weapon. Basically the node of the WeaponStructure tree
    /// </summary>
    public class WeaponPart : MonoBehaviour
    {
        /// <summary>
        /// the type of part this weapon part represents
        /// </summary>
        public WeaponPartType PartType { get; set; }

        /// <summary>
        /// the parent of this weaponpart
        /// </summary>
        public WeaponPart Parent { get; private set; }

        /// <summary>
        /// the Children of this weaponpart
        /// </summary>
        public List<WeaponPart> Children { get; private set; }

        /// <summary>
        /// the adaptors this part has
        /// </summary>
        public List<Adaptor> Adaptors { get; private set; }

        /// <summary>
        /// a link between the adaptors of this part and the Child that is connected to a specific adaptor.
        /// For example, if this part has three barrel adaptors, you could look up which child-barrel is connected to which of the adaptors.
        /// </summary>
        public Dictionary<WeaponPart, Adaptor> AdaptorConnections { get; private set; }

        /// <summary>
        /// initializes the part with just the Parent
        /// assigns itself to the Parent as a child
        /// </summary>
        /// <param name="_parent">the Parent object of this part</param>
        public WeaponPart(WeaponPart _parent)
        {
            changeParent(_parent);
            Children = new List<WeaponPart>();
        }

        /// <summary>
        /// initializes the part with a Parent and m_Children
        /// assigns itself to the Parent as a child
        /// </summary>
        /// <param name="_parent">the Parent object of this part</param>
        /// <param name="_children">the Children objects of this part</param>
        /// <param name="_adaptorConnections">OPTIONAL: to which adaptors the children are connected, if not specified the children are connected randomly</param>
        public WeaponPart (WeaponPart _parent, List<WeaponPart> _children, Dictionary<WeaponPart, Adaptor> _adaptorConnections = null)
        {
            changeParent(_parent);
            Children = _children;

            if (_adaptorConnections != null)
                AdaptorConnections = _adaptorConnections;
            else
            {
                AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();
                int i = 0;
                foreach (WeaponPart child in Children)
                {
                    child.connectToAdaptor(Adaptors[i]);
                    AdaptorConnections.Add(child, Adaptors[i]);
                    i++;
                }
            }

        }

        /// <summary>
        /// connects this weapon part to an adaptor.
        /// </summary>
        /// <param name="adaptor"></param>
        public void connectToAdaptor(Adaptor adaptor)
        {
            throw new System.NotImplementedException();
            //TODO: here we need to physically move the weapon part to align with the adaptor and do other stuff to make sure everything fits
        }

        /// <summary>
        /// adds a child to this part
        /// </summary>
        /// <param name="child">the child to add</param>
        public void addChild(WeaponPart child)
        {
            if (Children.Contains(child))
                return;

            Children.Add(child);
        }

        /// <summary>
        /// removes a child from this part
        /// </summary>
        /// <param name="child">the child to remove </param>
        public void removeChild(WeaponPart child)
        {
            if (!Children.Contains(child))
                return;

            Children.Remove(child);
        }    

        /// <summary>
        /// changes this part's Parent and automatically assigns itself to the Parent as a child
        /// </summary>
        /// <param name="newParent">the new Parent</param>
        public void changeParent(WeaponPart newParent)
        {
            if (newParent == null)
                return;

            if (Parent != null)
                Parent.removeChild(this);

            Parent = newParent;

            Parent.addChild(this);
        }
    }
}
