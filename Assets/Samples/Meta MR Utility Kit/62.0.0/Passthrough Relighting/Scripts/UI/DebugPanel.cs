/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.XR.MRUtilityKit;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private Toggle shadowsToggle;
    [SerializeField] private Toggle highlightsToggle;
    [SerializeField] private Toggle globalMeshToggle;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Slider lightAttenuationSlider;
    [SerializeField] private Slider passthroughBrightnessSlider;
    [SerializeField] private Slider lightOpaquenessSlider;

    [SerializeField] private Renderer oppyRenderer;
    [SerializeField] private GameObject blobShadowProjector;
    [SerializeField] private Material sceneMaterial;
    [SerializeField] private OppyCharacterController oppyController;
    [SerializeField] private OppyLightGlow oppyLightGlow;
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    private EffectMesh[] effectMeshes;
    private const string HighLightAttenuationShaderPropertyName = "_HighLightAttenuation";
    private const string HighLightOpaquenessShaderPropertyName = "_HighlightOpacity";

    private void Awake()
    {
        shadowsToggle.onValueChanged.AddListener(ShadowsSettingsToggled);
        highlightsToggle.onValueChanged.AddListener(HighlightSettingsToggled);
        globalMeshToggle.onValueChanged.AddListener(GlobalMeshSettingsToggled);
        respawnButton.onClick.AddListener(oppyController.Respawn);
        lightAttenuationSlider.onValueChanged.AddListener(
            (val) => { sceneMaterial.SetFloat(HighLightAttenuationShaderPropertyName, val); }
        );
        lightOpaquenessSlider.onValueChanged.AddListener(
            (val) => { sceneMaterial.SetFloat(HighLightOpaquenessShaderPropertyName, val); }
        );
        passthroughBrightnessSlider.onValueChanged.AddListener(
            (brightness) =>
            {
                passthroughLayer.SetBrightnessContrastSaturation(brightness);
            }
        );
        effectMeshes = FindObjectsOfType<EffectMesh>();
    }

    public void DisableGlobalMeshToggle()
    {
        bool globalMeshExists = MRUK.Instance && MRUK.Instance.GetCurrentRoom() && MRUK.Instance.GetCurrentRoom().GetGlobalMeshAnchor();
        globalMeshToggle.interactable = globalMeshExists;
    }

    private void GlobalMeshSettingsToggled(bool globalMeshActive)
    {
        if (globalMeshActive && !Application.isEditor)
        {
            foreach (var effectMesh in effectMeshes)
            {
                if ((effectMesh.Labels & MRUKAnchor.SceneLabels.GLOBAL_MESH) == 0)
                {
                    effectMesh.DestroyMesh();
                }
                else
                {
                    effectMesh.CreateMesh();
                }
            }
        }
        else
        {
            foreach (var effectMesh in effectMeshes)
            {
                if ((effectMesh.Labels & MRUKAnchor.SceneLabels.GLOBAL_MESH) != 0)
                {
                    effectMesh.DestroyMesh(LabelFilter.FromEnum(effectMesh.Labels));
                }
                else
                {
                    effectMesh.CreateMesh();
                }
            }
        }
        oppyController.Respawn();
    }

    private void HighlightSettingsToggled(bool highlightsOn)
    {
        sceneMaterial.SetFloat(HighLightAttenuationShaderPropertyName, highlightsOn ? 1 : 0);
        oppyLightGlow.SetGlowActive(highlightsOn);
    }

    private void ShadowsSettingsToggled(bool realtimeShadows)
    {
        if (realtimeShadows)
        {
            oppyRenderer.shadowCastingMode = ShadowCastingMode.On;
            blobShadowProjector.SetActive(false);
        }
        else
        {
            oppyRenderer.shadowCastingMode = ShadowCastingMode.Off;
            blobShadowProjector.SetActive(true);
        }
    }
}
