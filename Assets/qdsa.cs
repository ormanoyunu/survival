
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro
{
    public class qdsa : MonoBehaviour
    {
        HealthManager adsada;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                adsada.ReceiveDamage(new DamageInfo(20, DamageType.Cut));
            }
        }
    }
}

