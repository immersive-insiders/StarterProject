// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using UnityEngine;
using Oculus.Haptics;
using System;

// This scene is a minimal integration example, meant to run on device (f.e. Meta Quest 2, Meta Quest Pro).
// It showcases how events, like button presses, can be hooked up to haptic feedback; and how we can use other input, like
// a controller's thumbstick movements, to modulate haptic effects.
// We gain access to the Haptics SDK's features through an API by importing Oculus.Haptics (see above).
public class HapticsSdkPlaySample : MonoBehaviour
{
    // The haptic clips are assignable in the Unity editor.
    // For this example, we are using the two demo clips found in Assets/Haptics.
    // Haptic clips can be designed in Haptics Studio (https://developer.oculus.com/experimental/exp-haptics-studio)
    public HapticClip clip1;
    public HapticClip clip2;
    HapticClipPlayer _playerLeft1;
    HapticClipPlayer _playerLeft2;
    HapticClipPlayer _playerRight1;
    HapticClipPlayer _playerRight2;

    protected virtual void Start()
    {
        // We create two haptic clip players for each hand.
        _playerLeft1 = new HapticClipPlayer(clip1);
        _playerLeft2 = new HapticClipPlayer(clip2);
        _playerRight1 = new HapticClipPlayer(clip1);
        _playerRight2 = new HapticClipPlayer(clip2);

        // We increase the priority for the second player on both hands.
        _playerLeft2.priority = 1;
        _playerRight2.priority = 1;
    }

    // This helper function allows us to identify the controller we are currently playing back on.
    // We use this further down for logging purposes.
    String GetControllerName(OVRInput.Controller controller)
    {
        if (controller == OVRInput.Controller.LTouch)
        {
            return "left controller";
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            return "right controller";
        }

        return "unknown controller";
    }

    // This section provides a series of interactions that showcase the playback and modulation capabilities of the
    // Haptics SDK.
    void HandleControllerInput(OVRInput.Controller controller, HapticClipPlayer clipPlayer1, HapticClipPlayer clipPlayer2, Controller hand)
    {
        string controllerName = GetControllerName(controller);

        try
        {
            // Play first clip with default priority using the index trigger
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                clipPlayer1.Play(hand);
                Debug.Log("Should feel vibration from clipPlayer1 on " + controllerName + ".");
            }

            // Play second clip with higher priority using the grab button
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            {
                clipPlayer2.Play(hand);
                Debug.Log("Should feel vibration from clipPlayer2 on " + controllerName + ".");
            }

            // Stop first clip when releasing the index trigger
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                clipPlayer1.Stop();
                Debug.Log("Vibration from clipPlayer1 on " + controllerName + " should stop.");
            }

            // Stop second clip when releasing the grab button
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
            {
                clipPlayer2.Stop();
                Debug.Log("Vibration from clipPlayer2 on " + controllerName + " should stop.");
            }

            // Loop first clip using the B/Y-button
            if (OVRInput.GetDown(OVRInput.Button.Two, controller))
            {
                clipPlayer1.isLooping = !clipPlayer1.isLooping;
                Debug.Log(String.Format("Looping should be {0} on " + controllerName + ".", clipPlayer1.isLooping));
            }

            // Modulate the amplitude and frequency of the first clip using the thumbstick
            // - Moving left/right modulates the frequency shift
            // - Moving up/down modulates the amplitude
            if (controller == OVRInput.Controller.LTouch)
            {
                clipPlayer1.amplitude = Mathf.Clamp(1.0f + OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y, 0.0f, 1.0f);
                clipPlayer1.frequencyShift = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x;
            }
            else if (controller == OVRInput.Controller.RTouch)
            {
                clipPlayer1.amplitude = Mathf.Clamp(1.0f + OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y, 0.0f, 1.0f);
                clipPlayer1.frequencyShift = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;
            }
        }

        // If any exceptions occur, we catch and log them here.
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    // We poll for controller interactions on every frame using the Update() loop
    protected virtual void Update()
    {
        HandleControllerInput(OVRInput.Controller.LTouch, _playerLeft1, _playerLeft2, Controller.Left);
        HandleControllerInput(OVRInput.Controller.RTouch, _playerRight1, _playerRight2, Controller.Right);
    }

    protected virtual void OnDestroy()
    {
        _playerLeft1?.Dispose();
        _playerLeft2?.Dispose();
        _playerRight1?.Dispose();
        _playerRight2?.Dispose();
    }

    // Upon exiting the application (or when playmode is stopped) we release the haptic clip players and uninitialize (dispose) the SDK.
    protected virtual void OnApplicationQuit()
    {
        Haptics.Instance.Dispose();
    }
}
