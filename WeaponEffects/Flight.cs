using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class Flight : MonoBehaviour
    {
        public Item item;

        public float minForce;
        public float maxForce;

        protected Interactor rightInteractor;
        protected Interactor leftInteractor;
        protected Transform thrustTransform;

        public void Initialize()
        {
            item.OnHeldActionEvent += WeaponItem_OnHeldActionEvent;
            thrustTransform = item.definition.flyDirRef;
        }

        private void WeaponItem_OnHeldActionEvent(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
            {
                if (interactor.side == Side.Right)
                {
                    rightInteractor = interactor;
                }
                else
                {
                    leftInteractor = interactor;
                }
            }
            else if (action == Interactable.Action.AlternateUseStop || action == Interactable.Action.Ungrab)
            {
                if (interactor.side == Side.Right)
                {
                    rightInteractor = null;
                }
                else
                {
                    leftInteractor = null;
                }
            }
        }

        protected void FixedUpdate()
        {
            if (rightInteractor)
            {
                if (leftInteractor)
                {
                    Player.local.locomotion.rb.AddForce(thrustTransform.forward * maxForce, ForceMode.Force);
                } else
                {
                    Player.local.locomotion.rb.AddForce(thrustTransform.forward * minForce, ForceMode.Force);
                }
            }
            else if (leftInteractor)
            {
                Player.local.locomotion.rb.AddForce(thrustTransform.forward * minForce, ForceMode.Force);
            }

        }
    }
}
