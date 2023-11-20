using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class testsuka
{
    private FirstPersonController.Factory _asd;

    public testsuka(FirstPersonController.Factory asd)
    {
        _asd = asd;
        var palyer = _asd.Create();
        Debug.Log(palyer);
    }
}
