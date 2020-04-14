using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;

namespace WeaponEffects
{
    class FlightModule : ItemModule
    {
        public float minForce;
        public float maxForce;



        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            Flight flight = item.gameObject.AddComponent<Flight>();
            flight.item = item;
            flight.minForce = minForce;
            flight.maxForce = maxForce;
            flight.Initialize();
        }



    }
}
