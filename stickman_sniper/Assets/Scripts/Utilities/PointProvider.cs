using UnityEngine;

namespace StickmanSniper.Utilities
{
    public interface IPointProvider
    {
        Vector3 Point { get; set; }
    }

    public class PointProvider : IPointProvider
    {
        public Vector3 Point { get; set; }
    }
}