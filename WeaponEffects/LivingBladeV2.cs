using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;
using UnityEngine.AI;

namespace WeaponEffects
{
    class LivingBladeV2 : MonoBehaviour
    {
        public Item item;
        public float lifeSpan;
        public float speed;

        public Vector2 minMaxDelayBetweenActions;
        public float range;


        private AudioSource attacksound;
        private GameObject activeFX;
        private GameObject deactiveFX;


        private float startTime;
        private Creature target;

        private bool active;

        private float navUpdatesDelay = 0.2f;

        public void Inizialize()
        {
            attacksound = item.definition.GetCustomReference("attacksound")?.GetComponent<AudioSource>();
            activeFX = item.definition.GetCustomReference("activation")?.gameObject;
            deactiveFX = item.definition.GetCustomReference("deactivation")?.gameObject;

            item.OnUngrabEvent += Item_OnUngrabEvent;
            item.OnGrabEvent += Item_OnGrabEvent;
        }

        private void Item_OnGrabEvent(Handle handle, Interactor interactor)
        {
            active = false;
            target = null;
        }

        private void Item_OnUngrabEvent(Handle handle, Interactor interactor, bool throwing)
        {
            if (item.rb.velocity.magnitude > 5 && !active)
            {
                StartCoroutine(AttackCoroutine(item));
            }

        }

        IEnumerator AttackCoroutine(Item item)
        {
            target = null;
            active = true;

            yield return new WaitForSeconds(0.5f);
            item.rb.useGravity = false;
            FXPlayer.StopAllFxOnObject(deactiveFX);
            FXPlayer.StartAllFxOnObject(activeFX);
            bool first = true;

            startTime = Time.time;


            while (active && (Time.time - startTime) < lifeSpan)
            {
                
                if (target != null && !target.health.isKilled)
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
                    }

                    Vector3 pos;
                    if (Physics.Raycast(item.transform.position, -Vector3.up, out RaycastHit hit))
                    {
                        pos = hit.point;
                    }
                    else
                    {
                        pos = transform.position;
                    }

                    NavMeshPath path = new NavMeshPath();
                    NavMesh.CalculatePath(pos, target.transform.position, NavMesh.AllAreas, path);

                    if (path.corners.Length > 2)
                    {
                        Vector3 corner = new Vector3(path.corners[1].x, path.corners[1].y + 1.5f, path.corners[1].z);
                        item.transform.LookAt(corner);
                        item.rb.velocity = item.transform.forward * speed;
                    }
                    else
                    {
                        item.transform.LookAt(target.ragdoll.parts[(int)HumanBodyBones.Chest].transform);
                        item.rb.velocity = item.transform.forward * speed;
                    }
                    item.Throw(1, Item.FlyDetection.Forced);
                    item.rb.useGravity = false;
                    yield return new WaitForSeconds(navUpdatesDelay);

                } else
                {
                    item.Throw(1, Item.FlyDetection.Forced);
                    target = FindNearstEnemy(item.transform);
                    float delay = UnityEngine.Random.Range(minMaxDelayBetweenActions.x, minMaxDelayBetweenActions.y);
                    if (!first)
                    {
                        yield return new WaitForSeconds(delay);
                    } else
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (target != null)
                    {
                        attacksound?.Play();
                        first = false;
                    }
                    
                }

            }
            item.rb.useGravity = true;
            FXPlayer.StopAllFxOnObject(activeFX);
            FXPlayer.StartAllFxOnObject(deactiveFX);

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
