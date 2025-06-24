using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMinimapLevel : MonoBehaviour
{
    public static ChangeMinimapLevel instance;

    public RawImage image;
    private int _currentLevel = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null) instance = this;
        image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChangeLevel.instance.sceneNum = _currentLevel;
        SelectMinimap(_currentLevel);
    }
    public void SetCurrentLevel(int intlevel)
    {
        _currentLevel = intlevel;
    }
    private void SelectMinimap(int currentLevel)
    {
        switch (currentLevel)
        {
            case 0:
                image.material = Resources.Load<Material>("Map/MinimapLevel1Fog");
                return;
            case 1:
                image.material = Resources.Load<Material>("Map/MinimapLevel2Fog");
                return;
            case 2:
                image.material = Resources.Load<Material>("Map/MinimapLevel3Fog");
                return;

        }
    }
}
