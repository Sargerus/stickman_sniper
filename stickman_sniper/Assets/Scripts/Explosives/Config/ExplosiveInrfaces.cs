using System.Collections.Generic;
using UnityEngine;

namespace StickmanSniper.Explosives
{
    public interface IExplosive
    {
        List<Collider> Explode();
    }
}