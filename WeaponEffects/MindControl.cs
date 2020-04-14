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
    class MindControlModule : ItemModule
    {
        public float damagePerSec;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            MindControl mindControl = item.gameObject.AddComponent<MindControl>();
            mindControl.item = item;
            mindControl.damage = damagePerSec;
            mindControl.Initialize();
        }
    }

    class MindControl : MonoBehaviour
    {
        public Item item;
        public float damage = 1;

        private GameObject controlledFX;

        public void Initialize()
        {
            controlledFX = item.definition.GetCustomReference("active")?.gameObject;
            item.OnCollisionEvent += Item_OnCollisionEvent;
        }

        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (collisionInstance.damageStruct.hitRagdollPart?.ragdoll.creature)
            {
                Creature creature = collisionInstance.damageStruct.hitRagdollPart?.ragdoll.creature;
                if (creature != Creature.player && !creature.health.isKilled && creature.factionId != 2) 
                {
                    if (collisionInstance.damageStruct.penetration == DamageStruct.Penetration.Hit || collisionInstance.damageStruct.penetration == DamageStruct.Penetration.Pressure)
                    {
                        int faction = creature.factionId;
                        StartCoroutine(MindControlCoroutine(creature, faction));
                    }
                }
            }
        }

        IEnumerator MindControlCoroutine(Creature creature, int origFac)
        {
            creature.SetFaction(2);
            GameObject fxs = null;
            if (controlledFX != null)
            {
                fxs = GameObject.Instantiate(controlledFX, creature.body.headBone);
                fxs.transform.localPosition = new Vector3(0, 0, 0);

                FXPlayer.StartAllFxOnObject(fxs);
            }


            while (item.isPenetrating && !creature.health.isKilled)
            {
                creature.SetFaction(2);
                CollisionStruct collisionStruct = new CollisionStruct(new DamageStruct(Damager.DamageType.Poison, damage), null, null, null, null, null, null, null);
                creature.health.Damage(ref collisionStruct);
                yield return new WaitForSeconds(1f);
            }

            if (fxs != null)
            {
                Destroy(fxs);
            }

            creature.SetFaction(origFac);
        }

    }
}
