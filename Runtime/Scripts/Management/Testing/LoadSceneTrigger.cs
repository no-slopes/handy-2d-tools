using System.Collections;
using System.Collections.Generic;
using H2DT.Management;
using H2DT.Management.Scenes;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneTrigger : MonoBehaviour
{
    [Space]
    [SerializeField]
    protected SceneInfo _sceneInfo;

    [Space]
    [SerializeField]
    protected Button _loadSceneButton;

    protected void OnEnable()
    {
        _loadSceneButton.onClick.AddListener(Load);
    }

    protected void OnDisable()
    {
        _loadSceneButton.onClick.RemoveListener(Load);
    }

    public void Load()
    {
        // SceneHandler.Instance?.LoadScene(_sceneInfo);
    }
}
