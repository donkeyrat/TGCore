using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class RandomSpellTarget : MonoBehaviour
{
    private SpellTarget SpellTarget;
    private TeamHolder TeamHolder;
    
    public float targetingRadius = 50f;
    public bool setTargetOnStart = true;
    
    private void Start()
    {
        TeamHolder = GetComponent<TeamHolder>();
        SpellTarget = GetComponent<SpellTarget>();
        if (setTargetOnStart) SetTarget();
    }
    
    public void SetTarget()
    {
        var hits = Physics.SphereCastAll(transform.position, targetingRadius, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
        var foundUnits = hits
            .Select(hit => hit.transform.root.GetComponent<Unit>())
            .Where(x => x && !x.data.Dead && (!TeamHolder || x.Team != TeamHolder.team))
            .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
            .Distinct()
            .ToArray();
        if (foundUnits.Length > 0)
        {
            var selectedUnit = foundUnits[Random.Range(0, foundUnits.Length - 1)];
            SpellTarget.rig = selectedUnit.data.mainRig;
            SpellTarget.distance = Vector3.Distance(transform.position, selectedUnit.transform.position);
        }
    }
}