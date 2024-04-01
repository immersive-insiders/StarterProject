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

public class FancyResizer
{

    public GameObject CreateResizedObject(Vector3 newSize, GameObject parent, FancyResizable sourcePrefab)
    {
        GameObject prefab = Object.Instantiate(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
        prefab.name = sourcePrefab.name;

        FancyResizable resizable = prefab.GetComponent<FancyResizable>();
        resizable.SetNewSize(newSize);
        if (resizable == null)
        {
            return prefab;
        }

        Mesh resizedMesh = ProcessVertices(resizable, newSize);

        MeshFilter mf = prefab.GetComponent<MeshFilter>();
        mf.sharedMesh = resizedMesh;
        mf.sharedMesh.RecalculateBounds();

        // child it after creation so the bounds math plays nicely
        prefab.transform.parent = parent.transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;

        return prefab;
    }

    #region PRIVATE METHODS

    public static Mesh ProcessVertices(FancyResizable resizable, Vector3 newSize, bool pivot = false)
    {
        Mesh originalMesh = resizable.currentMesh;
        Vector3 originalBounds = resizable.DefaultSize;

        // Force scaling if newSize is smaller than the original mesh
        FancyResizable.Method methodX = (originalBounds.x < newSize.x || resizable.ScalingX == FancyResizable.Method.NONE)
          ? resizable.ScalingX
          : FancyResizable.Method.SCALE;
        FancyResizable.Method methodY = (originalBounds.y < newSize.y || resizable.ScalingY == FancyResizable.Method.NONE)
          ? resizable.ScalingY
          : FancyResizable.Method.SCALE;
        FancyResizable.Method methodZ = (originalBounds.z < newSize.z || resizable.ScalingZ == FancyResizable.Method.NONE)
          ? resizable.ScalingZ
          : FancyResizable.Method.SCALE;

        Vector3[] resizedVertices = originalMesh.vertices;

        Vector3 localPivot = resizable.transform.InverseTransformPoint(resizable.PivotPosition);
        float pivotX = (1 / resizable.DefaultSize.x) * localPivot.x;
        float pivotY = (1 / resizable.DefaultSize.y) * localPivot.y;
        float pivotZ = (1 / resizable.DefaultSize.z) * localPivot.z;

        for (int i = 0; i < resizedVertices.Length; i++)
        {
            Vector3 vertexPosition = resizedVertices[i];
            vertexPosition.x = CalculateNewVertexPosition(
              methodX,
              vertexPosition.x,
              originalBounds.x,
              newSize.x,
              resizable.DefaultSize.x * (0.5f - resizable.PaddingX),
              resizable.DefaultSize.x * (resizable.PaddingXMax - 0.5f),
              pivotX
            );

            vertexPosition.y = CalculateNewVertexPosition(
              methodY,
              vertexPosition.y,
              originalBounds.y,
              newSize.y,
              resizable.DefaultSize.y * (0.5f - resizable.PaddingY),
              resizable.DefaultSize.y * (resizable.PaddingYMax - 0.5f),
              pivotY
            );

            vertexPosition.z = CalculateNewVertexPosition(
              methodZ,
              vertexPosition.z,
              originalBounds.z,
              newSize.z,
              resizable.DefaultSize.z * (0.5f - resizable.PaddingZ),
              resizable.DefaultSize.z * (resizable.PaddingZMax - 0.5f),
              pivotZ
            );
            if (pivot)
            {
                vertexPosition += resizable.transform.InverseTransformPoint(resizable.PivotPosition);
            }

            resizedVertices[i] = vertexPosition;
        }
        Mesh clonedMesh = MonoBehaviour.Instantiate(originalMesh);
        clonedMesh.vertices = resizedVertices;
        return clonedMesh;
    }

    private static float CalculateNewVertexPosition(
      FancyResizable.Method resizeMethod,
      float currentPosition,
      float currentSize,
      float newSize,
      float padding,
      float paddingMax,
      float pivot
    )
    {
        float resizedRatio = (newSize - currentSize) / 2;
        float sign = Mathf.Sign(currentPosition);
        switch (resizeMethod)
        {
            case FancyResizable.Method.ADAPT:
                if (Mathf.Abs(currentPosition) >= padding)
                {
                    currentPosition += resizedRatio * sign;
                }
                break;

            case FancyResizable.Method.ADAPT_WITH_ASYMMETRICAL_PADDING:
                if (currentPosition <= paddingMax || padding <= currentPosition)
                {
                    currentPosition += resizedRatio * sign;
                }
                break;

            case FancyResizable.Method.SCALE:
                currentPosition *= newSize / currentSize;
                break;

            case FancyResizable.Method.NONE:
                break;
        }

        float pivotPos = newSize * (-pivot);
        currentPosition += pivotPos;

        return currentPosition;
    }
    #endregion
}
