using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;

namespace WeaponEffects
{
    class ColorChanger : MonoBehaviour
    {
        public float duration;
        public Color color;

        public void Run()
        {
            foreach (Material mat in GetComponent<Creature>().bodyMeshRenderer.materials)
            {
                StartCoroutine(SetColorOfMat(mat));
            }
            
        }

        IEnumerator SetColorOfMat(Material material)
        {
            Color defaultColor = material.color;

            material.SetColor("_BaseColor", color);
            yield return new WaitForSeconds(duration);
            material.SetColor("_BaseColor", defaultColor);

            Destroy(this);
        }
    }
}
