using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Management;
using H2DT.Management.Booting;
using H2DT.Management.Scenes;
using UnityEngine;

public class LoadSceneAfterTime : BooterSubject
{

    [Space]
    [SerializeField]
    private SceneHandler _sceneHandler;

    [Space]
    [SerializeField]
    protected SceneInfo _sceneInfo;

    [SerializeField]
    protected float _time;

    // Start is called before the first frame update
    void Start()
    {
        WaitAndLoad(_time);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected async void WaitAndLoad(float time)
    {
        int convertedTime = (int)time * 1000;

        await Task.Delay(convertedTime);

        await _sceneHandler.LoadScene(_sceneInfo);
    }
}
