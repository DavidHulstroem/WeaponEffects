using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class Gravity : MonoBehaviour
    {
        public Item item;

        private void LateUpdate()
        {
            if (item != null)
            {
                item.rb.useGravity = false;
            }
        } 
    }
}
