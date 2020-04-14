using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class IceModule : ItemModule
    {
        public float duration;
        public float animSpeedPercent;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            Ice water = item.gameObject.AddComponent<Ice>();
            water.item = item;
            water.duration = duration;
            water.animSpeed = animSpeedPercent;
            water.Initialize();
        }
    }
}
