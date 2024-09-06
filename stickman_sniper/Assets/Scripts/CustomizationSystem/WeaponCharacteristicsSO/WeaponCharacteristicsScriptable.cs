using System;
using UnityEngine;

namespace Customization
{
    [CreateAssetMenu(menuName = "[CUSTOMIZATION]Customization/WeaponCharacteristics", fileName = "new WeaponCharacteristics")]
    public class WeaponCharacteristicsScriptable : ScriptableObject
    {
        public string weaponName;
        public float multiplierMovementSpeed = 1.0f;
        public bool automatic;
        public bool boltAction;
        public int shotCount = 1;
        public Vector2 spread;
        public float projectileImpulse = 400.0f;
        public int roundsPerMinutes = 200;
        public bool cycledReload;
        public bool canReloadWhenFull = true;
        public bool automaticReloadOnEmpty;
        public float automaticReloadOnEmptyDelay = 0.25f;
        public Transform socketEjection;
        public bool canReloadAimed = true;
        public GameObject prefabCasing;
        public GameObject prefabProjectile;
        public RuntimeAnimatorController controller;
        public Sprite spriteBody;
        public AudioClip audioClipHolster;
        public AudioClip audioClipUnholster;
        public AudioClip audioClipReload;
        public AudioClip audioClipReloadEmpty;
        public AudioClip audioClipReloadOpen;
        public AudioClip audioClipReloadInsert;
        public AudioClip audioClipReloadClose;
        public AudioClip audioClipFireEmpty;
        public AudioClip audioClipBoltAction;
        public LayerMask layerMask;
        public GameObject slowmotionBullet;
        public float damage;

        public CustomizationIndexes CustomizationIndexes;

#if UNITY_EDITOR
    public void CopyValues(WeaponCharacteristicsScriptable wcs)
    {
        weaponName = wcs.weaponName;
        multiplierMovementSpeed = wcs.multiplierMovementSpeed;
        automatic = wcs.automatic;
        boltAction = wcs.boltAction;
        shotCount = wcs.shotCount;
        spread = wcs.spread;
        projectileImpulse = wcs.projectileImpulse;
        roundsPerMinutes = wcs.roundsPerMinutes;
        cycledReload = wcs.cycledReload;
        canReloadWhenFull = wcs.canReloadWhenFull;
        automaticReloadOnEmpty = wcs.automaticReloadOnEmpty;
        automaticReloadOnEmptyDelay = wcs.automaticReloadOnEmptyDelay;
        socketEjection = wcs.socketEjection;
        canReloadAimed = wcs.canReloadAimed;
        prefabCasing = wcs.prefabCasing;
        prefabProjectile = wcs.prefabProjectile;
        controller = wcs.controller;
        spriteBody = wcs.spriteBody;
        audioClipHolster = wcs.audioClipHolster;
        audioClipUnholster = wcs.audioClipUnholster;
        audioClipReload = wcs.audioClipReload;
        audioClipReloadEmpty = wcs.audioClipReloadEmpty;
        audioClipReloadOpen = wcs.audioClipReloadOpen;
        audioClipReloadInsert = wcs.audioClipReloadInsert;
        audioClipReloadClose = wcs.audioClipReloadClose;
        audioClipFireEmpty = wcs.audioClipFireEmpty;
        audioClipBoltAction = wcs.audioClipBoltAction;
        layerMask = wcs.layerMask;
        slowmotionBullet = wcs.slowmotionBullet;
        damage = wcs.damage;

        CustomizationIndexes.scopeDefaultShow = wcs.CustomizationIndexes.scopeDefaultShow;
        CustomizationIndexes.scopeIndex = wcs.CustomizationIndexes.scopeIndex;
        CustomizationIndexes.muzzleIndex = wcs.CustomizationIndexes.muzzleIndex;
        CustomizationIndexes.laserIndex = wcs.CustomizationIndexes.laserIndex;
        CustomizationIndexes.gripIndex = wcs.CustomizationIndexes.gripIndex;
        CustomizationIndexes.magazineIndex = wcs.CustomizationIndexes.magazineIndex;
    }
#endif
    }

    public enum AttachmentsTab
    {
        None = 0,
        Scope = 1,
        Muzzle = 2,
        Laser = 3,
        Grip = 4,
        Magazine = 5
    }

    [Serializable]
    public class CustomizationIndexes
    { 
        public bool scopeDefaultShow = true;
        public int scopeIndex = -1;
        public int muzzleIndex = -1;
        public int laserIndex = -1;
        public int gripIndex = -1;
        public int magazineIndex = -1;

        public int GetIndex(AttachmentsTab tab) => tab switch
        {
            AttachmentsTab.Scope => scopeIndex,
            AttachmentsTab.Muzzle => muzzleIndex,
            AttachmentsTab.Laser => laserIndex,
            AttachmentsTab.Grip => gripIndex,
            AttachmentsTab.Magazine => magazineIndex,
            _ => -1
        };

        public void SetIndex(AttachmentsTab currentTab, int value)
        {
            switch (currentTab)
            {
                case AttachmentsTab.Scope: scopeIndex = value; break;
                case AttachmentsTab.Muzzle: muzzleIndex = value; break;
                case AttachmentsTab.Laser: laserIndex = value; break;
                case AttachmentsTab.Grip: gripIndex = value; break;
                case AttachmentsTab.Magazine: magazineIndex = value; break;
            }
        }
    }
}