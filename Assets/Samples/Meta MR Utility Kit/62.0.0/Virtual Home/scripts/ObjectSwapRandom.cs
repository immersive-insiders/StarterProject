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
using System.Collections.Generic;

// Spawns a random object from the list with the same transform and scale as the owner.
// used for creating variety within the scene rather than replicate the same prefab

public class ObjectSwapRandom : MonoBehaviour
{
    // List of GameObjects to choose from
    public bool Seed = false;
    public List<GameObject> objectsToChooseFrom;
    private GameObject spawnedObject = null; //debugging only, I think. Can be removed

    private GameObject chosenObject;


    public void SpawnRandom()
    {

        // Make sure the list is not empty
        if (objectsToChooseFrom.Count == 0)
        {
            Debug.LogError("List of objects to choose from is empty!");
            return;
        }

        if (Seed == true) //Seed-based randomization, allows you to reproduce the same results in the same room.
        {
            Vector3 position = transform.position;
            float seedNumber = position.x * position.z; //generate a long random float by multiplying the x and z world positions together
            seedNumber = Mathf.Abs(seedNumber); // eliminate the chance of negative float values
            int newInt = Mathf.FloorToInt(seedNumber * 1000f) % 1000; //extract only the last 3 digits of our random value
            float seedFloat = (float)newInt / 1000.0f; // reduce the random int (between 000-999) to a float (betwen .000 and .999)

            int count = objectsToChooseFrom.Count; // total size of the list of prefabs to space
            float multipliedFloat = seedFloat * count; // multiply this new float by the number of entries in the List
            int randomIndex = Mathf.FloorToInt(multipliedFloat); //round downwards, providing an int between list minimum and max
            // Debug.Log(randomIndex);
            chosenObject = objectsToChooseFrom[randomIndex]; //choose the prefab
        }
        else
        {
            // Choose a random object from the list
            int randomIndex = Random.Range(0, objectsToChooseFrom.Count);
            chosenObject = objectsToChooseFrom[randomIndex];
        }


        // Instantiate the chosen GameObject at the same position, rotation, and scale as the owner
        spawnedObject = Instantiate(chosenObject, transform);
        spawnedObject.transform.localPosition = Vector3.zero;
        spawnedObject.transform.localRotation = Quaternion.identity;
        spawnedObject.transform.localScale = Vector3.one;
    }

    // Start function, expectation is that this script is added to a prefab object that is instantiated
    void Start()
    {
        SpawnRandom();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) //debug, swap all object when testing in editor
        {
            Destroy(spawnedObject);
            SpawnRandom();
        }
    }


    // call this to run the randomization again in your scene

    public void SwapWalls()
    {
        Destroy(spawnedObject);
        SpawnRandom();

    }
}
