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
using UnityEngine.Events;
using Unity.AI.Navigation;
using Meta.XR.MRUtilityKit;

[RequireComponent(typeof(NavMeshSurface))]
public class ResizeNavMesh : MonoBehaviour
{
    public bool useGlobalMesh = false;
    public UnityEvent onNavMeshInitialized = new UnityEvent();
    private float minimumNavMeshSurfaceArea = 0;

    public Material navMeshMaterial;
    private NavMeshSurface navMeshSurface;
    private EffectMesh[] effectMeshes = null;


    private void Awake()
    {
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
        effectMeshes = FindObjectsByType<EffectMesh>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            ToggleGlobalMesh();
        }
    }

    private void ToggleGlobalMesh()
    {
        if (Application.isEditor)
            return;

        if (!useGlobalMesh && MRUK.Instance?.GetCurrentRoom()?.GetGlobalMeshAnchor())
        {
            // Use scene mesh to define the  navigation surface
            foreach (var effectMesh in effectMeshes)
            {
                if ((effectMesh.Labels & MRUKAnchor.SceneLabels.GLOBAL_MESH) != 0)
                {
                    effectMesh.AddColliders();
                }
                else
                {
                    effectMesh.DestroyColliders();
                }
            }
            InitializeBounds();
        }
        else
        {
            // Use scene objects to define the  navigation surface
            foreach (var effectMesh in effectMeshes)
            {
                if ((effectMesh.Labels & MRUKAnchor.SceneLabels.GLOBAL_MESH) == 0)
                {
                    effectMesh.AddColliders();
                }
                else
                {
                    effectMesh.DestroyColliders();
                }
            }
            navMeshSurface = CreateNavMeshFromRoomBounds();
            navMeshSurface.BuildNavMesh();
        }
        useGlobalMesh = !useGlobalMesh;
    }

    public void InitializeBounds()
    {
        navMeshSurface.BuildNavMesh();
        if (navMeshSurface.navMeshData.sourceBounds.extents.x * navMeshSurface.navMeshData.sourceBounds.extents.z > minimumNavMeshSurfaceArea)
        {
            onNavMeshInitialized?.Invoke();
        }
        else
        {
            Debug.LogWarning("ResizeNavMesh failed to generate a nav mesh, this may be because the room is too small" +
                " or the AgentType settings are to strict");
        }
    }

    private NavMeshSurface CreateNavMeshFromRoomBounds()
    {
        var mapBounds = MRUK.Instance.GetCurrentRoom().GetRoomBounds();
        var mapCenter = new Vector3(mapBounds.center.x, mapBounds.min.y, mapBounds.center.z);
        navMeshSurface.center = mapCenter;

        var mapScale = new Vector3(mapBounds.size.x, 0.05f, mapBounds.size.z);
        navMeshSurface.size = mapScale;
        return navMeshSurface;
    }
}
