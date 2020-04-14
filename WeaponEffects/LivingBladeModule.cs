using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    public class LivingBladeModule : ItemModule
    {
        public Vector2 lifeSpanMinMax;
        public Vector2 minMaxDelayBetweenActions;
        public float range;
        public float speed;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);


            LivingBladeV2 livingBlade = item.gameObject.AddComponent<LivingBladeV2>();
            livingBlade.item = item;
            livingBlade.lifeSpan = UnityEngine.Random.Range(lifeSpanMinMax.x, lifeSpanMinMax.y);
            livingBlade.minMaxDelayBetweenActions = minMaxDelayBetweenActions;
            livingBlade.range = range;
            livingBlade.speed = speed;
            livingBlade.Inizialize();
        }
    }
}
