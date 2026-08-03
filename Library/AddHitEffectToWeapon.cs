using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TGCore.Library;

public class AddHitEffectToWeapon : MonoBehaviour
{
    private Weapon[] Weapons;
    private SpookySwords[] SpookySwords;
    private static GameObject Pool;
    private Dictionary<RangeWeapon, GameObject> WeaponToProjectileDict = new Dictionary<RangeWeapon, GameObject>();
    private Dictionary<MeleeWeapon, MeleeWeaponAddEffect> WeaponToEffectDict = new Dictionary<MeleeWeapon, MeleeWeaponAddEffect>();
    
    public UnitEffectBase effect;
    public bool goOnStart = true;

    private void Awake()
    {
        if (!Pool)
        {
            Pool = new GameObject("TG Projectile Pool")
            { 
                hideFlags = HideFlags.HideAndDontSave,
                transform =
                {
                    position = Vector3.down * -1000f
                }
            };
            Pool.SetActive(false);
        }
    }

    private void Start()
    {
        Weapons = transform.root.GetComponentsInChildren<Weapon>();
        SpookySwords = transform.root.GetComponentsInChildren<SpookySwords>();
        if (goOnStart) AddEffects();
    }

    public void AddEffects()
    {
        foreach (var weapon in Weapons)
        {
            switch (weapon)
            {
                case MeleeWeapon meleeWeapon:
                    var collisionWeapon = weapon.GetComponent<CollisionWeapon>();
                    if (collisionWeapon)
                    {
                        var meleeWeaponEffect = meleeWeapon.gameObject.AddComponent<MeleeWeaponAddEffect>();
                        meleeWeaponEffect.EffectPrefab = effect;
                        meleeWeaponEffect.ignoreTeamMates = true;
                        
                        var effectList = ((CollisionWeaponEffect[])GetField(collisionWeapon, "meleeWeaponEffects")).ToList();
                        effectList.Add(meleeWeaponEffect);
                        SetField(collisionWeapon, "meleeWeaponEffects", effectList.ToArray());
                        
                        WeaponToEffectDict.Add(meleeWeapon, meleeWeaponEffect);
                    }
                    break;
                case RangeWeapon rangeWeapon:
                {
                    var projectile = rangeWeapon.ObjectToSpawn;
                    var newProjectile = CloneAndPoolObject(projectile);
                    var pooled = newProjectile.GetComponent<PooledProjectile>();
                    if (pooled)
                    {
                        Destroy(pooled);
                    }
                    
                    var projHits = newProjectile.GetComponentsInChildren<ProjectileHit>();
                    var collisionWeapons = newProjectile.GetComponentsInChildren<CollisionWeapon>();
                    var explosions = newProjectile.GetComponentsInChildren<Explosion>();

                    foreach (var projectileHit in projHits)
                    {
                        var projectileEffect = projectileHit.gameObject.AddComponent<ProjectileHitAddEffect>();
                        projectileEffect.EffectPrefab = effect;

                        /*
                        if (projectileHit.objectsToSpawn.Length > 0)
                        {
                            foreach (var obj in projectileHit.objectsToSpawn)
                            {
                                obj.objectToSpawn = CloneAndPoolObject(obj.objectToSpawn);
                                var objExplosions = obj.objectToSpawn.GetComponentsInChildren<Explosion>();
                                foreach (var explosion in objExplosions)
                                {
                                    if (explosion.onlyTeamMates) continue;
                                    var explosionEffect = explosion.gameObject.AddComponent<AddObjectEffect>();
                                    explosionEffect.EffectPrefab = effect;
                                    explosionEffect.OnlyOnce = true;
                                }
                            }
                        }
                        */
                    }

                    foreach (var collision in collisionWeapons)
                    {
                        var meleeWeaponEffect = collision.gameObject.AddComponent<MeleeWeaponAddEffect>();
                        meleeWeaponEffect.EffectPrefab = effect;
                        meleeWeaponEffect.ignoreTeamMates = true;
                    }

                    foreach (var explosion in explosions)
                    {
                        if (explosion.onlyTeamMates) continue;
                        var explosionEffect = explosion.gameObject.AddComponent<AddObjectEffect>();
                        explosionEffect.EffectPrefab = effect;
                        explosionEffect.OnlyOnce = true;
                    }
                    
                    rangeWeapon.ObjectToSpawn = newProjectile;
                    if (!WeaponToProjectileDict.ContainsKey(rangeWeapon))
                    {
                        WeaponToProjectileDict.Add(rangeWeapon, projectile);
                    }
                    break;
                }
            }
        }

        foreach (var swords in SpookySwords)
        {
            swords.sourceSword = CloneAndPoolObject(swords.sourceSword);
            var projHits = swords.sourceSword.GetComponentsInChildren<ProjectileHit>();
            foreach (var projectileHit in projHits)
            {
                var projectileEffect = projectileHit.gameObject.AddComponent<ProjectileHitAddEffect>();
                projectileEffect.EffectPrefab = effect;

                if (projectileHit.objectsToSpawn.Length > 0)
                {
                    foreach (var obj in projectileHit.objectsToSpawn)
                    {
                        obj.objectToSpawn = CloneAndPoolObject(obj.objectToSpawn);
                        var objExplosions = obj.objectToSpawn.GetComponentsInChildren<Explosion>();
                        foreach (var explosion in objExplosions)
                        {
                            if (explosion.onlyTeamMates) continue;
                            var explosionEffect = explosion.gameObject.AddComponent<AddObjectEffect>();
                            explosionEffect.EffectPrefab = effect;
                            explosionEffect.OnlyOnce = true;
                        }
                    }
                }
            }

            var spawnedSwords = (List<SpookySword>)swords.GetField("swords");
            foreach (var sword in spawnedSwords.Where(x => x != null && x.gameObject))
            {
                var swordProjHits = sword.gameObject.GetComponentsInChildren<ProjectileHit>();
                foreach (var projectileHit in swordProjHits)
                {
                    var projectileEffect = projectileHit.gameObject.AddComponent<ProjectileHitAddEffect>();
                    projectileEffect.EffectPrefab = effect;
                    
                    var effectList = ((ProjectileHitEffect[])GetField(projectileHit, "hitEffects")).ToList();
                    effectList.Add(projectileEffect);
                    SetField(projectileHit, "hitEffects", effectList.ToArray());
                }
            }
        }
    }
    
