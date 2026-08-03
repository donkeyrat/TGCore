using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class RemoveHealthBar : MonoBehaviour
{
    private void Start()
    {
        var unit = transform.root.GetComponent<Unit>();
        unit.SetHealthBarActive(false);
    }
}