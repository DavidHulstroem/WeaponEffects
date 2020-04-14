using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class EnergySword : MonoBehaviour
    {
        public Item item;

        private ParticleSystem swordParticles;
        private GameObject collisionGroupObject;
        private GameObject bladeMeshObject;

        private AudioSource activation;
        private AudioSource deactivation;
        private AudioSource loop;

        private bool activated;

        public void Initialize()
        {
            swordParticles = item.definition.GetCustomReference("particles").transform.GetComponent<ParticleSystem>();
            collisionGroupObject = item.definition.GetCustomReference("collisiongroup").transform.gameObject;
            try
            {
                bladeMeshObject = item.definition.GetCustomReference("blademesh").transform.gameObject;
            }
            catch (Exception)
            {
                bladeMeshObject = new GameObject("bladeRef");
            }


            activation = item.definition.GetCustomReference("activation").transform.GetComponent<AudioSource>();
            deactivation = item.definition.GetCustomReference("deactivation").transform.GetComponent<AudioSource>();
            loop = item.definition.GetCustomReference("loop").transform.GetComponent<AudioSource>();


            activated = false;

            swordParticles.Stop();
            collisionGroupObject.SetActive(false);
            bladeMeshObject.SetActive(false);

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
            {
                if (!activated)
                {
                    BladeActivate(true);
                }
                else
                {
                    BladeActivate(false);
                }
            }
        }

        private void BladeActivate(bool active)
        {
            foreach (Whoosh whoosh in item.GetComponentsInChildren<Whoosh>())
            {
                whoosh.gameObject.SetActive(active);
            }
            bladeMeshObject.SetActive(active);
            collisionGroupObject.SetActive(active);
            activated = active;
            if (active)
            {
                swordParticles.Play();
                activation.Play();
                loop.Play();

            }
            else
            {
                swordParticles.Stop();
                deactivation.Play();
                loop.Stop();
                foreach (Damager damager in item.damagers)
                {
                    damager.UnPenetrateAll();
                }
            }
        }
    }
}
