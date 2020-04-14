using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;

namespace WeaponEffects
{
    class Ice : MonoBehaviour
    {
        public Item item;
        public float duration;
        public float animSpeed;


        public void Initialize()
        {
            item.OnCollisionEvent += Item_OnCollisionEvent;
        }

        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (collisionInstance.damageStruct.hitRagdollPart)
            {
                if (collisionInstance.damageStruct.damage > 1)
                {
                    Creature creature = collisionInstance.targetCollider.GetComponentInParent<Creature>();
                    if (creature != Creature.player && !creature.health.isKilled)
                    {
                        if (creature.animator.speed == 1)
                        {
                            StartCoroutine(SlowCoroutine(creature));
                        }
                    }
                }
            }
        }

        IEnumerator SlowCoroutine(Creature targetCreature)
        {
            targetCreature.animator.speed *= (animSpeed / 100);
            targetCreature.locomotion.speed *= (animSpeed / 100);
            yield return new WaitForSeconds(duration);
            targetCreature.animator.speed /= (animSpeed / 100);
            targetCreature.locomotion.speed /= (animSpeed / 100);
        }
    }
}
