//Copyright 2022, Infima Games. All Rights Reserved.

using stickman_sniper.Weapon.Explosives;
using System;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Weapon. This class handles most of the things that weapons need.
    /// </summary>
    public class InfimaAutomaticWeapon : InfimaWeapon
    {
        [SerializeField] private LayerMask layerMask;

        public override void Fire(float spreadMultiplier = 1)
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
            ammunitionCurrent = Mathf.Clamp(ammunitionCurrent - 1, 0, magazineBehaviour.GetAmmunitionTotal());

            //Set the slide back if we just ran out of ammunition.
            if (ammunitionCurrent == 0)
                SetSlideBack(1);

            //Play all muzzle effects.
            muzzleBehaviour.Effect();

            RaycastHit[] result = new RaycastHit[2];

            //Spawn as many projectiles as we need.
            for (var i = 0; i < shotCount; i++)
            {
                //Determine a random spread value using all of our multipliers.
                Vector3 spreadValue = UnityEngine.Random.insideUnitSphere * (spread * spreadMultiplier);
                //Remove the forward spread component, since locally this would go inside the object we're shooting!
                spreadValue.z = 0;
                //Convert to world space.
                //spreadValue = playerCamera.TransformDirection(spreadValue);

                ////Spawn projectile from the projectile spawn point.
                //GameObject projectile = Instantiate(prefabProjectile, playerCamera.position, Quaternion.Euler(playerCamera.eulerAngles + spreadValue));
                ////Add velocity to the projectile.
                //projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileImpulse;

                Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f + spreadValue.x, 0.5f + spreadValue.y, 0));
                Array.Clear(result, 0, result.Length);
                if (Physics.RaycastNonAlloc(ray, result, 100f, layerMask) > 0)
                {
                    var hit = result[0];
                    var enemy = hit.transform.GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        Vector3 direction = (hit.point - playerCamera.position).normalized;
                        direction.y = 0.5f;

                        //if (_levelProgressObserver.TotalEnemies - _levelProgressObserver.KilledEnemies.Value == 1)
                        //{
                        //    await _coreProducer.KillEnemyWeaponSlowmotion(enemy, hit.point,
                        //        () =>
                        //        {
                        //            enemy.PrepareForDeath();
                        //            hit.rigidbody.AddForce(direction * _model.PushForce, ForceMode.Impulse);
                        //        });
                        //}
                        //else
                        //{
                        enemy.PrepareForDeath();
                        hit.rigidbody.AddForce(direction * projectileImpulse, ForceMode.Impulse);
                        //}

                        return;
                    }

                    var explosive = hit.transform.GetComponentInParent<IExplosive>();
                    if (explosive != null)
                    {
                        explosive.Explode();

                        return;
                    }
                }
            }
        }
    }
}