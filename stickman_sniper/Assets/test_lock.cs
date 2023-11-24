using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class test_lock : MonoBehaviour
{
    private ILevelLoader _levelLoader;

    [Inject]
    public void Construct(ILevelLoader levelLoader)
    {
        _levelLoader = levelLoader;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
        _levelLoader.LoadLevel();
    }
}
