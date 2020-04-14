using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;

namespace WeaponEffects
{
    class Venom : MonoBehaviour
    {
        public Item item;
        public float damage;
        public float duration;
        public float delayBetweenDamages;
        public void Initialize()
        {
            item.OnCollisionEvent += Item_OnCollisionEvent;
        }

        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (collisionInstance.targetCollider.GetComponentInParent<Creature>())
            {
                Creature creature = collisionInstance.targetCollider.GetComponentInParent<Creature>();

                if (!creature.health.isKilled)
                {
                    if (collisionInstance.damageStruct.damageType == Damager.DamageType.Blunt || collisionInstance.damageStruct.damage < 1)
                    {
                        if (collisionInstance.damageStruct.damageType != Damager.DamageType.Pierce)
                        {
                            return;
                        }
                    }

                    StartCoroutine(VenomCoroutine(creature, collisionInstance));
                }
            }
        }

        IEnumerator VenomCoroutine(Creature creature, CollisionStruct refCollisionStruct)
        {
            float startTime = Time.time;
            while ((Time.time - startTime) < duration)
            {

                yield return new WaitForSeconds(delayBetweenDamages);


                CollisionStruct collisionStruct = refCollisionStruct;
                collisionStruct.damageStruct.damage = damage;
                collisionStruct.damageStruct.damageType = Damager.DamageType.Poison;
                collisionStruct.impulseVelocity = new Vector3(0, 0, 0);
                collisionStruct.damageStruct.recoil = 1;

                creature.health.Damage(ref collisionStruct);


            }
        }
    }
}
