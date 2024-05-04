using EasyProgressBar;
using UnityEngine;
using Zenject;

public class EnemyHPCanvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private ProgressBar progressBar;

    private IProgressBarAimDotProvider _progressBarAimDotProvider;

    [Inject]
    private void Construct([InjectOptional] IProgressBarAimDotProvider progressBarAimDotProvider)
    {
        _progressBarAimDotProvider = progressBarAimDotProvider;
    }

    private void Update()
    {
        if (_progressBarAimDotProvider != null)
            progressBar.transform.rotation = Quaternion.LookRotation((progressBar.transform.position - _progressBarAimDotProvider.Point).normalized);
    }

    public void SetActiveCanvas(bool active)
    {
        canvas.enabled = active;
    }

    public void SetHp(float hp)
    {
        progressBar.FillAmount = hp;
    }
}
