using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class Effect_Lightning : UnitEffectBase
{
    private List<Unit> HitList = new List<Unit>();
    private Team OwnTeam;

    public float lightningDamage = 100f;
    public float maxTargetRange = 6f;
    public int chainLength = 5;
    public int chains = 2;
    public GameObject lineObject;
    
    public override void DoEffect()
    {
        var rootUnit = transform.root.GetComponent<Unit>();
        if (!rootUnit)
        {
            Destroy(gameObject);
            return;
        }
        OwnTeam = rootUnit.Team;
        HitList.Add(rootUnit);
        rootUnit.data.healthHandler.TakeDamage(lightningDamage, Vector3.zero);

        StartCoroutine(DoLightning(rootUnit));
    }

    public override void Ping()
    {
    }
    
    private IEnumerator DoLightning(Unit startingUnit)
    {
        var sourceUnitsLists = new List<Unit[]>();
        for (var a = 0; a < chainLength; a++)
        {
            sourceUnitsLists.Add(new Unit[chainLength]);
        }
        for (var i = 0; i < chainLength; i++)
        {
            var sourceUnits = sourceUnitsLists[i];
            var targetUnits = new Unit[chains];
            for (var j = 0; j < chains; j++)
            {
                if (i == 0 && startingUnit)
                {
                    sourceUnits[j] = startingUnit;
                }
                if (!sourceUnits[j])
                {
                    continue;
                }
                
                targetUnits[j] = SetTarget(sourceUnits[j].data.mainRig.position);
                
                if (targetUnits[j] && targetUnits[j].data && targetUnits[j].data.healthHandler && lineObject)
                {
                    var line = Instantiate(lineObject, sourceUnits[j].transform, true);
                    line.transform.FindChildRecursive("T1").position = sourceUnits[j].data.mainRig.position;
                    line.transform.FindChildRecursive("T2").position = targetUnits[j].data.mainRig.position;

                    targetUnits[j].data.healthHandler.TakeDamage(lightningDamage, Vector3.zero);

                    HitList.Add(targetUnits[j]);
                }

                if (i + 1 < chainLength)
                {
                    sourceUnitsLists[i + 1][j] = targetUnits[j];
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
            
        HitList.Clear();
    }

    private Unit SetTarget(Vector3 source)
    {
        var hits = Physics.SphereCastAll(source, maxTargetRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
        var foundUnits = hits
            .Select(hit => hit.transform.root.GetComponent<Unit>())
            .Where(x => x && !x.data.Dead && x.Team == OwnTeam && !HitList.Contains(x))
            .OrderBy(x => (x.data.mainRig.transform.position - source).magnitude)
            .Distinct()
            .ToArray();
        return foundUnits.Length > 0 ? foundUnits[0] : null;
    }
}