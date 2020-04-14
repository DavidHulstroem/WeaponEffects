using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class Smite : MonoBehaviour
    {
        public Item item;
        public float chargeTime;

        private GameObject chargingFX;
        private GameObject activatedFX;
        private GameObject chargedFX;

        /*
        private ParticleSystem chargingParticleSystem;
        private ParticleSystem activatedParticleSystem;
        
        private AudioSource activeAudioSource;
        private AudioSource chargedAudioSource;
        */

        private float lastUseTime;

        private bool isCharged;
        private bool isActivated;
        public void Initialize()
        {
            /*
            chargingParticleSystem = item.definition.GetCustomReference("chargingFX").GetComponent<ParticleSystem>();
            activatedParticleSystem = item.definition.GetCustomReference("activatedFX").GetComponent<ParticleSystem>();

            activeAudioSource = item.definition.GetCustomReference("chargingFX").GetComponent<AudioSource>();
            chargedAudioSource = item.definition.GetCustomReference("chargedFX").GetComponent<AudioSource>();
            */

            chargingFX = item.definition.GetCustomReference("charging")?.gameObject;
            activatedFX = item.definition.GetCustomReference("active")?.gameObject;
            chargedFX = item.definition.GetCustomReference("deactive")?.gameObject;

            item.OnCollisionEvent += Item_OnCollisionEvent;
            item.OnHeldActionEvent += Item_OnHeldActionEvent;

            isCharged = true;
            FXPlayer.StopAllFxOnObject(chargingFX);
            FXPlayer.StopAllFxOnObject(activatedFX);
            FXPlayer.StartAllFxOnObject(chargedFX);
        }

        private void Item_OnHeldActionEvent(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (isCharged)
            {
                if (action == Interactable.Action.AlternateUseStart)
                {
                    if (!isActivated)
                    {
                        ActivateBlade();
                    }
                    else if (isActivated)
                    {
                        DeactivateBlade();
                    }

                }
            }


        }

        private void ActivateBlade()
        {
            isActivated = true;
            FXPlayer.StartAllFxOnObject(activatedFX);
            FXPlayer.StopAllFxOnObject(chargedFX);
            
        }

        private void DeactivateBlade()
        {
            isActivated = false;
            FXPlayer.StartAllFxOnObject(chargedFX);
            FXPlayer.StopAllFxOnObject(activatedFX);
        }

        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (isActivated)
            {
                if (collisionInstance.damageStruct.hitRagdollPart)
                {
                    Creature creature = collisionInstance.targetCollider.GetComponentInParent<Creature>();
                    if (creature != Creature.player)
                    {
                        if (collisionInstance.damageStruct.damageType != Damager.DamageType.Blunt || collisionInstance.damageStruct.damage > 1)
                        {
                            Explode(creature);
                        }
                    } else
                    {
                        creature.health.Damage(ref collisionInstance);
                        UseCharge();
                    }


                } else if (collisionInstance.damageStruct.hitItem)
                {
                    Item itemHit = collisionInstance.otherItem;
                    if (itemHit.mainHandler)
                    {
                        Creature creature = itemHit.mainHandler.bodyHand.body.creature;
                        if (creature != Creature.player)
                        {
                            creature.TryAction(new ActionStagger(collisionInstance.contactNormal, 10f, ActionStagger.Type.FallGround), true);
                            Recoil(creature, collisionInstance);
                        } else
                        {
                            if (itemHit.mainHandler.side == Side.Right)
                            {
                                creature.body.handRight.interactor.UnGrab(false);
                            } else
                            {
                                creature.body.handLeft.interactor.UnGrab(false);
                            }
                            Recoil(creature, collisionInstance);
                        }
                    }


                }
            }
        }

        private void Recoil(Creature creature, CollisionStruct collisionStruct)
        {
            creature.locomotion.rb.velocity = -collisionStruct.contactNormal * 10;
        }

        private void Explode(Creature creature)
        {
            creature.health.Kill();

            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (part != creature.ragdoll.parts[9])
                {
                    if (part.partData.sliceAllowed)
                    {
                        creature.ragdoll.Slice(part.partData.bone);
                    }
                }

            }

            foreach (Collider collider in Physics.OverlapSphere(creature.transform.position, 5))
            {
                if (collider.GetComponent<RagdollPart>())
                {
                    Rigidbody rb = collider.GetComponent<RagdollPart>().GetComponent<Rigidbody>();
                    rb.AddExplosionForce(2000, creature.transform.position, 10);
                }
            }

            UseCharge();

        }

        private void UseCharge()
        {
            //Stop all particles
            FXPlayer.StopAllFxOnObject(chargedFX);
            FXPlayer.StopAllFxOnObject(activatedFX);
            FXPlayer.StartAllFxOnObject(chargingFX);

            isCharged = false;
            isActivated = false;
            lastUseTime = Time.time;
        }

        private void Update()
        {
            if (!isCharged)
            {
                if ((Time.time - lastUseTime) > chargeTime)
                {
                    isCharged = true;
                    FXPlayer.StopAllFxOnObject(chargingFX);
                    FXPlayer.StartAllFxOnObject(chargedFX);
                    // Play is charged particles
                }
            }

            if (item.mainHandler)
            {
                if (item.mainHandler.bodyHand.body.creature)
                {
                    Creature creature = item.mainHandler.bodyHand.body.creature;
                    if (creature != Creature.player)
                    {
                        if (isCharged && !isActivated)
                        {
                            ActivateBlade();
                        }
                    }
                }
            } 
        }
    }
}
