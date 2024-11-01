using UnityEngine;
using System.Collections.Generic;

namespace TGCore.Library
{
    public class UnitPartBreaking : MonoBehaviour
    {
        private void Start()
        {
            OwnData = GetComponent<DataHandler>();
            OwnData.unit.WasDealtDamageAction += BreakPart;
        }

        private void Update() 
        {
            Counter += Time.deltaTime;
        }

        public void BreakPart(float damage)
        {
            if (breakableParts.Count <= 0 || !(OwnData.health <= OwnData.maxHealth * percentHealthRequirement) ||
                !(Random.value < breakChance) || !(Counter >= cooldown) || !(damage >= damageThreshold)) return;
            
            Counter = 0f;
                    
            var selectedPart = breakableParts[Random.Range(0, breakableParts.Count - 1)];
            selectedPart.AddComponent<Rigidbody>().mass = selectedPart.GetComponentInParent<Rigidbody>().mass;
            selectedPart.transform.SetParent(transform.root);

            var delay = selectedPart.GetComponent<DelayEvent>();
            if (delay) delay.Go();

            breakableParts.Remove(selectedPart);
        }
        
        private DataHandler OwnData;
        private float Counter;

        public List<GameObject> breakableParts;
        
        [Range(0f, 1f)]
        public float percentHealthRequirement = 0.5f;

        public float damageThreshold = 100f;

        public float breakChance = 0.5f;

        public float cooldown = 3f;
    }
}