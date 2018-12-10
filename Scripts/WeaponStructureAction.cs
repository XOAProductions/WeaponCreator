﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOAProductions.WeaponDesigner
{
    public enum WeaponStructureActionType
    {
        AddPart,
        ReplacePart,
        RemovePart
    }
   

    /// <summary>
    /// Handles actions on a weaponstructure and the animations of the adaptors, so we don't have to worry about controlling animations anytime we
    /// want to perform an action on a structure
    /// </summary>
    public class WeaponStructureAction
    {
        readonly WeaponStructure Structure;

        readonly WeaponPart PartToReplace;
        readonly WeaponPart PartToAdd;
        readonly WeaponPart PartToRemove;
        readonly WeaponPart ReplacementPart;
        public   Adaptor m_Adaptor { get; private set; }

        readonly WeaponStructureActionType TypeOfAction;
        

        public bool Finalized { get; private set; }

        private List<Adaptor> AffectedAdaptors = new List<Adaptor>();

        #region public methods

        /// <summary>
        /// configures this to add a part
        /// </summary>
        /// <param name="adaptor"></param>
        /// <param name="partToAdd"></param>
        public WeaponStructureAction (Adaptor adaptor, WeaponPart partToAdd, WeaponStructure structure)
        {
            m_Adaptor = adaptor;
            PartToAdd = partToAdd;

            Structure = structure;

            TypeOfAction = WeaponStructureActionType.AddPart;
            

            Finalized = false;
        }

        /// <summary>
        /// configures this to remove a part
        /// </summary>
        /// <param name="partToRemove"></param>
        public WeaponStructureAction(WeaponPart partToRemove, WeaponStructure structure)
        {
            PartToRemove = partToRemove;

            Structure = structure;

            TypeOfAction = WeaponStructureActionType.RemovePart;
            

            Finalized = false;
        }

        /// <summary>
        /// configures this to replace a part
        /// </summary>
        /// <param name="partToReplace"></param>
        /// <param name="replacementPart"></param>
        public WeaponStructureAction(WeaponPart partToReplace, WeaponPart replacementPart, WeaponStructure structure)
        {
            PartToReplace = partToReplace;
            ReplacementPart = replacementPart;

            Structure = structure;

            TypeOfAction = WeaponStructureActionType.ReplacePart;
            

            Finalized = false;
        }

        /// <summary>
        /// Begins the specified action that this instance should perform
        /// </summary>
        public void BeginAction()
        {
            //We want to add a part
            if(TypeOfAction == WeaponStructureActionType.AddPart)
            {
                m_Adaptor.OnAnimatorOpened += OnAdaptorOpenedAddPart; //bind to open call of adaptor
                m_Adaptor.Open(); //open adaptor
                
            }

            //We want to remove a part
            else if(TypeOfAction == WeaponStructureActionType.ReplacePart)
            {
                var ConnectingAdaptor = PartToReplace.getConnectingAdaptor(); //see if part that will be removed is connected to an adaptor

                m_Adaptor = ConnectingAdaptor;

                if (ConnectingAdaptor == null) //if null, we have a toplevel part, thus we need to call directly
                    OnAdaptorOpenedReplacePart();
                else //else we open the adaptor and wait for it
                {
                    ConnectingAdaptor.OnAnimatorOpened += OnAdaptorOpenedReplacePart;
                    ConnectingAdaptor.Open();
                }
            }

            else if (TypeOfAction == WeaponStructureActionType.RemovePart)
            {
                var ConnectingAdaptor = PartToRemove.getConnectingAdaptor();

                m_Adaptor = ConnectingAdaptor;

                if(ConnectingAdaptor == null)//if null, we have a toplevel part, thus we need to call directly
                {
                    OnAdaptorOpenedRemovePart();
                }
                else //else we open the adaptor and wait for it
                {
                    ConnectingAdaptor.OnAnimatorOpened += OnAdaptorOpenedRemovePart;
                    ConnectingAdaptor.Open();
                }
            }
        }

        #endregion


        /// <summary>
        /// This instance's action is completed
        /// </summary>
        void OnActionFinalized()
        {
            m_Adaptor.OnAnimatorNotFullyOpened -= OnActionFinalized;
            Finalized = true;
        }

        #region AddPart

        /// <summary>
        /// Once the target adaptor is opened, adds the specified part
        /// </summary>
        void OnAdaptorOpenedAddPart()
        {
            m_Adaptor.OnAnimatorOpened -= OnAdaptorOpenedAddPart;
            Structure.AddWeaponPart(m_Adaptor, PartToAdd);

            m_Adaptor.OnAnimatorNotFullyOpened += OnActionFinalized;
            m_Adaptor.Close();
           
        }



        #endregion

        #region ReplacePart


        /// <summary>
        /// Once connecting adaptor is opened, collects all effected adaptors and opens them
        /// </summary>
        void OnAdaptorOpenedReplacePart()
        {
            try
            {
                m_Adaptor.OnAnimatorOpened -= OnAdaptorOpenedReplacePart;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            foreach(Adaptor a in PartToReplace.Adaptors)
            {
                if (PartToReplace.AdaptorConnections.ContainsValue(a))
                {
                    AffectedAdaptors.Add(a);
                    a.OnAnimatorOpened += OnAffectedAdaptorOpened;
                    a.Open();
                }
            }

            
            
        }

        /// <summary>
        /// Once an affected adaptors is opened, checks wether all of them are opened now, and calls OnAllAffectedAdaptorsOpenedReplacePart
        /// </summary>
        void OnAffectedAdaptorOpened()
        {
            
            foreach(Adaptor a in AffectedAdaptors)
            {
                if (!a.isOpened)
                {
                    return;
                }
            }

            foreach (Adaptor a in AffectedAdaptors)
                a.OnAnimatorOpened -= OnAffectedAdaptorOpened;

            OnAllAffectedAdaptorsOpenedReplacePart();

        }

        /// <summary>
        /// Replaces the part
        /// </summary>
        void OnAllAffectedAdaptorsOpenedReplacePart()
        {
            Structure.ReplaceWeaponPart(PartToReplace, ReplacementPart);

            foreach (Adaptor a in ReplacementPart.Adaptors)
                a.Close();

            m_Adaptor.OnAnimatorNotFullyOpened += OnActionFinalized;
            m_Adaptor.Close();
        }


        #endregion

        #region RemovePart

        /// <summary>
        /// Once the connecting adaptor is opened, removes the part
        /// </summary>
        void OnAdaptorOpenedRemovePart()
        {
            try
            {
                m_Adaptor.OnAnimatorOpened -= OnAdaptorOpenedRemovePart;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            Structure.RemoveWeaponPart(PartToRemove);


            m_Adaptor.OnAnimatorNotFullyOpened += OnActionFinalized;
            m_Adaptor.Close();
            
        }

        #endregion
    }
}