    public void RemoveEffects()
    {
        foreach (var weapon in Weapons)
        {
            switch (weapon)
            {
                case MeleeWeapon meleeWeapon:
                    var collisionWeapon = weapon.GetComponent<CollisionWeapon>();
                    if (WeaponToEffectDict.TryGetValue(meleeWeapon, out var meleeWeaponEffect))
                    {
                        var effectList = ((CollisionWeaponEffect[])GetField(collisionWeapon, "meleeWeaponEffects")).ToList();
                        effectList.Remove(meleeWeaponEffect);
                        SetField(collisionWeapon, "meleeWeaponEffects", effectList.ToArray());
                    }
                    break;
                case RangeWeapon rangeWeapon:
                {
                    if (WeaponToProjectileDict.TryGetValue(rangeWeapon, out var projectile))
                    {
                        rangeWeapon.ObjectToSpawn = projectile;
                    }
                    break;
                }
            }
        }
    }

    private static void PoolObject(GameObject gameObject)
    {
        gameObject.transform.parent = Pool.transform;
        gameObject.transform.localPosition = Vector3.zero;
        SetHideFlagsChildren(Pool.transform, HideFlags.HideAndDontSave);
    }

    private static GameObject CloneAndPoolObject(GameObject obj)
    {
        var newObj = Instantiate(obj, Pool.transform);
        PoolObject(newObj);
        return newObj;
    }

    private static void SetHideFlagsChildren(Transform t, HideFlags hf = HideFlags.DontSave)
    {
        if (t.gameObject)
        {
            t.gameObject.hideFlags = hf;
        }
        if (t.childCount > 0)
        {
            for (var i = 0; i < t.childCount; i++)
            {
                SetHideFlagsChildren(t.GetChild(i), hf);
            }
        }
    }

    private static object GetField<T>(T self, string name) where T : class
    {
        FieldInfo field = typeof (T).GetField(name, ~BindingFlags.Default);
        return field != (FieldInfo) null ? field.GetValue((object) self) : (object) null;
    }

    private static T SetField<T>(T self, string name, object value) where T : class
    {
        FieldInfo field = typeof (T).GetField(name, ~BindingFlags.Default);
        if (field != (FieldInfo) null)
            field.SetValue((object) self, value);
        return self;
    }

}