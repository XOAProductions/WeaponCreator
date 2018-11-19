using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XOAProductions.WeaponDesigner
{
    public class Adaptor : MonoBehaviour
    {

        public string CloseAnimationBoolName = "Closed";
        public Animator anim;
        public Transform ChildPartTransform;
        public WeaponPartType WeaponTypeOfAdaptor;

        private void Start()
        {
            anim = GetComponent<Animator>();
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




    }
}
