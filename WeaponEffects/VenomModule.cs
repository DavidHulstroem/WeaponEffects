using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class VenomModule : ItemModule
    {
        public Vector2 damageMinMax;
        public float duration;
        public float delayBetweenDamages;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);

            Venom venom = item.gameObject.AddComponent<Venom>();
            venom.item = item;
            venom.damage = UnityEngine.Random.Range(damageMinMax.x, damageMinMax.y);
            venom.duration = duration;
            venom.delayBetweenDamages = delayBetweenDamages;
            venom.Initialize();
        }
    }
}
