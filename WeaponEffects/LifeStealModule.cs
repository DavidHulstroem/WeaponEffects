using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class LifeStealModule : ItemModule
    {
        public float dmgPercentPerSec;
        public float lifeStealPercent;
        public float pierceDamagePerSec;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            LifeSteal lifeSteal = item.gameObject.AddComponent<LifeSteal>();
            lifeSteal.item = item;
            lifeSteal.dmgPercentPerSec = dmgPercentPerSec;
            lifeSteal.lifeStealPercent = lifeStealPercent;
            lifeSteal.pierceDamagePerSec = pierceDamagePerSec;
            lifeSteal.Initialize();
        }
    }
}
