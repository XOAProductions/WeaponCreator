using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOAProductions.WeaponDesigner
{
    public class LoadingMechanismAdaptor : Adaptor
    {

        public AudioSource AdaptorPlateAudioSource, LockAudioSource;
        public AudioClip AdaptorOpenClip, AdaptorCloseClip, LockOpenClip, LockCloseClip;

        [Range(0, 1)]
        public float AdaptorOpenVolume, AdaptorCloseVolume, LockOpenVolume, LockCloseVolume;

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

        public void OnLockOpen()
        {
            if (isUnconnected)
                return;

            LockAudioSource.PlayOneShot(LockOpenClip, LockOpenVolume);
        }

        public void OnLockClose()
        {
            if (isUnconnected)
                return;

            LockAudioSource.PlayOneShot(LockCloseClip, LockCloseVolume);
        }

    }
}

