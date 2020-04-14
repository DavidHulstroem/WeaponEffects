using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class ShadowModule : ItemModule
    {
        public float chargeTime;
        public float invisibilityTime;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);

            Shadow shadow = item.gameObject.AddComponent<Shadow>();
            shadow.item = item;
            shadow.chargeTime = chargeTime;
            shadow.invisibilityTime = invisibilityTime;
            shadow.Initialize();
        }


    }
}
