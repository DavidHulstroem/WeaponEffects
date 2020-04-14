using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class SmiteModule : ItemModule
    {
        public float chargeTime;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            Smite smite = item.gameObject.AddComponent<Smite>();
            smite.item = item;
            smite.chargeTime = chargeTime;
            smite.Initialize();
        }
    }
}
