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

        public string CloseAnimationBoolName = "Closed";
        public Animator anim;
        public Transform ChildPartTransform;
        public WeaponPartType WeaponTypeOfAdaptor;
        public bool isOpened = false;
        public bool isUnconnected = true;

        public delegate void AdaptorStateChange();
        public  event AdaptorStateChange OnAnimatorOpened = delegate { };
        public  event AdaptorStateChange OnAnimatorNotFullyOpened = delegate { };

        private void Start()
        {
            
            anim = GetComponent<Animator>();
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

        public void AdaptorFullyOpened()
        {
            isOpened = true;
            OnAnimatorOpened();
        }

        public void AdaptorNotFullyOpened()
        {
            isOpened = false;
            OnAnimatorNotFullyOpened();
        }


    }
}
