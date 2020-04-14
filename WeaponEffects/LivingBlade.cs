using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using BS;

namespace WeaponEffects
{
    class LivingBlade : MonoBehaviour
    {
        public Item item;
        public float lifeSpan;

        public Vector2 minMaxDelayBetweenActions;
        public float range;

        private float time;
        private Creature target;

        public void Inizialize()
        {
            item.OnUngrabEvent += Item_OnUngrabEvent;
            item.OnGrabEvent += Item_OnGrabEvent;
        }

        private void Item_OnGrabEvent(Handle handle, Interactor interactor)
        {
            if (target != null)
            {
                time = lifeSpan;
            }
        }

        private void Item_OnUngrabEvent(Handle handle, Interactor interactor, bool throwing)
        {
            if (throwing)
            {
                time = 0;
                StartCoroutine(AttackCoroutine(item));
            }
            
        }

        IEnumerator AttackCoroutine(Item thrownItem)
        {
            target = FindNearstEnemy(thrownItem.transform);
            if (target != null)
            {
                yield return new WaitForSeconds(0.5f);
                time += 0.5f;

                item.rb.useGravity = false;
                while (!target.health.isKilled && time < lifeSpan)
                {
                    if (item.isPenetrating)
                    {
                        yield return new WaitForSeconds(0.2f);
                        foreach (Damager damager in item.damagers)
                        {
                            damager.UnPenetrateAll();
                        }
                        item.rb.velocity = -item.transform.forward * 5;
                        yield return new WaitForSeconds(0.3f);
                        time += 0.5f;
                    }
                    else
                    {
                        item.transform.LookAt(target.ragdoll.parts[(int)HumanBodyBones.Chest].transform);
                        item.Throw(5, Item.FlyDetection.Forced);
                        item.rb.velocity = item.transform.forward * 20;
                        yield return new WaitForSeconds(0.1f);
                        time += 0.1f;
                    }
                }
                
                if (time > lifeSpan)
                {
                    item.rb.useGravity = true;
                    target = null;
                }
                else
                {
                    foreach (Damager damager in item.damagers)
                    {
                        damager.UnPenetrateAll(); 
                    }
                    item.rb.velocity = Vector3.up * 1;
                    float delay = UnityEngine.Random.Range(minMaxDelayBetweenActions.x, minMaxDelayBetweenActions.y);
                    yield return new WaitForSeconds(delay);
                    time += delay;
                    StartCoroutine(AttackCoroutine(thrownItem));
                }
            } else
            {
                yield return new WaitForSeconds(2f);
                time += 3f;
                if (time > lifeSpan)
                {
                    item.rb.useGravity = true;
                    target = null;
                } else
                {
                    StartCoroutine(AttackCoroutine(thrownItem));
                }
            }
            
        }

        private Creature FindNearstEnemy(Transform transform)
        {
            float dist = Mathf.Infinity;
            Creature returnCreature = null;
            foreach (Creature creature in Creature.list)
            {
                if (creature != Creature.player && !creature.health.isKilled)
                {
                    float _d = Vector3.Distance(creature.transform.position, transform.position);
                    if (_d < range)
                    {
                        if (_d < dist)
                        {
                            returnCreature = creature;
                            dist = _d;
                        }
                    }

                }
            }
            return returnCreature;
        }
    }
}
