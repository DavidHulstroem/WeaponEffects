using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class FireModule : ItemModule
    {
        public float fireDamage;
        public Vector2 burnDurationMinMax;
        public string activeWhooshId;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            Fire fire = item.gameObject.AddComponent<Fire>();
            fire.item = item;
            fire.fireDamage = fireDamage;
            fire.burnDuration = burnDurationMinMax;
            fire.whooshId = activeWhooshId;
            fire.Initialize();
        }
    }
}
