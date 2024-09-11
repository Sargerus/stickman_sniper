using System;
using UnityEngine;

[CreateAssetMenu(menuName = "[CUSTOMIZATION]Customization/WeaponCharacteristics", fileName = "new WeaponCharacteristics")]
public class WeaponCharacteristicsScriptable : ScriptableObject
{
    [Title(label: "Settings")]

    [Tooltip("Weapon Name. Currently not used for anything, but in the future, we will use this for pickups!")]
    public string weaponName;

    [Tooltip("How much the character's movement speed is multiplied by when wielding this weapon.")]
    public float multiplierMovementSpeed = 1.0f;

    [Title(label: "Firing")]

    [Tooltip("Is this weapon automatic? If yes, then holding down the firing button will continuously fire.")]
    public bool automatic;

    [Tooltip("Is this weapon bolt-action? If yes, then a bolt-action animation will play after every shot.")]
    public bool boltAction;

    [Tooltip("Amount of shots fired at once. Helpful for things like shotguns, where there are multiple projectiles fired at once.")]
    public int shotCount = 1;

    [Tooltip("How far the weapon can fire from the center of the screen.")]
    public Vector2 spread;

    [Tooltip("How fast the projectiles are.")]
    [SerializeField]
    public float projectileImpulse = 400.0f;

    [Tooltip("Amount of shots this weapon can shoot in a minute. It determines how fast the weapon shoots.")]
    public int roundsPerMinutes = 200;

    [Title(label: "Reloading")]

    [Tooltip("Determines if this weapon reloads in cycles, meaning that it inserts one bullet at a time, or not.")]
    public bool cycledReload;

    [Tooltip("Determines if the player can reload this weapon when it is full of ammunition.")]
    public bool canReloadWhenFull = true;

    [Tooltip("Should this weapon be reloaded automatically after firing its last shot?")]
    public bool automaticReloadOnEmpty;

    [Tooltip("Time after the last shot at which a reload will automatically start.")]
    public float automaticReloadOnEmptyDelay = 0.25f;

    [Title(label: "Animation")]

    [Tooltip("Transform that represents the weapon's ejection port, meaning the part of the weapon that casings shoot from.")]
    public Transform socketEjection;

    [Tooltip("Settings this to false will stop the weapon from being reloaded while the character is aiming it.")]
    public bool canReloadAimed = true;

    [Title(label: "Resources")]

    [Tooltip("Casing Prefab.")]
    public GameObject prefabCasing;

    [Tooltip("Projectile Prefab. This is the prefab spawned when the weapon shoots.")]
    public GameObject prefabProjectile;

    [Tooltip("The AnimatorController a player character needs to use while wielding this weapon.")]
    public RuntimeAnimatorController controller;

    [Tooltip("Weapon Body Texture.")]
    public Sprite spriteBody;

    [Title(label: "Audio Clips Holster")]

    [Tooltip("Holster Audio Clip.")]
    public AudioClip audioClipHolster;

    [Tooltip("Unholster Audio Clip.")]
    public AudioClip audioClipUnholster;

    [Title(label: "Audio Clips Reloads")]

    [Tooltip("Reload Audio Clip.")]
    public AudioClip audioClipReload;

    [Tooltip("Reload Empty Audio Clip.")]
    public AudioClip audioClipReloadEmpty;

    [Title(label: "Audio Clips Reloads Cycled")]

    [Tooltip("Reload Open Audio Clip.")]
    public AudioClip audioClipReloadOpen;

    [Tooltip("Reload Insert Audio Clip.")]
    public AudioClip audioClipReloadInsert;

    [Tooltip("Reload Close Audio Clip.")]
    public AudioClip audioClipReloadClose;

    [Title(label: "Audio Clips Other")]

    [Tooltip("AudioClip played when this weapon is fired without any ammunition.")]
    public AudioClip audioClipFireEmpty;

    [Tooltip("")]
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
        CustomizationIndexes.skinIndex = wcs.CustomizationIndexes.skinIndex;
    }
#endif
}

[Serializable]
public class CustomizationIndexes
{
    public CustomizationIndexes()
    {
        
    }

    public CustomizationIndexes(CustomizationIndexes source)
    {
        scopeDefaultShow = source.scopeDefaultShow;
        scopeIndex = source.scopeIndex;
        muzzleIndex = source.muzzleIndex;
        laserIndex = source.laserIndex;
        gripIndex = source.gripIndex;
        magazineIndex = source.magazineIndex;
        skinIndex = source.skinIndex;
    }

    [Title(label: "Scope")]

    [Tooltip("Determines if the ironsights should be shown on the weapon model.")]
    public bool scopeDefaultShow = true;

    [Tooltip("Selected Scope Index. If you set this to a negative number, ironsights will be selected as the enabled scope.")]
    public int scopeIndex = -1;

    [Title(label: "Muzzle")]

    [Tooltip("Selected Muzzle Index.")]
    public int muzzleIndex = -1;

    [Title(label: "Laser")]

    [Tooltip("Selected Laser Index.")]
    public int laserIndex = -1;

    [Title(label: "Grip")]

    [Tooltip("Selected Grip Index.")]
    public int gripIndex = -1;

    [Title(label: "Magazine")]

    [Tooltip("Selected Magazine Index.")]
    public int magazineIndex = -1;

    public int skinIndex = 0;

    public int GetIndex(AttachmentsTab tab) => tab switch
    {
        AttachmentsTab.Scope => scopeIndex,
        AttachmentsTab.Muzzle => muzzleIndex,
        AttachmentsTab.Laser => laserIndex,
        AttachmentsTab.Grip => gripIndex,
        AttachmentsTab.Magazine => magazineIndex,
        AttachmentsTab.Skin => skinIndex,
        _ => -1
    };

    internal void SetIndex(AttachmentsTab currentTab, int value)
    {
        switch (currentTab)
        {
            case AttachmentsTab.Scope: scopeIndex = value; break;
            case AttachmentsTab.Muzzle: muzzleIndex = value; break;
            case AttachmentsTab.Laser: laserIndex = value; break;
            case AttachmentsTab.Grip: gripIndex = value; break;
            case AttachmentsTab.Magazine: magazineIndex = value; break;
            case AttachmentsTab.Skin: skinIndex = value; break;
        }
    }
}