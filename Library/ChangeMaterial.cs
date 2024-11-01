using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class ChangeMaterial : MonoBehaviour
    {
        public void Start()
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                OriginalMaterials.Add(Instantiate(mat));
            }

            var unit = transform.root.GetComponent<Unit>();
            var ownTeamHolder = GetComponent<TeamHolder>();
            var parentTeamHolder = GetComponentInParent<TeamHolder>();
            var childTeamHolder = GetComponentInChildren<TeamHolder>();

            if (unit) Team = unit.Team;
            else if (ownTeamHolder) Team = ownTeamHolder.team;
            else if (parentTeamHolder) Team = parentTeamHolder.team;
            else if (childTeamHolder) Team = childTeamHolder.team;
        }

        public void MaterialChange(float speed)
        {
            hasChanged = true;
            StartCoroutine(DoMaterialChange(index, Instantiate(renderer.materials[index]), newMaterial, speed));
        }

        public void MaterialRevert(float speed)
        {
            if (!hasChanged) return;

            StopAllCoroutines();

            StartCoroutine(DoMaterialChange(index, newMaterial, OriginalMaterials[index], speed));
        }

        public void MaterialTeamChange(float speed)
        {
            StopAllCoroutines();

            StartCoroutine(DoMaterialChange(index, Instantiate(renderer.materials[index]),
                Team == Team.Red ? redMaterial : blueMaterial, speed));
        }

        public void MaterialTeamRevert(float speed)
        {
            StopAllCoroutines();

            StartCoroutine(DoMaterialChange(index, Team == Team.Red ? redMaterial : blueMaterial,
                OriginalMaterials[index], speed));
        }

        public void AllMaterialsChange(float speed)
        {
            StopAllCoroutines();

            for (var i = 0; i < renderer.materials.Length; i++)
            {
                StartCoroutine(DoMaterialChange(i, Instantiate(renderer.materials[i]), newMaterial, speed));
            }
        }

        public void AllMaterialsRevert(float speed)
        {
            StopAllCoroutines();

            for (var i = 0; i < renderer.materials.Length; i++)
            {
                StartCoroutine(DoMaterialChange(i, newMaterial, OriginalMaterials[i], speed));
            }
        }

        public void AllMaterialsTeamChange(float speed)
        {
            StopAllCoroutines();

            for (var i = 0; i < renderer.materials.Length; i++)
            {
                StartCoroutine(DoMaterialChange(i, Instantiate(renderer.materials[i]),
                    Team == Team.Red ? redMaterial : blueMaterial, speed));
            }
        }

        public void AllMaterialsTeamRevert(float speed)
        {
            StopAllCoroutines();

            for (var i = 0; i < renderer.materials.Length; i++)
            {
                StartCoroutine(DoMaterialChange(i, Team == Team.Red ? redMaterial : blueMaterial, OriginalMaterials[i],
                    speed));
            }
        }

        public void Stop()
        {
            StopAllCoroutines();
        }

        private IEnumerator DoMaterialChange(int indexToLerp, Material materialToStartWith, Material matToLerpTo,
            float lerpSpeed)
        {
            var t = 0f;
            while (t < 1f && !transform.root.GetComponentsInChildren<UnitEffectBase>().ToList()
                       .Find(x => x.effectID == 1987))
            {
                t += Time.deltaTime * lerpSpeed;

                renderer.materials[indexToLerp].Lerp(materialToStartWith, matToLerpTo, Mathf.Clamp(t, 0f, 1f));
                yield return null;
            }
        }

        private List<Material> OriginalMaterials = new();
        private Team Team;
        private bool hasChanged;

        [Header("Material Settings")] public Renderer renderer;
        public int index;
        public Material newMaterial;

        [Header("Team Color Settings")] public Material redMaterial;
        public Material blueMaterial;
    }
}