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

using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SelectableText : MonoBehaviour
{
    [SerializeField] private bool SelectedOnToggleOn;
    private TextMeshProUGUI selectedText;
    private Color selectedColor = Color.white;
    private Color deselectedColor = new Color(0.3f,0.3f,0.3f);

    void Awake()
    {
        selectedText = GetComponent<TextMeshProUGUI>();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            if (SelectedOnToggleOn)
            {
                selectedText.color = selectedColor;
            }
            else
            {
                selectedText.color = deselectedColor;
            }
        }
        else
        {
            if (!SelectedOnToggleOn)
            {
                selectedText.color = selectedColor;
            }
            else
            {
                selectedText.color = deselectedColor;
            }
        }
    }
}
