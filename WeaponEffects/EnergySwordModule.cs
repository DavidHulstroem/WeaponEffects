using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class EnergySwordModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);

            EnergySword energySword = item.gameObject.AddComponent<EnergySword>();
            energySword.item = item;
            energySword.Initialize();
        }
    }
}
