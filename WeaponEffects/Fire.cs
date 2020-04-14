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
    class Fire : MonoBehaviour
    {
        public Item item;

        public float fireDamage;
        public Vector2 burnDuration;
        public string whooshId;

        private string defaultWhooshId;

        private GameObject activeFX;
        private GameObject onFireFX;


        private bool active;
        public void Initialize()
        {
            active = false;

            activeFX = item.definition.GetCustomReference("active")?.gameObject;
            onFireFX = item.definition.GetCustomReference("burning")?.gameObject;

            defaultWhooshId = item.data.whooshs[0].fxId;

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
            item.OnCollisionEvent += Item_OnCollisionEvent;
        }

        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (active)
            {
                if (collisionInstance.damageStruct.hitRagdollPart)
                {
                    if (collisionInstance.damageStruct.damageType == Damager.DamageType.Pierce || collisionInstance.damageStruct.damageType == Damager.DamageType.Slash)
                    {
                        if (!collisionInstance.damageStruct.hitRagdollPart.GetComponent<Burning>())
                        {
                            StartCoroutine(Burn(collisionInstance.damageStruct.hitRagdollPart, collisionInstance));
                        }
                            
                    }
                }
            }
        }


        private void Update()
        {
            foreach (Creature creature in Creature.list)
            {
                if (creature != Creature.player && !creature.health.isKilled)
                {
                    if (creature.GetComponentsInChildren<Burning>().Length >= 2)
                    {
                        CreatureAudioPreset creatureAudioPreset = null;

                        if (creature.umaCharacter.GetSex() == UMARace.Sex.Male)
                        {
                            creatureAudioPreset = creature.speak.maleAudioPreset;
                        }
                        else if (creature.umaCharacter.GetSex() == UMARace.Sex.Female)
                        {
                            creatureAudioPreset = creature.speak.femaleAudioPreset;
                        }

                        if (creatureAudioPreset != null)
                        {
                            if (!creatureAudioPreset.fallingSounds.Contains(creature.speak.GetComponent<AudioSource>().clip))
                            {
                                if (UnityEngine.Random.Range(0f, 1f) >  0.5f)
                                {
                                    FXPlayer.PlaySound(creature, CreatureAudio.Type.Falling);
                                } else
                                {
                                    FXPlayer.PlaySound(creature, CreatureAudio.Type.Death);
                                }
                                
                                
                            }
                        }

                        creature.StopAction<ActionParry>();
                        creature.StopAction<ActionWatch>();
                        creature.StopAction<ActionMove>();
                        creature.StopAction<ActionStrafe>();
                        creature.StopAction<ActionGrab>();
                        creature.TryAction(new ActionShock(10, 1), true);
                    }
                }
            }

        }

        private void Item_OnHeldActionEvent(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
            {
                if (!active)
                {
                    ActivateFireBlade();
                } else
                {
                    DeactivateFireBlade();
                }
            }
        }



        private void ActivateFireBlade()
        {
            active = true;
            FXPlayer.StartAllFxOnObject(activeFX);
            if (whooshId != null)
            {
                item.GetComponentInChildren<Whoosh>().Load(Catalog.current.GetData<FXData>(whooshId), Whoosh.Trigger.Always, item.data.whooshs[0].minVelocity, item.data.whooshs[0].maxVelocity);
            }
            
        }

        private void DeactivateFireBlade()
        {
            active = false;
            FXPlayer.StopAllFxOnObject(activeFX);
            if (whooshId != null)
            {
                item.GetComponentInChildren<Whoosh>().Load(Catalog.current.GetData<FXData>(defaultWhooshId), Whoosh.Trigger.Always, item.data.whooshs[0].minVelocity, item.data.whooshs[0].maxVelocity);
            }
            
        }

        IEnumerator Burn(RagdollPart ragdollPart, CollisionStruct collisionStruct)
        {
            Burning burning = ragdollPart.gameObject.AddComponent<Burning>();

            Creature creature = ragdollPart.ragdoll.creature;
            float duration = UnityEngine.Random.Range(burnDuration.x, burnDuration.y);
            float startTime = Time.time;
            GameObject burningParticles = GameObject.Instantiate(onFireFX, ragdollPart.transform);
            burningParticles.transform.localPosition = new Vector3(0, 0, 0);
            
            FXPlayer.StartAllFxOnObject(burningParticles);

            

            int i = 0;
            while ((Time.time - startTime) < duration)
            {
                if (i >= 4)
                {
                    
                    i = 0;
                }
                CollisionStruct collisionStruct1 = new CollisionStruct(new DamageStruct(Damager.DamageType.Poison, fireDamage), null, null, null, null, null, null, null);
                creature.health.Damage(ref collisionStruct1);
                i++;
                yield return new WaitForSeconds(0.5f);
                
            }

            FXPlayer.StopAllFxOnObject(burningParticles);

            yield return new WaitUntil(() => !burningParticles.GetComponentInChildren<ParticleSystem>().isPlaying);

            Destroy(burning);
            Destroy(burningParticles);
        }


    }
}
