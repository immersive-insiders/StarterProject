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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureResizer : MonoBehaviour
{
    public AnchorPrefabSpawner furnitureSpawner;

    private void Awake()
    {
        furnitureSpawner.onPrefabSpawned.AddListener(() => ResizeFurniture());
    }

    void ResizeFurniture()
    {
        foreach (GameObject f in furnitureSpawner.SpawnedPrefabs)
        {
            FancyResizable resizable = f.GetComponentInChildren<FancyResizable>(true);
            if (resizable != null)
            {
                var anchor = f.GetComponentInParent<MRUKAnchor>(true);
                if (anchor != null)
                {
                    if (anchor.HasVolume)
                    {
                        var newSize = anchor.VolumeBounds.Value.size;
                        resizable.transform.localPosition = new Vector3(0, -newSize.z, 0);
                        var rotatedSize = new Vector3(newSize.x, newSize.z, newSize.y);
                        var forwardDirection = f.transform.localRotation * Vector3.forward;
                        if (Mathf.Abs(forwardDirection.x) > Utilities.InvSqrt2)
                        {
                            rotatedSize = new Vector3(newSize.y, newSize.z, newSize.x);
                        }
                        resizable.SetNewSize(rotatedSize);
                        BoxCollider collider = f.GetComponentInChildren<BoxCollider>(true);
                        if (collider != null)
                        {
                            var center = anchor.VolumeBounds.Value.center;
                            var rotatedCenter = new Vector3(center.x, center.z, center.y);
                            collider.size = rotatedSize;
                            collider.center = rotatedCenter;
                        }
                    }
                    else if (anchor.HasPlane)
                    {
                        var newSize = anchor.PlaneRect.Value.size;
                        resizable.transform.localPosition = new Vector3(0, -newSize.y / 2, 0);
                        float avgScale = (f.transform.localScale.x + f.transform.localScale.y) / 2.0f;
                        resizable.SetNewSize(new Vector3(newSize.x, newSize.y, avgScale * resizable.DefaultSize.z));
                    }
                    f.transform.localPosition = Vector3.zero;
                    f.transform.localScale = Vector3.one;
                }
            }
        }
    }
}
