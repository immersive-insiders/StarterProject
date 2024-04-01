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
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(FancyResizable))]
public class FancyResizableEditor : Editor
{
    SerializedProperty originalMesh;
    SerializedProperty originalMaterial;
    SerializedProperty pivotTransform;

    SerializedProperty swapMesh;
    SerializedProperty maxScaleBeforeSwap;
    SerializedProperty oversizeMesh;
    SerializedProperty oversizeMaterial;
    SerializedProperty minScaleBeforeSwap;
    SerializedProperty undersizeMesh;
    SerializedProperty undersizeMaterial;

    SerializedProperty scalingX;
    SerializedProperty scalingY;
    SerializedProperty scalingZ;

    SerializedProperty paddingX;
    SerializedProperty paddingY;
    SerializedProperty paddingZ;

    SerializedProperty paddingXMax;
    SerializedProperty paddingYMax;
    SerializedProperty paddingZMax;

    SerializedProperty updateInPlayMode;

    SerializedProperty newSize;

    GUIStyle underText;

    private void OnEnable()
    {

        originalMesh = serializedObject.FindProperty("OriginalMesh");
        originalMaterial = serializedObject.FindProperty("OriginalMaterial");
        pivotTransform = serializedObject.FindProperty("_pivotTransform");
        swapMesh = serializedObject.FindProperty("swapMesh");
        maxScaleBeforeSwap = serializedObject.FindProperty("maxScaleBeforeSwap");
        oversizeMesh = serializedObject.FindProperty("OversizeMesh");
        undersizeMaterial = serializedObject.FindProperty("UndersizeMaterial");
        minScaleBeforeSwap = serializedObject.FindProperty("minScaleBeforeSwap");
        undersizeMesh = serializedObject.FindProperty("UndersizeMesh");
        oversizeMaterial = serializedObject.FindProperty("OversizeMaterial");
        scalingX = serializedObject.FindProperty("ScalingX");
        scalingY = serializedObject.FindProperty("ScalingY");
        scalingZ = serializedObject.FindProperty("ScalingZ");
        paddingX = serializedObject.FindProperty("PaddingX");
        paddingY = serializedObject.FindProperty("PaddingY");
        paddingZ = serializedObject.FindProperty("PaddingZ");
        paddingXMax = serializedObject.FindProperty("PaddingXMax");
        paddingYMax = serializedObject.FindProperty("PaddingYMax");
        paddingZMax = serializedObject.FindProperty("PaddingZMax");
        updateInPlayMode = serializedObject.FindProperty("_updateInPlayMode");
        newSize = serializedObject.FindProperty("_newSize");
        underText = new GUIStyle();
        underText.fontStyle = FontStyle.Italic;
        underText.normal.textColor = Color.gray;
    }


    public override void OnInspectorGUI()
    {
        FancyResizable resizable = target as FancyResizable;

        serializedObject.Update();


        EditorGUILayout.PropertyField(updateInPlayMode);
        EditorGUILayout.PropertyField(originalMesh, new GUIContent("Original Mesh", "The mesh to resize"));
        EditorGUILayout.PropertyField(originalMaterial, new GUIContent("Original Material", "The original material for this mesh"));
        EditorGUILayout.PropertyField(pivotTransform, new GUIContent("Pivot Transform", "The point used to calculate the position of the mesh after being resized.\n" +
            "Any child named 'Pivot' will be used automatically."));
        EditorGUILayout.PropertyField(swapMesh);
        if(swapMesh.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(maxScaleBeforeSwap);
            EditorGUILayout.PropertyField(oversizeMesh);
            EditorGUILayout.PropertyField(oversizeMaterial);
            EditorGUILayout.PropertyField(minScaleBeforeSwap);
            EditorGUILayout.PropertyField(undersizeMesh);
            EditorGUILayout.PropertyField(undersizeMaterial);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.PropertyField(scalingX, new GUIContent("Scaling X", "The scaling method applied on the X axis"));
        EditorGUI.indentLevel++;
        switch (resizable.ScalingX)
        {
            case FancyResizable.Method.ADAPT:
                EditorGUILayout.PropertyField(paddingX,
                    new GUIContent("Padding X", "Define a symmetrical padding area that will not be stretched on this axis"));
                break;
            case FancyResizable.Method.ADAPT_WITH_ASYMMETRICAL_PADDING:
                EditorGUILayout.PropertyField(paddingX,
                    new GUIContent("Padding X Min", "Lower bound of an asymmetrical padding area that will not be stretched on this axis"));
                EditorGUILayout.PropertyField(paddingXMax,
                    new GUIContent("Padding X Max", "Upper bound of an asymmetrical padding area that will not be stretched on this axis"));
                break;
            case FancyResizable.Method.SCALE:
                CreateUnderText("Unity's default scaling method", underText);
                break;
            case FancyResizable.Method.NONE:
                CreateUnderText("Lock the scale on this axis", underText);
                break;
            default:
                break;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(scalingY, new GUIContent("Scaling Y", "The scaling method applied on the Z axis"));
        EditorGUI.indentLevel++;
        switch (resizable.ScalingY)
        {
            case FancyResizable.Method.ADAPT:
                EditorGUILayout.PropertyField(paddingY,
                    new GUIContent("Padding Y", "Define a symmetrical padding area that will not be stretched on this axis"));
                break;
            case FancyResizable.Method.ADAPT_WITH_ASYMMETRICAL_PADDING:
                EditorGUILayout.PropertyField(paddingY,
                    new GUIContent("Padding Y Min", "Lower bound of an asymmetrical padding area that will not be stretched on this axis"));
                EditorGUILayout.PropertyField(paddingYMax,
                    new GUIContent("Padding Y Max", "Upper bound of an asymmetrical padding area that will not be stretched on this axis"));
                break;
            case FancyResizable.Method.SCALE:
                CreateUnderText("Unity's default scaling method", underText);
                break;
            case FancyResizable.Method.NONE:
                CreateUnderText("Lock the scale on this axis", underText);
                break;
            default:
                break;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(scalingZ, new GUIContent("Scaling Z", "The scaling method applied on the Z axis"));
        EditorGUI.indentLevel++;
        switch (resizable.ScalingZ)
        {
            case FancyResizable.Method.ADAPT:
                EditorGUILayout.PropertyField(paddingZ,
                    new GUIContent("Padding Z", "Define a symmetrical padding area that will not be stretched on this axis"));
                break;
            case FancyResizable.Method.ADAPT_WITH_ASYMMETRICAL_PADDING:
                EditorGUILayout.PropertyField(paddingZ,
                    new GUIContent("Padding Z Min", "Lower bound of an asymmetrical padding area that will not be stretched on this axis"));
                EditorGUILayout.PropertyField(paddingZMax,
                    new GUIContent("Padding Z Max", "Upper bound of an asymmetrical padding area that will not be stretched on this axis"));
                break;
            case FancyResizable.Method.SCALE:
                CreateUnderText("Unity's default scaling method", underText);
                break;
            case FancyResizable.Method.NONE:
                CreateUnderText("Lock the scale on this axis", underText);
                break;
            default:
                break;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(newSize, new GUIContent("New Size", "The new size of the mesh"));
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateUnderText(string label, GUIStyle style)
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField(new GUIContent(label), style);
        EditorGUI.indentLevel--;
    }
}
#endif
