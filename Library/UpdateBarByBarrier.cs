using UnityEngine;
using UnityEngine.UI;

namespace TGCore.Library;

public class UpdateBarByBarrier : MonoBehaviour
{
    private CameraAbilityPossess Possess;
    private Effect_Shield Shield;
    private AchillesArmor.UnitIsArmored Armor;
    
    public Image image;

    private void Start()
    {
        Possess = MainCam.instance.GetComponentInParent<CameraAbilityPossess>();
    }

    private void Update()
    {
        if (Possess.currentUnit)
        {
            Shield = Possess.currentUnit.GetComponentInChildren<Effect_Shield>();
            Armor = Possess.currentUnit.GetComponent<AchillesArmor.UnitIsArmored>();

            if (Armor && Shield)
            {
                image.fillAmount = Mathf.Clamp((Shield.currentShield + Armor.armorHealth) / (Shield.maxShield + Armor.startingArmorHealth), 0f, 1f);
            }
            else if (Armor)
            {
                image.fillAmount = Mathf.Clamp(Armor.armorHealth / Armor.startingArmorHealth, 0f, 1f);
            }
            else if (Shield)
            {
                image.fillAmount = Mathf.Clamp(Shield.currentShield / Shield.maxShield, 0f, 1f);
            }
        }
        else
        {
            image.fillAmount = 0f;
        }
    }
}