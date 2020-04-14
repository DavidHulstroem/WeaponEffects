using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class Shadow : MonoBehaviour
    {
        public Item item;
        public float chargeTime;
        public float invisibilityTime;

        private GameObject activeFX;
        private GameObject chargedFX;

        private float lastUseTime;

        private Creature hiddenCreature;

        public void Initialize()
        {
            lastUseTime = 0;
            hiddenCreature = null;

            activeFX = item.definition.GetCustomReference("active")?.gameObject;
            chargedFX = item.definition.GetCustomReference("deactive")?.gameObject;

            FXPlayer.StopAllFxOnObject(activeFX);
            FXPlayer.StartAllFxOnObject(chargedFX);


            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
            {
                if ((Time.time - lastUseTime) > (chargeTime + invisibilityTime))
                {
                    FXPlayer.StartAllFxOnObject(activeFX);
                    FXPlayer.StopAllFxOnObject(chargedFX);
                    lastUseTime = Time.time;
                    StartCoroutine(InvisibilityCoroutine(interactor.bodyHand.body.creature));
                }
            }
        }

        private void LateUpdate()
        {
            if (hiddenCreature)
            {
                foreach (SkinnedMeshRenderer renderer in hiddenCreature.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.enabled = false;
                }
            }
            if (item.mainHandler)
            {
                if (item.mainHandler.bodyHand.body.creature)
                {
                    Creature creature = item.mainHandler.bodyHand.body.creature;
                    if (creature != Creature.player)
                    {
                        if ((Time.time - lastUseTime) > (chargeTime + invisibilityTime))
                        {
                            FXPlayer.StartAllFxOnObject(activeFX);
                            FXPlayer.StopAllFxOnObject(chargedFX);
                            lastUseTime = Time.time;
                            StartCoroutine(InvisibilityCoroutine(item.mainHandler?.bodyHand.body.creature));
                        }
                    }
                }
            }

        }

        IEnumerator InvisibilityCoroutine( Creature creature)
        {
            hiddenCreature = creature;

            while ((Time.time - lastUseTime) < invisibilityTime)
            {
                if (creature == Creature.player)
                {
                    foreach (Creature creature2 in Creature.list)
                    {
                        if (creature2 != Creature.player && !creature2.health.isKilled && creature2.factionId != -1)
                        {
                            StartCoroutine(ChangeFaction(creature2, lastUseTime));
                        }
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }

            FXPlayer.StopAllFxOnObject(activeFX);
            hiddenCreature = null;
            yield return new WaitForSeconds(chargeTime);
            FXPlayer.StartAllFxOnObject(chargedFX);

            PlayerControl.GetHand(item.mainHandler.playerHand.side)?.HapticPlayClip(Catalog.current.gameData.haptics.spellSelected, 1);
        }

        IEnumerator ChangeFaction(Creature creature, float timeS)
        {
            int fac = creature.factionId;
            creature.SetFaction(-1);
            yield return new WaitUntil(() => (Time.time - timeS) > invisibilityTime);
            creature.SetFaction(fac);

        }
        
    }
}
