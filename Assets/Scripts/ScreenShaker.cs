using UnityEngine;
using Cinemachine;

//Adapted from
//https://github.com/Lumidi/CameraShakeInCinemachine/blob/master/SimpleCameraShakeInCinemachine.cs
public class ScreenShaker : MonoBehaviour 
{
    public float ShakeAmplitude = 1.2f;         // Cinemachine Noise Profile Parameter
    public float ShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    private float ShakeElapsedTime = 0f;
    
    private CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    // Use this for initialization
    void Start()
    {
        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShakeElapsedTime > 0)
        {
            // Set Cinemachine Camera Noise parameters
            virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
            virtualCameraNoise.m_FrequencyGain = ShakeFrequency;
            
            ShakeElapsedTime -= Time.deltaTime;
        }
        else //Reset when over
        {
            virtualCameraNoise.m_AmplitudeGain = 0f;
            ShakeElapsedTime = 0f;
        }
    }

    public void Shake(float duration = 0.3f)
    {
        ShakeElapsedTime = duration;
    }
}