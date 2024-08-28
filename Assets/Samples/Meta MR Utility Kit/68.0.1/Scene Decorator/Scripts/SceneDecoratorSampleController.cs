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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.XR.MRUtilityKit;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;

public class SceneDecoratorSampleController : MonoBehaviour
{
    [SerializeField] private SceneDecorator Decorator;
    [SerializeField] private EffectMesh EffectMesh;
    [SerializeField] private EffectMesh EffectMesh_GlobalMesh;
    [SerializeField] public List<InspectorDecoration> Decorations;

    private bool _meshVisibility;
    private readonly Dictionary<KeyCode, Action> keysPressed = new();
    private bool keyPressed;

    [Serializable]
    public struct InspectorDecoration
    {
        public DecorationStyle Style;
        public SceneDecoration Decoration;
    }

    [Serializable]
    public enum DecorationStyle
    {
        None,
        Floor1,
        Floor2,
        Walls,
        Desk,
        Everything
    }

    private void Update()
    {
        for (var i = 0; i < keysPressed.Count; i++)
        {
            var kv = keysPressed.ElementAt(i);
            if (Input.GetKeyDown(kv.Key) && !keyPressed)
            {
                keyPressed = true;
                kv.Value();
            }

            if (Input.GetKeyUp(kv.Key))
            {
                keyPressed = false;
            }
        }
    }


    private async Task Start()
    {
        await MRUK.Instance.LoadSceneFromDevice();
        keysPressed.Add(KeyCode.S, () => ToggleMesh());
        keysPressed.Add(KeyCode.D, () => ClearDecorations());
        keysPressed.Add(KeyCode.F, () => Decoration1());
        keysPressed.Add(KeyCode.G, () => Decoration2());
        keysPressed.Add(KeyCode.H, () => Decoration3());
        keysPressed.Add(KeyCode.J, () => Decoration4());
        keysPressed.Add(KeyCode.K, () => AllDecorations());
    }

    public void ToggleMesh()
    {
        _meshVisibility = !_meshVisibility;
        EffectMesh.ToggleEffectMeshVisibility(_meshVisibility);
        EffectMesh_GlobalMesh.ToggleEffectMeshVisibility(_meshVisibility);
    }

    public void ClearDecorations()
    {
        Decorator.ClearDecorations();
        Decorator.sceneDecorations.Clear();
    }

    public void Decoration1()
    {
        Decorator.sceneDecorations.Clear();
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Floor1));
        Decorator.DecorateScene();
    }

    public void Decoration2()
    {
        Decorator.sceneDecorations.Clear();
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Floor2));
        Decorator.DecorateScene();
    }

    public void Decoration3()
    {
        Decorator.sceneDecorations.Clear();
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Walls));
        Decorator.DecorateScene();
    }

    public void Decoration4()
    {
        Decorator.sceneDecorations.Clear();
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Desk));
        Decorator.DecorateScene();
    }

    public void AllDecorations()
    {
        Decorator.sceneDecorations.Clear();
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Desk));
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Walls));
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Floor2));
        Decorator.sceneDecorations.Add(GetDecorationByStyle(DecorationStyle.Floor1));
        Decorator.DecorateScene();
    }


    private SceneDecoration GetDecorationByStyle(DecorationStyle style)
    {
        return (from e in Decorations where style == e.Style select e.Decoration).FirstOrDefault();
    }


    public void RequestSpaceSetupManual()
    {
        _ = OVRScene.RequestSpaceSetup();
        _ = MRUK.Instance.LoadSceneFromDevice(false);
    }
}
