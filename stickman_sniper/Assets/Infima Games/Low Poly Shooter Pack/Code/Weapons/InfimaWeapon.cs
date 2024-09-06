//Copyright 2022, Infima Games. All Rights Reserved.

using Customization;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Weapon. This class handles most of the things that weapons need.
    /// </summary>
    public class InfimaWeapon : WeaponBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "Settings")]

        [Tooltip("Weapon Name. Currently not used for anything, but in the future, we will use this for pickups!")]
        [SerializeField]
        protected string weaponName;

        [Tooltip("How much the character's movement speed is multiplied by when wielding this weapon.")]
        [SerializeField]
        protected float multiplierMovementSpeed = 1.0f;

        [Title(label: "Firing")]

        [Tooltip("Is this weapon automatic? If yes, then holding down the firing button will continuously fire.")]
        [SerializeField]
        protected bool automatic;

        [Tooltip("Is this weapon bolt-action? If yes, then a bolt-action animation will play after every shot.")]
        [SerializeField]
        protected bool boltAction;

        [Tooltip("Amount of shots fired at once. Helpful for things like shotguns, where there are multiple projectiles fired at once.")]
        [SerializeField]
        protected int shotCount = 1;

        [Tooltip("How far the weapon can fire from the center of the screen.")]
        [SerializeField]
        protected Vector2 spread;

        [Tooltip("How fast the projectiles are.")]
        [SerializeField]
        protected float projectileImpulse = 400.0f;

        [Tooltip("Amount of shots this weapon can shoot in a minute. It determines how fast the weapon shoots.")]
        [SerializeField]
        protected int roundsPerMinutes = 200;

        [Title(label: "Reloading")]

        [Tooltip("Determines if this weapon reloads in cycles, meaning that it inserts one bullet at a time, or not.")]
        [SerializeField]
        protected bool cycledReload;

        [Tooltip("Determines if the player can reload this weapon when it is full of ammunition.")]
        [SerializeField]
        protected bool canReloadWhenFull = true;

        [Tooltip("Should this weapon be reloaded automatically after firing its last shot?")]
        [SerializeField]
        protected bool automaticReloadOnEmpty;

        [Tooltip("Time after the last shot at which a reload will automatically start.")]
        [SerializeField]
        protected float automaticReloadOnEmptyDelay = 0.25f;

        [Title(label: "Animation")]

        [Tooltip("Transform that represents the weapon's ejection port, meaning the part of the weapon that casings shoot from.")]
        [SerializeField]
        protected Transform socketEjection;

        [Tooltip("Settings this to false will stop the weapon from being reloaded while the character is aiming it.")]
        [SerializeField]
        protected bool canReloadAimed = true;

        [Title(label: "Resources")]

        [Tooltip("Casing Prefab.")]
        [SerializeField]
        protected GameObject prefabCasing;

        [Tooltip("Projectile Prefab. This is the prefab spawned when the weapon shoots.")]
        [SerializeField]
        protected GameObject prefabProjectile;

        [Tooltip("The AnimatorController a player character needs to use while wielding this weapon.")]
        [SerializeField]
        protected RuntimeAnimatorController controller;

        [Tooltip("Weapon Body Texture.")]
        [SerializeField]
        protected Sprite spriteBody;

        [Title(label: "Audio Clips Holster")]

        [Tooltip("Holster Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipHolster;

        [Tooltip("Unholster Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipUnholster;

        [Title(label: "Audio Clips Reloads")]

        [Tooltip("Reload Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipReload;

        [Tooltip("Reload Empty Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipReloadEmpty;

        [Title(label: "Audio Clips Reloads Cycled")]

        [Tooltip("Reload Open Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipReloadOpen;

        [Tooltip("Reload Insert Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipReloadInsert;

        [Tooltip("Reload Close Audio Clip.")]
        [SerializeField]
        protected AudioClip audioClipReloadClose;

        [Title(label: "Audio Clips Other")]

        [Tooltip("AudioClip played when this weapon is fired without any ammunition.")]
        [SerializeField]
        protected AudioClip audioClipFireEmpty;

        [Tooltip("")]
        [SerializeField]
        protected AudioClip audioClipBoltAction;

        protected Subject<Unit> _onFire = new();
        public System.IObservable<Unit> OnFire => _onFire; 

        #endregion

        #region FIELDS

        /// <summary>
        /// Weapon Animator.
        /// </summary>
        protected Animator animator;
        /// <summary>
        /// Attachment Manager.
        /// </summary>
        protected WeaponAttachmentManagerBehaviour attachmentManager;

        /// <summary>
        /// Amount of ammunition left.
        /// </summary>
        protected int ammunitionCurrent; //in magazine
        protected int ammunitionInStock; //left spare
        protected int ammunitionMax; //total max spare
        public override bool CanReloadEnoughSpareBullets() => ammunitionCurrent < magazineBehaviour.GetMagazineSize() 
            && ammunitionInStock > 0;

        public override int GetAmmunitionSpareLeft() => ammunitionInStock;

        #region Attachment Behaviours

        /// <summary>
        /// Equipped scope Reference.
        /// </summary>
        protected ScopeBehaviour scopeBehaviour;

        /// <summary>
        /// Equipped Magazine Reference.
        /// </summary>
        protected MagazineBehaviour magazineBehaviour;
        /// <summary>
        /// Equipped Muzzle Reference.
        /// </summary>
        protected MuzzleBehaviour muzzleBehaviour;

        /// <summary>
        /// Equipped Laser Reference.
        /// </summary>
        protected LaserBehaviour laserBehaviour;
        /// <summary>
        /// Equipped Grip Reference.
        /// </summary>
        protected GripBehaviour gripBehaviour;

        #endregion

        /// <summary>
        /// The GameModeService used in this game!
        /// </summary>
        protected IGameModeService gameModeService;
        /// <summary>
        /// The main player character behaviour component.
        /// </summary>
        protected CharacterBehaviour characterBehaviour;

        /// <summary>
        /// The player character's camera.
        /// </summary>
        protected Transform playerCamera;

        #endregion

        #region UNITY

        protected override void Awake()
        {
            //Get Animator.
            animator = GetComponent<Animator>();
            //Get Attachment Manager.
            attachmentManager = GetComponent<WeaponAttachmentManagerBehaviour>();

            //Cache the game mode service. We only need this right here, but we'll cache it in case we ever need it again.
            gameModeService = ServiceLocator.Current.Get<IGameModeService>();
            //Cache the player character.
            characterBehaviour = gameModeService.GetPlayerCharacter();
            //Cache the world camera. We use this in line traces.
            playerCamera = characterBehaviour.GetCameraWorld().transform;
        }

        #endregion

        public void Initialize()
        {
            //Get Scope.
            scopeBehaviour = attachmentManager.GetEquippedScope();
            //Get Magazine.
            magazineBehaviour = attachmentManager.GetEquippedMagazine();
            //Get Muzzle.
            muzzleBehaviour = attachmentManager.GetEquippedMuzzle();
            //Get Laser.
            laserBehaviour = attachmentManager.GetEquippedLaser();
            //Get Grip.
            gripBehaviour = attachmentManager.GetEquippedGrip();
            //Max Out Ammo.
            ammunitionMax = magazineBehaviour.GetMagazineSpareCapcity();
            ammunitionInStock = ammunitionMax;
            ammunitionCurrent = magazineBehaviour.GetMagazineSize();
        }

#if UNITY_EDITOR

        [Sirenix.OdinInspector.Button]
        private void FullfillScriptable(WeaponCharacteristicsScriptable wcs)
        {
            wcs.weaponName = weaponName;
            wcs.multiplierMovementSpeed = multiplierMovementSpeed;
            wcs.automatic = automatic;
            wcs.boltAction = boltAction;
            wcs.shotCount = shotCount;
            wcs.spread = spread;
            wcs.projectileImpulse = projectileImpulse;
            wcs.roundsPerMinutes = roundsPerMinutes;
            wcs.cycledReload = cycledReload;
            wcs.canReloadWhenFull = canReloadWhenFull;
            wcs.automaticReloadOnEmpty = automaticReloadOnEmpty;
            wcs.automaticReloadOnEmptyDelay = automaticReloadOnEmptyDelay;
            wcs.socketEjection = socketEjection;
            wcs.canReloadAimed = canReloadAimed;
            wcs.prefabCasing = prefabCasing;
            wcs.prefabProjectile = prefabProjectile;
            wcs.controller = controller;
            wcs.spriteBody = spriteBody;
            wcs.audioClipHolster = audioClipHolster;
            wcs.audioClipUnholster = audioClipUnholster;
            wcs.audioClipReload = audioClipReload;
            wcs.audioClipReloadEmpty = audioClipReloadEmpty;
            wcs.audioClipReloadOpen = audioClipReloadOpen;
            wcs.audioClipReloadInsert = audioClipReloadInsert;
            wcs.audioClipReloadClose = audioClipReloadClose;
            wcs.audioClipFireEmpty = audioClipFireEmpty;
            wcs.audioClipBoltAction = audioClipBoltAction;
        }

#endif

        #region GETTERS

        /// <summary>
        /// GetFieldOfViewMultiplierAim.
        /// </summary>
        public override float GetFieldOfViewMultiplierAim()
        {
            //Make sure we don't have any issues even with a broken setup!
            if (scopeBehaviour != null)
                return scopeBehaviour.GetFieldOfViewMultiplierAim();

            //Error.
            Debug.LogError("Weapon has no scope equipped!");

            //Return.
            return 1.0f;
        }
        /// <summary>
        /// GetFieldOfViewMultiplierAimWeapon.
        /// </summary>
        public override float GetFieldOfViewMultiplierAimWeapon()
        {
            //Make sure we don't have any issues even with a broken setup!
            if (scopeBehaviour != null)
                return scopeBehaviour.GetFieldOfViewMultiplierAimWeapon();

            //Error.
            Debug.LogError("Weapon has no scope equipped!");

            //Return.
            return 1.0f;
        }

        /// <summary>
        /// GetAnimator.
        /// </summary>
        public override Animator GetAnimator() => animator;
        /// <summary>
        /// CanReloadAimed.
        /// </summary>
        public override bool CanReloadAimed() => canReloadAimed;

        /// <summary>
        /// GetSpriteBody.
        /// </summary>
        public override Sprite GetSpriteBody() => spriteBody;
        /// <summary>
        /// GetMultiplierMovementSpeed.
        /// </summary>
        public override float GetMultiplierMovementSpeed() => multiplierMovementSpeed;

        /// <summary>
        /// GetAudioClipHolster.
        /// </summary>
        public override AudioClip GetAudioClipHolster() => audioClipHolster;
        /// <summary>
        /// GetAudioClipUnholster.
        /// </summary>
        public override AudioClip GetAudioClipUnholster() => audioClipUnholster;

        /// <summary>
        /// GetAudioClipReload.
        /// </summary>
        public override AudioClip GetAudioClipReload() => audioClipReload;
        /// <summary>
        /// GetAudioClipReloadEmpty.
        /// </summary>
        public override AudioClip GetAudioClipReloadEmpty() => audioClipReloadEmpty;

        /// <summary>
        /// GetAudioClipReloadOpen.
        /// </summary>
        public override AudioClip GetAudioClipReloadOpen() => audioClipReloadOpen;
        /// <summary>
        /// GetAudioClipReloadInsert.
        /// </summary>
        public override AudioClip GetAudioClipReloadInsert() => audioClipReloadInsert;
        /// <summary>
        /// GetAudioClipReloadClose.
        /// </summary>
        public override AudioClip GetAudioClipReloadClose() => audioClipReloadClose;

        /// <summary>
        /// GetAudioClipFireEmpty.
        /// </summary>
        public override AudioClip GetAudioClipFireEmpty() => audioClipFireEmpty;
        /// <summary>
        /// GetAudioClipBoltAction.
        /// </summary>
        public override AudioClip GetAudioClipBoltAction() => audioClipBoltAction;

        /// <summary>
        /// GetAudioClipFire.
        /// </summary>
        public override AudioClip GetAudioClipFire() => muzzleBehaviour.GetAudioClipFire();
        /// <summary>
        /// GetAmmunitionCurrent.
        /// </summary>
        public override int GetAmmunitionCurrent() => ammunitionCurrent;

        /// <summary>
        /// GetAmmunitionTotal.
        /// </summary>
        public override int GetAmmunitionTotal() => magazineBehaviour.GetMagazineSize();
        /// <summary>
        /// HasCycledReload.
        /// </summary>
        public override bool HasCycledReload() => cycledReload;

        /// <summary>
        /// IsAutomatic.
        /// </summary>
        public override bool IsAutomatic() => automatic;
        /// <summary>
        /// IsBoltAction.
        /// </summary>
        public override bool IsBoltAction() => boltAction;

        /// <summary>
        /// GetAutomaticallyReloadOnEmpty.
        /// </summary>
        public override bool GetAutomaticallyReloadOnEmpty() => automaticReloadOnEmpty;
        /// <summary>
        /// GetAutomaticallyReloadOnEmptyDelay.
        /// </summary>
        public override float GetAutomaticallyReloadOnEmptyDelay() => automaticReloadOnEmptyDelay;

        /// <summary>
        /// CanReloadWhenFull.
        /// </summary>
        public override bool CanReloadWhenFull() => canReloadWhenFull;
        /// <summary>
        /// GetRateOfFire.
        /// </summary>
        public override float GetRateOfFire() => roundsPerMinutes;

        /// <summary>
        /// IsFull.
        /// </summary>
        public override bool IsFull() => ammunitionCurrent == magazineBehaviour.GetMagazineSize();
        /// <summary>
        /// HasAmmunition.
        /// </summary>
        public override bool HasAmmunition() => ammunitionCurrent > 0;

        /// <summary>
        /// GetAnimatorController.
        /// </summary>
        public override RuntimeAnimatorController GetAnimatorController() => controller;
        /// <summary>
        /// GetAttachmentManager.
        /// </summary>
        public override WeaponAttachmentManagerBehaviour GetAttachmentManager() => attachmentManager;

        #endregion

        #region METHODS

        /// <summary>
        /// Reload.
        /// </summary>
        public override void Reload()
        {
            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            const string boolName = "Reloading";
            animator.SetBool(boolName, true);

            //Try Play Reload Sound.
            ServiceLocator.Current.Get<IAudioManagerService>().PlayOneShot(HasAmmunition() ? audioClipReload : audioClipReloadEmpty, new AudioSettings(1.0f, 0.0f, false));

            //Play Reload Animation.
            animator.Play(cycledReload ? "Reload Open" : (HasAmmunition() ? "Reload" : "Reload Empty"), 0, 0.0f);
        }
        /// <summary>
        /// Fire.
        /// </summary>
        public override async UniTask Fire(float spreadMultiplier = 1.0f)
        {
            //We need a muzzle in order to fire this weapon!
            if (muzzleBehaviour == null)
                return;

            //Make sure that we have a camera cached, otherwise we don't really have the ability to perform traces.
            if (playerCamera == null)
                return;

            //Play the firing animation.
            const string stateName = "Fire";
            animator.Play(stateName, 0, 0.0f);
            //Reduce ammunition! We just shot, so we need to get rid of one!
            ammunitionCurrent = Mathf.Clamp(ammunitionCurrent - 1, 0, magazineBehaviour.GetMagazineSize());

            //Set the slide back if we just ran out of ammunition.
            if (ammunitionCurrent == 0)
                SetSlideBack(1);

            //Play all muzzle effects.
            muzzleBehaviour.Effect();
            _onFire.OnNext(Unit.Default);

            //Spawn as many projectiles as we need.
            for (var i = 0; i < shotCount; i++)
            {
                //Determine a random spread value using all of our multipliers.
                Vector3 spreadValue = Random.insideUnitSphere * (spread * spreadMultiplier);
                //Remove the forward spread component, since locally this would go inside the object we're shooting!
                spreadValue.z = 0;
                //Convert to world space.
                spreadValue = playerCamera.TransformDirection(spreadValue);

                //Spawn projectile from the projectile spawn point.
                GameObject projectile = Instantiate(prefabProjectile, playerCamera.position, Quaternion.Euler(playerCamera.eulerAngles + spreadValue));
                //Add velocity to the projectile.
                projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileImpulse;
            }
        }

        /// <summary>
        /// FillAmmunition.
        /// </summary>
        public override void FillAmmunition(int amount)
        {
            //Update the value by a certain amount.
            ammunitionCurrent = amount != 0 ? Mathf.Clamp(ammunitionCurrent + amount,
                0, GetAmmunitionTotal()) : magazineBehaviour.GetMagazineSize();

           ammunitionInStock = Mathf.Clamp(ammunitionInStock - amount, 0, ammunitionMax);
        }
        /// <summary>
        /// SetSlideBack.
        /// </summary>
        public override void SetSlideBack(int back)
        {
            //Set the slide back bool.
            const string boolName = "Slide Back";
            animator.SetBool(boolName, back != 0);
        }

        /// <summary>
        /// EjectCasing.
        /// </summary>
        public override void EjectCasing()
        {
            //Spawn casing prefab at spawn point.
            if (prefabCasing != null && socketEjection != null)
                Instantiate(prefabCasing, socketEjection.position, socketEjection.rotation);
        }

        public virtual void SetCustomization(WeaponCharacteristicsScriptable wcs)
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

            attachmentManager.SetIndexes(wcs.CustomizationIndexes);
            attachmentManager.Initialize();
        }

        #endregion
    }
}