using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XOAProductions.Utility;


namespace XOAProductions.WeaponDesigner
{
    /// <summary>
    /// The types of parts that a weapon part can represent
    /// </summary>
    public enum WeaponPartType
    {
        Trigger = 0,
        LoadingMechanism = 1,
        FiringMechanism = 2,
        EnergyContainer = 3,
        Barrel = 4,
        AmmoClip = 5,
        Stock = 6,
        BarrelAttachment = 7,
        Scope = 8,
        CosmeticAttachment = 9
    }

    public static class WeaponPartTypeStrings{

        public static string[] WeaponPartTypes = new string[] { "Trigger",
                                                         "LoadingMechanism",
                                                         "FiringMechanism",
                                                         "EnergyContainer",
                                                         "Barrel",
                                                         "AmmoClip",
                                                         "Stock",
                                                         "BarrelAttachment",
                                                         "Scope",
                                                         "CosmeticAttachment" };

    }
    /// <summary>
    /// Base class for every weapon part that's connected in a weapon. Basically the node of the WeaponStructure tree
    /// </summary>
    public class WeaponPart : MonoBehaviour
    {
        /// <summary>
        /// the type of part this weapon part represents
        /// </summary>
        [SerializeField]
        public WeaponPartType PartType;

        private void Start()
        {
            ToggleAdaptorVisibility();
        }

        public void ToggleAdaptorVisibility()
        {
            var Hider = new MaterialHider();

            if (AdaptorConnections == null)
                AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();

            if (Adaptors == null)
                return;

            foreach (Adaptor a in Adaptors)
            {
                if (!AdaptorConnections.ContainsValue(a)) //adaptor isn't connected
                {
                    a.isUnconnected = true;

                    if (a.ChildPartTransform == null)
                        continue;

                    Hider.HideHierarchy(a.ChildPartTransform.gameObject, this.PartID); //hide the unconnected side
                   
                }
                else
                {
                    a.isUnconnected = false;
                }
            }
        }

        /// <summary>
        /// the name of this weapon part
        /// </summary>
        public string PartName;

        /// <summary>
        /// the id that this part has in Items.json
        /// </summary>
        public string PartID;

        /// <summary>
        /// the parent of this weaponpart
        /// </summary>
        public WeaponPart Parent { get; private set; }

        /// <summary>
        /// the Children of this weaponpart
        /// </summary>
        public List<WeaponPart> Children = new List<WeaponPart>();

        /// <summary>
        /// the adaptors this part has
        /// </summary>
        public List<Adaptor> Adaptors = new List<Adaptor>();

        /// <summary>
        /// a link between the adaptors of this part and the Child that is connected to a specific adaptor.
        /// For example, if this part has three barrel adaptors, you could look up which child-barrel is connected to which of the adaptors.
        /// </summary>
        public Dictionary<WeaponPart, Adaptor> AdaptorConnections { get;  set; }

        /// <summary>
        /// initializes the part with just the Parent
        /// assigns itself to the Parent as a child
        /// </summary>
        /// <param name="_parent">the Parent object of this part</param>
        public WeaponPart(WeaponPart _parent, string _partID)
        {
            ChangeParent(_parent);
            Children = new List<WeaponPart>();
            PartID = _partID;
        }

        /// <summary>
        /// initializes the part with a Parent and m_Children
        /// assigns itself to the Parent as a child
        /// </summary>
        /// <param name="_parent">the Parent object of this part</param>
        /// <param name="_children">the Children objects of this part</param>
        /// <param name="_adaptorConnections">OPTIONAL: to which adaptors the children are connected, if not specified the children are connected randomly</param>
        public WeaponPart (WeaponPart _parent, List<WeaponPart> _children, string _partID, Dictionary<WeaponPart, Adaptor> _adaptorConnections = null)
        {
            ChangeParent(_parent);
            Children = _children;
            PartID = _partID;

            if (_adaptorConnections != null)
                AdaptorConnections = _adaptorConnections;
            else
            {
                AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();
                int i = 0;
                foreach (WeaponPart child in Children)
                {
                    ConnectChildToAdaptor(Adaptors[i], child);
                    AdaptorConnections.Add(child, Adaptors[i]);
                    i++;
                }
            }

        }

        /// <summary>
        /// connects a child to an adaptor.
        /// </summary>
        /// <param name="adaptor"></param>
        public void ConnectChildToAdaptor(Adaptor adaptor, WeaponPart child)
        {
           
            if (AdaptorConnections == null)
                AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();

           // if (AdaptorConnections.ContainsValue(adaptor))//adaptor already connected
           //     return;

            if (adaptor.WeaponTypeOfAdaptor != child.PartType)
                return;

            child.transform.parent = adaptor.ChildPartTransform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.Euler(0, 0, 0);

            AdaptorConnections.Add(child, adaptor);

            MaterialHider hider = new MaterialHider();
            hider.DisplayHierarchy(adaptor.ChildPartTransform.gameObject, this.PartID); //make connected side visible again
            adaptor.isUnconnected = false;
            //TODO: here we need to physically move the weapon part to align with the adaptor and do other stuff to make sure everything fits
        }

        /// <summary>
        /// adds a child to this part
        /// </summary>
        /// <param name="child">the child to add</param>
        public void AddChild(WeaponPart child)
        {
            if (Children.Contains(child))
                return;
            
            if (Children.Count == Adaptors.Count) //we can't have more children than adaptors
                return;

            Children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// removes a child from this part
        /// </summary>
        /// <param name="child">the child to remove </param>
        public void RemoveChild(WeaponPart child)
        {
            if (!Children.Contains(child))
                return;

            Children.Remove(child);
            Adaptor a = AdaptorConnections[child];
            AdaptorConnections.Remove(child);

            if (a != null)
            {
                MaterialHider hider = new MaterialHider();
                hider.HideHierarchy(a.ChildPartTransform.gameObject, this.PartID);
                a.isUnconnected = true;
            }
        }    

        /// <summary>
        /// changes this part's Parent and automatically assigns itself to the Parent as a child
        /// </summary>
        /// <param name="newParent">the new Parent</param>
        public void ChangeParent(WeaponPart newParent)
        {
            if (newParent == null)
                return;

            if (Parent != null)
                Parent.RemoveChild(this);

            Parent = newParent;

            Parent.AddChild(this);
        }

        /// <summary>
        /// quick setup for tests
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void TestSetup(string name, WeaponPartType type, WeaponPart _parent)
        {
            this.PartName = name;
            this.PartType = type;

            ChangeParent(_parent);
            Children = new List<WeaponPart>();
        }

        /// <summary>
        /// Detaches all child parts from this part
        /// </summary>
        public void DetachAllChildren()
        {
            foreach(WeaponPart child in Children)
            {
                child.transform.parent = null;
                
            }

            Children.Clear();
            if(AdaptorConnections != null)
                AdaptorConnections.Clear();

            
        }

        /// <summary>
        /// finds the adaptor by which this part is connected to it's parent
        /// </summary>
        /// <returns>the adaptor by which this part is connected to it's parent, or null if not connected to an adaptor</returns>
        public Adaptor getConnectingAdaptor()
        {
            Adaptor connectingAdaptor = null;
            this.Parent.AdaptorConnections.TryGetValue(this, out connectingAdaptor);

            return connectingAdaptor;
        }
    }
}
