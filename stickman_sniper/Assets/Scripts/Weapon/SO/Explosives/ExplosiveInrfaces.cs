using System.Collections.Generic;
using UnityEngine;

namespace stickman_sniper.Weapon.Explosives
{
    public interface IExplosive
    {
        List<Collider> Explode();
    }
}