using Customization;
using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using stickman_sniper.Producer;
using stickman_sniper.Weapon.Explosives;
using UnityEngine;
using Zenject;

public class InfimaSniperWeapon : InfimaWeapon
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject slowmotionBullet;
    [SerializeField] private Transform bulletStartPosition;
    [SerializeField] private float damage;
    [SerializeField] private float shootDistance;

    private ICoreProducer _coreProducer;

    [Inject]
    private void Construct(ICoreProducer coreProducer)
    {
        _coreProducer = coreProducer;
    }

    public override void SetCustomization(WeaponCharacteristicsScriptable wcs)
    {
        base.SetCustomization(wcs);

        layerMask = wcs.layerMask;
        slowmotionBullet = wcs.slowmotionBullet;
        damage = wcs.damage;
    }

    public override async UniTask Fire(float spreadMultiplier = 1)
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

        //Play all muzzle effects.
        muzzleBehaviour.Effect();
        _onFire.OnNext(UniRx.Unit.Default);

        //Spawn as many projectiles as we need.
        for (var i = 0; i < shotCount; i++)
        {
            //Reduce ammunition! We just shot, so we need to get rid of one!
            ammunitionCurrent = Mathf.Clamp(ammunitionCurrent - 1, 0, magazineBehaviour.GetMagazineSize());

            //Set the slide back if we just ran out of ammunition.
            if (ammunitionCurrent == 0)
                SetSlideBack(1);

            bool wasShot = false;
            //Determine a random spread value using all of our multipliers.
            Vector3 spreadValue = UnityEngine.Random.insideUnitSphere * (spread * spreadMultiplier);
            //Remove the forward spread component, since locally this would go inside the object we're shooting!
            spreadValue.z = 0;

            Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f + spreadValue.x, 0.5f + spreadValue.y, 0));
            if (Physics.Raycast(ray, out var hit, shootDistance, layerMask))
            {
                var enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    enemy.Damage(damage);

                    if (!enemy.TryGetStat(DWTools.RPG.CharacterStat.Health, out var stat))
                    {
                        return;
                    }

                    if (stat.Value > 0)
                    {
                        return;
                    }

                    Vector3 direction = (hit.point - playerCamera.position).normalized;
                    direction.y = 0.5f;

                    if (enemy.IsAlive.Value)
                    {
                        enemy.ActivateHpCanvas(false);

                        await _coreProducer.KillEnemyWeaponSlowmotion(
                            enemy,
                            bulletStartPosition.position,
                            hit.point,
                            slowmotionBullet,
                            () =>
                            {
                                enemy.PrepareForDeath();
                                hit.rigidbody.AddForce(direction * projectileImpulse, ForceMode.Impulse);
                            });
                    }
                    else
                    {
                        hit.rigidbody.AddForce(direction * projectileImpulse, ForceMode.Impulse);
                    }
                    //}
                    //else
                    //{
                    //enemy.PrepareForDeath();
                    //hit.rigidbody.AddForce(direction * projectileImpulse, ForceMode.Impulse);


                    wasShot = true;
                }

                if (!wasShot)
                {
                    var explosive = hit.transform.GetComponentInParent<IExplosive>();
                    explosive?.Explode();
                }
            }
        }
    }
}