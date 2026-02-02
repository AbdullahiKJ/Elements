using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    public int targetFrameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}