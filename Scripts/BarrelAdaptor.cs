using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOAProductions.WeaponDesigner
{
    public class BarrelAdaptor : Adaptor
    {
        public AudioSource BarrelNutAudioSource, PressureAudioSource, AdaptorAudioSource, SpringsAudioSource;
        public AudioClip PressurizeClip, DepressurizeClip, TwistOpenClip, TwistClosedClip, PullOutClip, PushInClip, ConnectSpringsClip, DisconnectSpringsClip;
        [Range(0,1)]
        public float PressurizeVolume, DepressurizeVolume, TwistOpenVolume, TwistClosedVolume, PulloutVolume, PushInVolume, ConnectSpringsVolume, DisconnectSpringsVolume;
        public ParticleSystem PressureParticles, ContinuusSmoke;


        private void StopPressureParticles()
        {
            PressureParticles.Stop();
            ContinuusSmoke.Play();
        }

        public void OnDepressurize()
        {
            if (isUnconnected)
                return;

            //barrel nut is opened, play depressurize sound...
            PressureAudioSource.PlayOneShot(DepressurizeClip, DepressurizeVolume);
            PressureParticles.Play();
            Invoke("StopPressureParticles", 0.3f);
        }
        
        public void OnPressurize()
        {
            if (isUnconnected)
                return;

            //barrel nut is closed, play pressurize sound...
            PressureAudioSource.PlayOneShot(PressurizeClip, PressurizeVolume);
            
        }

        public void OnBarrelNutTwistOpen()
        {
            if (isUnconnected)
                return;
            //BarrelNut is twisted open...
            BarrelNutAudioSource.PlayOneShot(TwistOpenClip, TwistOpenVolume);
        }

        public void OnBarrelNutTwistClosed()
        {
            if (isUnconnected)
                return;
            //barrelNut ist twisted closed...
            BarrelNutAudioSource.PlayOneShot(TwistClosedClip, TwistClosedVolume);
            ContinuusSmoke.Stop();
        }

        public void OnAdaptorPullOut()
        {
            if (isUnconnected)
                return;
            //Adaptor is pulled out...
            AdaptorAudioSource.PlayOneShot(PullOutClip, PulloutVolume);
        }

        public void OnAdaptorPushIn()
        {
            if (isUnconnected)
                return;
            //adaptor is pushed in...
            AdaptorAudioSource.PlayOneShot(PushInClip, PushInVolume);
        }

        public void OnSpringsDisconnect()
        {
            if (isUnconnected)
                return;
            //springs are disconnected...
            SpringsAudioSource.PlayOneShot(DisconnectSpringsClip, DisconnectSpringsVolume);
        }

        public void OnSpringsConnect()
        {
            if (isUnconnected)
                return;
            //springs are connected...
            SpringsAudioSource.PlayOneShot(ConnectSpringsClip, ConnectSpringsVolume);
        }
    }
}
