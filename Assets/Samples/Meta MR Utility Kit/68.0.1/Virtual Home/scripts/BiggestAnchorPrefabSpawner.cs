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

using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    /// Demonstrates how to implement custom spawning logic using the AnchorPrefabSpawner base class.
    /// This class selects the largest anchor and spawns a specific prefab for it, while providing custom alignment options.
    /// </summary>
    public class BiggestAnchorPrefabSpawner : AnchorPrefabSpawner
    {
        private List<MRUKAnchor> anchors;
        private MRUKAnchor _biggestAnchor;
        [SerializeField] public GameObject BiggestPrefab;

        [Header("Custom Alignment")] public FurnitureAnchorsX AnchorToFurnitureX = FurnitureAnchorsX.Center;
        public FurnitureAnchorsY AnchorToFurnitureY = FurnitureAnchorsY.Center;
        public FurnitureAnchorsZ AnchorToFurnitureZ = FurnitureAnchorsZ.Center;

        [System.Serializable]
        public enum FurnitureAnchorsX
        {
            Left,
            Center,
            Right
        }

        [System.Serializable]
        public enum FurnitureAnchorsY
        {
            Top,
            Center,
            Bottom
        }

        [System.Serializable]
        public enum FurnitureAnchorsZ
        {
            Front,
            Center,
            Back
        }

        /// <summary>
        /// Selects a GameObject from a list of prefabs based on certain conditions.
        /// </summary>
        /// <param name="anchor">The MRUKAnchor object to compare with the biggest anchor.</param>
        /// <param name="prefabs">The list of GameObjects to select from.</param>
        /// <returns>
        /// If the anchor is the biggest anchor, it returns the BiggestPrefab.
        /// If the anchor is not the biggest anchor, it selects a random prefab from the list.
        /// </returns>
        public override GameObject CustomPrefabSelection(MRUKAnchor anchor, List<GameObject> prefabs)
        {
            if (_biggestAnchor == null)
            {
                BiggestAnchorForLabel(anchor.Label);
            }

            if (anchor == _biggestAnchor) return BiggestPrefab;
            // When the prefabs list is not empty, it will be used to pick fallbacks for all the other anchors
            if (prefabs == null || prefabs.Count == 0)
            {
                return null; // only instantiate something on top of the biggest anchor. No fallback.
            }

            var random = new System.Random();
            var rndIndex = random.Next(0, prefabs.Count);
            return prefabs[rndIndex]; // fallback to random
        }

        private void BiggestAnchorForLabel(MRUKAnchor.SceneLabels Labels)
        {
            // Find the biggest anchor
            MRUKAnchor tempBiggestAnchor = null;
            float? biggestArea = 0;

            foreach (var anchor in MRUK.Instance.GetCurrentRoom().Anchors)
            {
                if ((Labels & anchor.Label) == 0)
                {
                    continue;
                }

                var area = anchor.VolumeBounds?.size.x * anchor.VolumeBounds?.size.y;
                if (area == null || !(area.Value > biggestArea.Value))
                {
                    continue;
                }

                biggestArea = area;
                tempBiggestAnchor = anchor;
            }

            _biggestAnchor = tempBiggestAnchor;
        }

        /// <summary>
        /// Custom alignment method that aligns the prefab to sit in front the anchor's volume
        /// </summary>
        public override Vector3 CustomPrefabAlignment(Bounds anchorVolumeBounds,
            Bounds? prefabBounds)
        {
            var newPos = anchorVolumeBounds.center;
            switch (AnchorToFurnitureX)
            {
                case FurnitureAnchorsX.Left:
                    newPos.x = anchorVolumeBounds.min.x;
                    break;
                case FurnitureAnchorsX.Right:
                    newPos.x = anchorVolumeBounds.max.x;
                    break;
                case FurnitureAnchorsX.Center:
                default:
                    break;
            }

            switch (AnchorToFurnitureY)
            {
                case FurnitureAnchorsY.Top:
                    newPos.z = anchorVolumeBounds.max.z;
                    break;
                case FurnitureAnchorsY.Bottom:
                    newPos.z = anchorVolumeBounds.min.z;
                    break;
                case FurnitureAnchorsY.Center:
                default:
                    break;
            }

            switch (AnchorToFurnitureZ)
            {
                case FurnitureAnchorsZ.Front:
                    newPos.y = anchorVolumeBounds.max.y;
                    break;
                case FurnitureAnchorsZ.Back:
                    newPos.y = anchorVolumeBounds.min.y;
                    break;
                case FurnitureAnchorsZ.Center:
                default:
                    break;
            }

            return newPos;
        }
    }
}
