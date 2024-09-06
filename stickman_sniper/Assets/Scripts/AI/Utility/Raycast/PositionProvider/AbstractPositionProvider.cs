using UnityEngine;

namespace StickmanSniper.AI
{
    public abstract class AbstractPositionProvider : MonoBehaviour
    {
        public abstract Vector3 GetPosition();
    }
}
