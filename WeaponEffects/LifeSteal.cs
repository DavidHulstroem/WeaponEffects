using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class LifeSteal : MonoBehaviour
    {
        public Item item;
        public float dmgPercentPerSec;
        public float lifeStealPercent;
        public float pierceDamagePerSec;

        public Creature handlerCreature;

        private GameObject holdingFX;
        private GameObject drainingFX;

        protected Interactor rightInteractor;
        protected Interactor leftInteractor;

        private float startTime;

        public void Initialize()
        {
            holdingFX = item.definition.GetCustomReference("active")?.gameObject;
            drainingFX = item.definition.GetCustomReference("draining")?.gameObject;
            FXPlayer.StopAllFxOnObject(holdingFX);
            FXPlayer.StopAllFxOnObject(drainingFX);

            item.OnGrabEvent += Item_OnGrabEvent;
            item.OnCollisionEvent += Item_OnCollisionEvent;
            item.OnUngrabEvent += Item_OnUngrabEvent;
        }

        private void Item_OnUngrabEvent(Handle handle, Interactor interactor, bool throwing)
        {
            if (item.handlers.Count <= 0)
            {
                FXPlayer.StopAllFxOnObject(holdingFX);
                handlerCreature = null;
            }
        }

        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (collisionInstance.damageStruct.hitRagdollPart)
            {
                Creature creature = collisionInstance.targetCollider.GetComponentInParent<Creature>();
                if (collisionInstance.damageStruct.damage > 1 && creature != handlerCreature)
                {
                    
                    float damage = collisionInstance.damageStruct.damage;

                    float stolenHealth = damage * (lifeStealPercent / 100);

                    handlerCreature.health.currentHealth = Mathf.Clamp(handlerCreature.health.currentHealth + stolenHealth, 0, handlerCreature.health.maxHealth);
                    

                    if (collisionInstance.damageStruct.penetration == DamageStruct.Penetration.Hit)
                    {
                        StartCoroutine(LifeStealPenetration(creature));
                    }
                }
            }
        }

        private void Item_OnGrabEvent(Handle handle, Interactor interactor)
        {
            handlerCreature = interactor.GetComponentInParent<Creature>();
            FXPlayer.StartAllFxOnObject(holdingFX);
        }

        private void Update()
        {
            if (handlerCreature != null && item.mainHandler != null)
            {
                if ((Time.time-startTime) > 1f)
                {
                    if (handlerCreature.health.currentHealth > 1)
                    {
                        float lostHealth = handlerCreature.health.maxHealth * (dmgPercentPerSec / 100);

                        handlerCreature.health.currentHealth = Mathf.Clamp(handlerCreature.health.currentHealth - lostHealth, 1, handlerCreature.health.maxHealth);

                        if (item.mainHandler.side == Side.Right)
                        {
                            PlayerControl.handRight.HapticPlayClip(Catalog.current.gameData.haptics.spellSelected);
                        } else
                        {
                            PlayerControl.handLeft.HapticPlayClip(Catalog.current.gameData.haptics.spellSelected);
                        }
                        
                        
                    }
                    startTime = Time.time;
                }
            } 
        }

        IEnumerator LifeStealPenetration(Creature creature)
        {
            FXPlayer.StartAllFxOnObject(drainingFX);
            creature.StopAction<ActionParry>();
            creature.StopAction<ActionWatch>();
            creature.StopAction<ActionMove>();
            creature.StopAction<ActionStrafe>();
            creature.StopAction<ActionGrab>();
            creature.TryAction(new ActionShock(10, 10f), true);
            while (item.isPenetrating && !creature.health.isKilled)
            {
                yield return new WaitForSeconds(0.1f);
                CollisionStruct collisionStruct = new CollisionStruct(new DamageStruct(Damager.DamageType.Poison, pierceDamagePerSec/10), null, null, null, null, null, null, null);
                creature.health.Damage(ref collisionStruct);

                handlerCreature.health.currentHealth = Mathf.Clamp(handlerCreature.health.currentHealth + ((pierceDamagePerSec/10)*(lifeStealPercent/100)), 0, handlerCreature.health.maxHealth);
            }
            creature.StopAction<ActionShock>();
            FXPlayer.StopAllFxOnObject(drainingFX);
        }
    }
}
