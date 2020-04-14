using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;

namespace WeaponEffects
{
    public static class FXPlayer
    {
        public static void StartAllFxOnObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                foreach (ParticleSystem particleSystem in gameObject.GetComponentsInChildren<ParticleSystem>())
                {
                    particleSystem.Play();
                }
                foreach (AudioSource audioSource in gameObject.GetComponentsInChildren<AudioSource>())
                {
                    audioSource.Play();
                }
                foreach (Light light in gameObject.GetComponentsInChildren<Light>())
                {
                    light.enabled = true;
                }
            }

        }

        public static void StopAllFxOnObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                foreach (ParticleSystem particleSystem in gameObject.GetComponentsInChildren<ParticleSystem>())
                {
                    particleSystem.Stop();
                }
                foreach (AudioSource audioSource in gameObject.GetComponentsInChildren<AudioSource>())
                {
                    audioSource.Stop();
                }
                foreach (Light light in gameObject.GetComponentsInChildren<Light>())
                {
                    light.enabled = false;
                }
            }
        }

       public static void PlaySound(Creature creature, CreatureAudio.Type type) 
       {
            if (creature != null)
            {
                AudioSource audioSource = creature.speak.GetComponent<AudioSource>();

                AudioClip audioClip = null;
                if (creature.umaCharacter)
                {
                    if (creature.umaCharacter.GetSex() == UMARace.Sex.Male)
                    {
                        audioClip = creature.speak.maleAudioPreset.PickAudio(type);
                    }
                    if (creature.umaCharacter.GetSex() == UMARace.Sex.Female)
                    {
                        audioClip = creature.speak.femaleAudioPreset.PickAudio(type);
                    }
                }
                if (audioClip)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
            }
       }
    }
}
