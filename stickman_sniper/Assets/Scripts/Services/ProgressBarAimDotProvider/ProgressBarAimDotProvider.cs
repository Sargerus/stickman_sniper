using UnityEngine;

public interface IProgressBarAimDotProvider
{
    Vector3 Point { get; set; }
}

public class ProgressBarAimDotProvider : IProgressBarAimDotProvider
{
    public Vector3 Point { get; set; }
}
