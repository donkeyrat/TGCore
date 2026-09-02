using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class GiveBuffsToAllies : MonoBehaviour
{
    private int BuffIndex;

    private List<GameObject> CurrentBuffs = new List<GameObject>();
    private TeamSystem TeamSystem;
    private Unit OwnUnit;
    public GameObject globalBuff;
    public GameObject buffOne;
    public GameObject buffTwo;
    public GameObject buffThree;
    public UnityEvent buffOneEvent;
    public UnityEvent buffTwoEvent;
    public UnityEvent buffThreeEvent;
    public float delayPerBuff = 0.02f;
    public float buffTime = 10f;
    
    private void Start()
    {
        TeamSystem = World.Active.GetOrCreateManager<TeamSystem>();
        OwnUnit = transform.root.GetComponent<Unit>();
    }

    public void DetermineCurrentBuffs()
    {
        CurrentBuffs.Clear();
        if (BuffIndex == 0 || BuffIndex == 2)
        {
            CurrentBuffs.Add(buffOne);
        }
        if (BuffIndex == 0 || BuffIndex == 1)
        {
            CurrentBuffs.Add(buffTwo);
        }
        if (BuffIndex == 1 || BuffIndex == 2)
        {
            CurrentBuffs.Add(buffThree);
        }
        
        BuffIndex++;
        if (BuffIndex > 2) BuffIndex = 0;
    }

    public void TriggerBuffEvent(int index)
    {
        if (CurrentBuffs[index] == buffOne)
        {
            buffOneEvent.Invoke();
        }
        else if (CurrentBuffs[index] == buffTwo)
        {
            buffTwoEvent.Invoke();
        }
        else if (CurrentBuffs[index] == buffThree)
        {
            buffThreeEvent.Invoke();
        }
    }
    
    public void GiveBuffs()
    {
        StartCoroutine(DoBuffs(CurrentBuffs.ToArray()));
    }
    
    public void GiveBuff(int index)
    {
        StartCoroutine(DoBuff(CurrentBuffs[index]));
    }

    private IEnumerator DoBuffs(GameObject[] currentBuffs)
    {
        var teamUnits = new List<Unit>(TeamSystem.GetTeamUnits(OwnUnit.Team));
        teamUnits.Remove(OwnUnit);

        foreach (var unit in teamUnits.Where(unit => unit && !unit.data.Dead && !unit.data.mainRig.isKinematic && !unit.GetComponent<UnitIsBuffed>()))
        {
            unit.gameObject.AddComponent<UnitIsBuffed>().buffTime = buffTime;
            Instantiate(globalBuff, unit.transform.position, unit.transform.rotation, unit.transform);
            
            foreach (var buff in currentBuffs)
            {
                var spawnedBuff = Instantiate(buff, unit.transform.position, unit.transform.rotation, unit.transform);
                
                var propItem = spawnedBuff.GetComponentInChildren<PropItem>();
                if (propItem && unit.RigType == Stitcher.TransformCatalog.RigType.Human && !unit.GetComponent<IceGiant>())
                    propItem.Equip(unit.gameObject, 
                        new PropItemData(),
                        new Stitcher.TransformCatalog(unit.gameObject, Stitcher.TransformCatalog.RigType.Human, "M_"),
                        unit.Team);
                else if (propItem)
                {
                    Destroy(propItem.gameObject);
                }
            }

            yield return new WaitForSeconds(delayPerBuff);
        }
    }
    private IEnumerator DoBuff(GameObject currentBuff)
    {
        var teamUnits = new List<Unit>(TeamSystem.GetTeamUnits(OwnUnit.Team));
        teamUnits.Remove(OwnUnit);

        foreach (var unit in teamUnits.Where(unit => unit && !unit.data.Dead && !unit.data.mainRig.isKinematic && unit.GetComponents<UnitIsBuffed>().Length < 2))
        {
            unit.gameObject.AddComponent<UnitIsBuffed>().buffTime = buffTime;
            Instantiate(globalBuff, unit.transform.position, unit.transform.rotation, unit.transform);
            
            var spawnedBuff = Instantiate(currentBuff, unit.transform.position, unit.transform.rotation, unit.transform);
                
            var propItem = spawnedBuff.GetComponentInChildren<PropItem>();
            if (propItem && unit.RigType == Stitcher.TransformCatalog.RigType.Human)
                propItem.Equip(unit.gameObject, 
                    new PropItemData(),
                    new Stitcher.TransformCatalog(unit.gameObject, Stitcher.TransformCatalog.RigType.Human, "M_"),
                    unit.Team);
            else if (propItem)
            {
                Destroy(propItem.gameObject);
            }

            yield return new WaitForSeconds(delayPerBuff);
        }
    }

    public class UnitIsBuffed : MonoBehaviour
    {
        private float Counter;

        private void Update()
        {
            Counter += Time.deltaTime;
            if (Counter > buffTime)
            {
                Destroy(this);
            }
        }
        
        public float buffTime;
    }
}