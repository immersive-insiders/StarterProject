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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ResizableObjectsList", menuName = "Resizable Objects List", order = 1)]
public class ResizableObjectsList : ScriptableObject
{
    public Labels label;

    [System.Serializable]
    public enum Labels
    {
        Generic,
        Table,
        Cabinet,
        Couch,
        Chair,
        Window,
        Door,
        Screen,
        Plant,
        WallArt,
        Lamp,
        Misc,
        Materials,
        None
    }

    public List<FurniturePiece> objects;
    public Dictionary<string, FurniturePiece> book;

    [System.Serializable]
    public class FurniturePiece
    {
        public string objectName = "Object 1";
        public string creator = "Name Surname";
        public string GUID = "";
        public GameObject prefab;
        public Texture2D thumbnail;
    }

    public void PopulateBook()
    {
        if (book != null && book.Count > 0)
        {
            Debug.LogWarning("Book was already populated");
            book.Clear();
        }
        else
        {
            book = new Dictionary<string, FurniturePiece>();
        }

        foreach (FurniturePiece piece in objects)
        {
            book[piece.GUID] = piece;
        }
    }

#if UNITY_EDITOR
    public void OnEnable()
    {
        bool dirty = false;
        foreach (FurniturePiece piece in objects)
        {
            if (piece.GUID == "")
            {
                piece.GUID = Guid.NewGuid().ToString();
                dirty = true;
            }
        }

        // These commands are needed in order for Unity to realize that
        // the scriptable object has changed.
        if (dirty)
        {
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
#endif
}
