// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using System;
using UnityEngine;
using UnityEngine.UI;

// This script helps a user to be guided through the integration example.
// It provides a text popUp in VR that provides guidance throughout the features of the sample scene.
public class HapticsSdkGuidance : MonoBehaviour
{
    private int _popUpIndex;
    public Text popUpText;

    private void Update()
    {
        try
        {
            // 1) Hold Index Trigger --> Play haptic sample 1 once
            if (_popUpIndex == 0)
            {
                popUpText.text =
                    "Press and hold the Index Trigger on either"
                    + "\n" + "controller to play the first haptic clip once.";
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) ||
                    OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                {
                    _popUpIndex++;
                }
            }
            // 2) Hold Grip Button --> Play haptic sample 2 once
            else if (_popUpIndex == 1)
            {
                popUpText.text =
                    "Press and hold the Grip Button on either"
                    + "\n" + "controller to play the second haptic clip once.";
                if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) ||
                    OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
                {
                    _popUpIndex++;
                }
            }
            // 3) Set loop on first clip (index) using the B/Y-button
            else if (_popUpIndex == 2)
            {
                popUpText.text =
                    "Press B/Y-button to toggle looping on the first clip."
                    + "\n" + "Press and hold Index Trigger to test.";
                if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.LTouch) ||
                    OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
                {
                    _popUpIndex++;
                }
            }
            // 4) Move thumbsticks to modulate haptic clip
            else if (_popUpIndex == 3)
            {
                popUpText.text =
                    "...while holding the Index Trigger, move the thumbstick"
                    + "\n" + "to modulate the playback on that side.";
                if (OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y != 0.0 ||
                    OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y != 0.0)
                {
                    _popUpIndex++;
                }
            }
            // 5) Test Priority --> Second clip should interrupt first clip
            else if (_popUpIndex == 4)
            {
                popUpText.text =
                    "...while looping the first clip, playing back the"
                    + "\n" + "higher priority second clip should"
                    + "\n" + "interrupt the first clip's playback.";
                if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) ||
                    OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
                {
                    _popUpIndex++;
                }
            }
            // 6) End of guide.
            else if (_popUpIndex == 5)
            {
                popUpText.text =
                    "That's all for this integration example!";
            }
        }

        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
