using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOAProductions.WeaponDesigner
{
    public class ShootingMechanismAdaptor : Adaptor
    {

        public AudioSource AdaptorPlateAudioSource, ScrewAudioSource;
        public AudioClip AdaptorOpenClip, AdaptorCloseClip, ScrewOpenClip, ScrewCloseClip;

        [Range(0,1)]
        public float AdaptorOpenVolume, AdaptorCloseVolume, ScrewOpenVolume, ScrewCloseVolume;

        public void OnAdaptorOpen()
        {
            if (isUnconnected)
                return;

            AdaptorPlateAudioSource.PlayOneShot(AdaptorOpenClip, AdaptorOpenVolume);
        }

        public void OnAdaptorClose()
        {
            if (isUnconnected)
                return;

            AdaptorPlateAudioSource.PlayOneShot(AdaptorCloseClip, AdaptorCloseVolume);
        }

        public void OnScrewOpen()
        {
            if (isUnconnected)
                return;

            ScrewAudioSource.PlayOneShot(ScrewOpenClip, ScrewOpenVolume);
        }

        public void OnScrewClose()
        {
            if (isUnconnected)
                return;

            ScrewAudioSource.PlayOneShot(ScrewCloseClip, ScrewCloseVolume);
        }
 
    }
}
