using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class yandexCrashDebug : MonoBehaviour
{
    private SceneContext sceneContext;

    // Start is called before the first frame update
    void Start()
    {
        sceneContext = GetComponent<SceneContext>();


    }
}
