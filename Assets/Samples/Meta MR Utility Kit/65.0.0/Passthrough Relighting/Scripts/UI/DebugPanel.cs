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

using System;
using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _shadowDropDown;
    [SerializeField] private Toggle _highlightsToggle;
    [SerializeField] private TMP_Dropdown _geometryDropDown;
    [SerializeField] private Button _respawnButton;
    [SerializeField] private Slider _lightIntensitySlider;
    [SerializeField] private Slider _passthroughBrightnessSlider;
    [SerializeField] private Slider _lightBlendFactor;
    [SerializeField] private Renderer _oppyRenderer;
    [SerializeField] private GameObject _blobShadowProjector;
    [SerializeField] private Material _sceneMaterial;
    [SerializeField] private OppyCharacterController _oppyController;
    [SerializeField] private OppyLightGlow _oppyLightGlow;
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

    private EffectMesh[] effectMeshes;
    private const string HighLightAttenuationShaderPropertyName = "_HighLightAttenuation";
    private const string HighLightOpaquenessShaderPropertyName = "_HighlightOpacity";

    private void Awake()
    {
        _shadowDropDown.onValueChanged.AddListener(ShadowsSettingsChanged);
        _highlightsToggle.onValueChanged.AddListener(HighlightSettingsToggled);
        _geometryDropDown.onValueChanged.AddListener(GeometrySettingsChanged);
        _respawnButton.onClick.AddListener(_oppyController.Respawn);
        _lightIntensitySlider.onValueChanged.AddListener(
            (val) => { _sceneMaterial.SetFloat(HighLightAttenuationShaderPropertyName, val); }
        );
        _lightBlendFactor.onValueChanged.AddListener(
            (val) => { _sceneMaterial.SetFloat(HighLightOpaquenessShaderPropertyName, val); }
        );
        _passthroughBrightnessSlider.onValueChanged.AddListener(
            (brightness) => { _passthroughLayer.SetBrightnessContrastSaturation(brightness); }
        );
        effectMeshes = FindObjectsOfType<EffectMesh>();
    }

    private void Start()
    {
        ToggleGeometryDropDown();
        if (_highlightsToggle)
            _highlightsToggle.isOn = true;
        if (_lightIntensitySlider)
            _lightIntensitySlider.value = 0.5f;
        if (_geometryDropDown)
            _geometryDropDown.value = 0;
    }

    public void ToggleGeometryDropDown()
    {
        bool globalMeshExists = MRUK.Instance && MRUK.Instance.GetCurrentRoom() &&
                                MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor;
        _geometryDropDown.interactable = globalMeshExists;
    }

    private void GeometrySettingsChanged(int optionSelected)
    {
        if (optionSelected == 1)
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

        _oppyController.Respawn();
    }

    private void HighlightSettingsToggled(bool highlightsOn)
    {
        _sceneMaterial.SetFloat(HighLightAttenuationShaderPropertyName, highlightsOn ? 1 : 0);
        _oppyLightGlow.SetGlowActive(highlightsOn);
    }

    private void ShadowsSettingsChanged(int dynamicShadow)
    {
        if (dynamicShadow == 0)
        {
            _oppyRenderer.shadowCastingMode = ShadowCastingMode.On;
            _blobShadowProjector.SetActive(false);
        }
        else
        {
            _oppyRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _blobShadowProjector.SetActive(true);
        }
    }
}
