using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XOAProductions.WeaponDesigner
{ 
    public enum AdaptorTypes
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
        
    }
    public class Adaptor : MonoBehaviour
    {

        //the bool that controls the state of the adaptor animation
        public string CloseAnimationBoolName = "Closed";
        //animator of the adaptor
        public Animator anim;
        //where child weaponparts are attached
        public Transform ChildPartTransform;
        //the type of childparts that can be attached
        public WeaponPartType WeaponTypeOfAdaptor;
        //is the adaptor opened?
        public bool isOpened = false;
        //is a child connected?
        public bool isUnconnected = true;


        //events
        public delegate void AdaptorStateChange();
        /// <summary>
        /// called when adaptor is fully opened
        /// </summary>
        public  event AdaptorStateChange OnAnimatorOpened = delegate { };
        /// <summary>
        /// called as soon as adaptor starts to close
        /// </summary>
        public  event AdaptorStateChange OnAnimatorNotFullyOpened = delegate { };

        //initializes some variables
        private void Start()
        {
            
            anim = GetComponent<Animator>();
            if(anim != null)
            isOpened = anim.GetBool(CloseAnimationBoolName);
        }

        /// <summary>
        /// Closes the Adaptor (animated)
        /// </summary>
        public void Close()
        {
            anim.SetBool(CloseAnimationBoolName, true);
        }

        /// <summary>
        /// Opens the Adaptor (animated)
        /// </summary>
        public void Open()
        {
            anim.SetBool(CloseAnimationBoolName, false);
        }

        /// <summary>
        /// called by animator when adaptor reaches completely open state
        /// </summary>
        public void AdaptorFullyOpened()
        {
            isOpened = true;
            OnAnimatorOpened();
        }

        /// <summary>
        /// called by animator as soon as adaptor isn't completely open
        /// </summary>
        public void AdaptorNotFullyOpened()
        {
            isOpened = false;
            OnAnimatorNotFullyOpened();
        }


    }
}
