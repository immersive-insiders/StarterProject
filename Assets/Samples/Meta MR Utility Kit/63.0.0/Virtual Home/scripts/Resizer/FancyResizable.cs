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

using UnityEngine;

[ExecuteInEditMode]
public class FancyResizable : MonoBehaviour
{
    public Vector3 PivotPosition => _pivotTransform.position;

    [Space(15)] public Method ScalingX;
    [Range(0, 0.5f)] public float PaddingX;
    [Range(0, 0.5f)] public float PaddingXMax;

    [Space(15)] public Method ScalingY;
    [Range(0, 0.5f)] public float PaddingY;
    [Range(0, 0.5f)] public float PaddingYMax;

    [Space(15)] public Method ScalingZ;
    [Range(0, 0.5f)] public float PaddingZ;
    [Range(0, 0.5f)] public float PaddingZMax;

    [SerializeField] public Mesh currentMesh;
    [SerializeField] private Vector3 _newSize;
    [SerializeField] private bool _updateInPlayMode;
    [SerializeField] private Transform _pivotTransform;

    [SerializeField] private bool swapMesh;
    [SerializeField] public Mesh OriginalMesh;
    [SerializeField] public Material OriginalMaterial;
    [SerializeField] public float maxScaleBeforeSwap = 2f;
    [SerializeField] private Mesh OversizeMesh;
    [SerializeField] public Material OversizeMaterial;
    [SerializeField] public float minScaleBeforeSwap = 0.5f;
    [SerializeField] private Mesh UndersizeMesh;
    [SerializeField] public Material UndersizeMaterial;

    private bool changed = false;

    public enum Method
    {
        ADAPT,
        ADAPT_WITH_ASYMMETRICAL_PADDING,
        SCALE,
        NONE
    }
    public Vector3 DefaultSize { get; set; }

    MeshFilter _meshFilter;
    public void SetNewSize(Vector3 newSize) => _newSize = newSize;

    private Color[] axisGizmosColors = {
        new Color(1, 0, 0, 0.5f), // red - X
        new Color(0, 1, 0, 0.5f), // green - Y
        new Color(0, 0, 1, 0.5f) // blue - Z
    };

    private Vector3 _currentSize;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = GetComponentInChildren<MeshFilter>(true);
        }
        if (_meshFilter != null)
        {
            if(OriginalMesh != null)
            {
                currentMesh = OriginalMesh;
            }
            else
            {
                currentMesh = _meshFilter.sharedMesh;
            }
            DefaultSize = OriginalMesh.bounds.size;
            _currentSize = _newSize = DefaultSize;
        }

        if (!_pivotTransform)
        {
            _pivotTransform = transform.Find("Pivot");
        }
    }


    public void Update()
    {
        if ((Application.isPlaying && !_updateInPlayMode))
        {
            return;
        }
        if (!_pivotTransform)
        {
            _pivotTransform = transform.Find("Pivot");
        }
        if (!_meshFilter)
        {
            _meshFilter = GetComponentInChildren<MeshFilter>();
        }
        if (_meshFilter && _pivotTransform && _currentSize != _newSize)
        {
            if (minScaleBeforeSwap >= 0 || maxScaleBeforeSwap >= 1)
            {
                SwapMesh();
            }

            Mesh resizedMesh = FancyResizer.ProcessVertices(this, _newSize, true);
            _meshFilter.sharedMesh = resizedMesh;
            _meshFilter.sharedMesh.RecalculateBounds();
            _currentSize = _newSize;
        }
    }

    private void SwapMesh()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        changed = false;
        if(meshRenderer && OriginalMaterial && OriginalMesh)
        {
            if ((_newSize.x <= minScaleBeforeSwap ||
                _newSize.y <= minScaleBeforeSwap ||
                _newSize.z <= minScaleBeforeSwap) &&
                UndersizeMesh != null)
            {
                currentMesh = UndersizeMesh;
                meshRenderer.material = UndersizeMaterial;
                changed = true;
            }
            if ((_newSize.x >= maxScaleBeforeSwap ||
                _newSize.y >= maxScaleBeforeSwap ||
                _newSize.z >= maxScaleBeforeSwap) &&
                UndersizeMesh != null &&
                !changed
                )
            {
                currentMesh = OversizeMesh;
                meshRenderer.material = OversizeMaterial;
                changed = true;
            }
            if(!changed)
            {
                currentMesh = OriginalMesh;
                meshRenderer.material = OriginalMaterial;
            }
            DefaultSize = currentMesh.bounds.size;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_pivotTransform)
        {
            return;
        }

        Gizmos.color = Color.red;
        float lineSize = 0.1f;
        Vector3 localPivot = _pivotTransform.position;
        Vector3 startX = localPivot + Vector3.left * lineSize * 0.5f;
        Vector3 startY = localPivot + Vector3.down * lineSize * 0.5f;
        Vector3 startZ = localPivot + Vector3.back * lineSize * 0.5f;

        Gizmos.DrawRay(startX, Vector3.right * lineSize);
        Gizmos.DrawRay(startY, Vector3.up * lineSize);
        Gizmos.DrawRay(startZ, Vector3.forward * lineSize);
    }

    private void OnDrawGizmosSelected()
    {
        if (_meshFilter == null)
            return;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 newCenter = _meshFilter.sharedMesh.bounds.center;
        Method[] scaling = {ScalingX, ScalingY, ScalingZ };
        float[] padding = {PaddingX, PaddingY, PaddingZ};
        float[] paddingMax = {PaddingXMax, PaddingYMax, PaddingZMax};

        for (int i = 0; i <= 2; i++)
        {
            Gizmos.color = axisGizmosColors[i];
            DrawPaddingCubeGizmo(newCenter, scaling[i], padding[i], paddingMax[i], i);
        }
        Gizmos.color = new Color(0, 1, 1, 1);
        Gizmos.DrawWireCube(newCenter, DefaultSize);
    }

    private void DrawPaddingCubeGizmo(Vector3 newCenter, Method scalingMethod, float padding, float paddingMax, int axis)
    {
        Vector3 center;
        Vector3 size;
        switch (scalingMethod)
        {
            case Method.ADAPT:
                size = _newSize;
                size[axis] = DefaultSize[axis] *(0.5f - padding) * 2;
                Gizmos.DrawWireCube(newCenter, size);
                break;
            case Method.ADAPT_WITH_ASYMMETRICAL_PADDING:
                size = _newSize;
                size[axis] = 0;
                center = newCenter;
                center[axis] = newCenter[axis] + (DefaultSize[axis] * (0.5f - padding));
                Gizmos.DrawWireCube(center, size);
                center = newCenter;
                center[axis] = newCenter[axis] + (DefaultSize[axis] * (paddingMax - 0.5f));
                Gizmos.DrawWireCube(center, size);
                break;
            case Method.NONE:
                Gizmos.DrawWireCube(newCenter, _newSize);
                break;
        }
    }
}
