using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void SetWalk(bool isWalk)
    {
        _animator.SetInteger("Speed", isWalk ? 1 : 0); 
    }

    public void SetShoot(bool isShooting)
    {
        _animator.SetBool("IsShooting", isShooting);
    }

    public void SetCroushing(bool isCrouching)
    {
        _animator.SetBool("IsCrouching", isCrouching);
    }
}
