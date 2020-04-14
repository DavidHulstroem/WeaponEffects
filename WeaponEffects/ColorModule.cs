using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class ColorModule : ItemModule
    {
        public float duration;
        public Vector4 colorRGBA;


        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.OnCollisionEvent += Item_OnCollisionEvent;
        }


        private void Item_OnCollisionEvent(ref CollisionStruct collisionInstance)
        {
            if (collisionInstance.targetCollider.GetComponentInParent<Creature>())
            {
                if (collisionInstance.damageStruct.damage > 1)
                {
                    Creature creature = collisionInstance.targetCollider.GetComponentInParent<Creature>();
                    if (creature != Creature.player && !creature.health.isKilled)
                    {

                        if (!creature.gameObject.GetComponent<ColorChanger>())
                        {
                            ColorChanger colorChanger = creature.gameObject.AddComponent<ColorChanger>();
                            colorChanger.duration = duration;
                            colorChanger.color = new Color(colorRGBA.x, colorRGBA.y, colorRGBA.z, colorRGBA.w);

                            colorChanger.Run();
                        }
                    }
                }
            }
        }

    }
}
