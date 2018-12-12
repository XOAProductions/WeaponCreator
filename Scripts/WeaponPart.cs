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
    /// <summary>
    /// contains string names of possible weaponpart types, mainly used to filter items.json
    /// </summary>
    public static class WeaponPartTypeStrings{


        /// <summary>
        /// the types a weapon part can be, useful to filter through items.json with these strings
        /// </summary>
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

        /// <summary>
        /// sets all the adaptors that are not connected to be invisble (only the side that's not connected to this part)
        /// also sets adaptor.isUnconnected to the respective value
        /// 
        /// TODO: this is a bit of a hack, should come back to this and clean it up
        /// </summary>
        public void ToggleAdaptorVisibility()
        {
            var Hider = new MaterialHider(); //instantiate new hider

            if (AdaptorConnections == null) //sometimes this isn't initialized for some reason...
                AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();

            if (Adaptors == null) //there are parts that don't have any adaptors (for example a silencer or something like that)
                return;

            foreach (Adaptor a in Adaptors) 
            {
                if (!AdaptorConnections.ContainsValue(a)) //adaptor isn't connected
                {
                    a.isUnconnected = true;

                    if (a.ChildPartTransform == null)
                        continue;

                    Hider.HideHierarchy(a.ChildPartTransform.gameObject, this.PartID); //hide the unconnected side and set id as this.partid, so we can selectively turn back on
                                                                                       //only the adaptor that belongs to this part instead of the whole hierarchy below our adaptor
                   
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
        [System.Obsolete("Constructing with automated connection is deprecated, please use WeaponStructureAction to perfom this task with added control of animations instead.", false)]
        public WeaponPart (WeaponPart _parent, List<WeaponPart> _children, string _partID, Dictionary<WeaponPart, Adaptor> _adaptorConnections = null)
        {
            //attaching to other parts has become quite a bit more challenging than this implementation, so WeaponStructureAction should be used instead, 
            //which has the added benefit of controlling animations, too. I'll leave the code commented out if it's needed again - or should be modified
            throw new System.NotImplementedException("Please use the WeaponStructureAction class to perform automated connections to other parts");
           
           // ChangeParent(_parent);
           // Children = _children;
           // PartID = _partID;
           //
           // if (_adaptorConnections != null)
           //     AdaptorConnections = _adaptorConnections;
           // else
           // {
           //     AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();
           //     int i = 0;
           //     foreach (WeaponPart child in Children)
           //     {
           //         ConnectChildToAdaptor(Adaptors[i], child);
           //         AdaptorConnections.Add(child, Adaptors[i]);
           //         i++;
           //     }
           // }

        }

        /// <summary>
        /// connects a child to an adaptor.
        /// </summary>
        /// <param name="adaptor"></param>
        public void ConnectChildToAdaptor(Adaptor adaptor, WeaponPart child)
        {
            //checks
            if (AdaptorConnections == null) //sometimes this thing isn't initialized properly, don't know why
                AdaptorConnections = new Dictionary<WeaponPart, Adaptor>();

            if (AdaptorConnections.ContainsValue(adaptor))//adaptor already connected
                return;

            if (adaptor.WeaponTypeOfAdaptor != child.PartType) //if trying to connect a wrong part
                return;

            //alignment
            child.transform.parent = adaptor.ChildPartTransform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.Euler(0, 0, 0);

            //create connection
            AdaptorConnections.Add(child, adaptor);

            //make connected side visible again
            MaterialHider hider = new MaterialHider();
            hider.DisplayHierarchy(adaptor.ChildPartTransform.gameObject, this.PartID); 
            adaptor.isUnconnected = false;
            
        }

        /// <summary>
        /// adds a child to this part
        /// </summary>
        /// <param name="child">the child to add</param>
        public void AddChild(WeaponPart child)
        {
            //checks
            if (Children.Contains(child))//if part is already a child
                return;
            
            if (Children.Count == Adaptors.Count) //we can't have more children than adaptors
                return;

            //add as child
            Children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// removes a child from this part
        /// </summary>
        /// <param name="child">the child to remove </param>
        public void RemoveChild(WeaponPart child)
        {
            //checks
            if (!Children.Contains(child))//if part is no child
                return;

            //remove child
            Children.Remove(child);
            Adaptor a = AdaptorConnections[child];
            AdaptorConnections.Remove(child);

            if (a != null) //better nullcheck it
            {
                //hide adaptor again
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
            //checks
            if (newParent == null)//passed null, TODO: what if we want to make it a toplevel part? might need to check this
                return;

            if (Parent != null) //if we're not toplevel part, remove from previous parent
                Parent.RemoveChild(this);

            //set as new parent and register as it's child
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
            foreach(WeaponPart child in Children) //disconnect the transforms
            {
                child.transform.parent = null;
                
            }

            //clear all children and connections
            Children.Clear();
            if(AdaptorConnections != null) //sometimes it's not initialized for some reason...
                AdaptorConnections.Clear();

            
        }

        /// <summary>
        /// finds the adaptor by which this part is connected to it's parent
        /// </summary>
        /// <returns>the adaptor by which this part is connected to it's parent, or null if not connected to an adaptor</returns>
        public Adaptor getConnectingAdaptor()
        {
            Adaptor connectingAdaptor = null; //TODO: i'm not sure if this creates a new instance? it seems to work fine in the tests, but maybe come back later and
                                              //research wether this might create any problems

            if (this.Parent == null) //return null if we're toplevel part
                return connectingAdaptor;

            this.Parent.AdaptorConnections.TryGetValue(this, out connectingAdaptor); //get adaptor by which we're connected to parent

            return connectingAdaptor;
        }
    }
}
