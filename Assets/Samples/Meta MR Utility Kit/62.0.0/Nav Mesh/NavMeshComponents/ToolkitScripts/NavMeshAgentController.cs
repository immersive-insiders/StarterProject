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
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine.AI;

// Navmesh simplifies the job of navigating virtual characters through procedurally created environments.
// By calling 'agent.SetDestination()' and providing a vector3, the agent will discover the best path to the target.
// Object position and rotation will be animated to show movement along a chosen path.

// Additionally, RandomNavPoint can be used for position finding (eg, discover a random floor area in the room for a minigolf hole)

// RoomWander.cs simply discovers a random position (RandomNavPoint) and then tells the agent to move there.
// The interval between changing positions is randomized as well as the characters speed)

public class NavMeshAgentController : MonoBehaviour
{
    private float delayTimer; // interval between random point discovery
    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject positionIndicator;
    private float timer; // used to calculate current timer value
    public bool VisualizeTargetPosition = false;

    void OnEnable()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = delayTimer;
        Transform childTransform = transform.Find("PositionIndicator");
        positionIndicator = childTransform.gameObject;
    }

    // Core loop. Agent will find a new position to move at random intervals and speed.
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delayTimer)
        {
            // set the new set of randomize values for target position and speed
            Vector3 newPos = RandomNavPoint();

            var room = MRUK.Instance?.GetCurrentRoom();
            if (!room)
            {
                return;
            }
            bool test = room.IsPositionInRoom(newPos, false); // occasionally NavMesh will generate areas outside the room, so we must test the value from RandomNavPoint

            if (!test)
            {
                Debug.Log("[NavMeshAgent] [Error]: destination is outside the room bounds, resetting to 0");
                newPos = Vector3.zero;
            }

            if (VisualizeTargetPosition)
            {
                positionIndicator.transform.parent = null;
                positionIndicator.transform.position = newPos;
            }

            agent.SetDestination(newPos);
            float newDelay = Random.Range(2f, 6.0f);
            delayTimer = newDelay;
            float newSpeed = Random.Range(1.2f, 1.6f);
            agent.speed = newSpeed;
            timer = 0;
        }
    }

    // generate a new position on the NavMesh
    public static Vector3 RandomNavPoint()
    {
        // TODO: we can cache this and only update it once the navmesh changes
        var triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();

        if (triangulation.indices.Length == 0)
        {
            return Vector3.zero;
        }

        // Compute the area of each triangle and the total surface area of the navmesh
        float totalArea = 0.0f;
        List<float> areas = new List<float>();
        for (int i = 0; i < triangulation.indices.Length;)
        {
            var i0 = triangulation.indices[i];
            var i1 = triangulation.indices[i + 1];
            var i2 = triangulation.indices[i + 2];
            var v0 = triangulation.vertices[i0];
            var v1 = triangulation.vertices[i1];
            var v2 = triangulation.vertices[i2];
            var cross = Vector3.Cross(v1 - v0, v2 - v0);
            float area = cross.magnitude * 0.5f;
            totalArea += area;
            areas.Add(area);
            i += 3;
        }

        // Pick a random triangle weighted by surface area (triangles with larger surface
        // area have more chance of being chosen)
        var rand = Random.Range(0, totalArea);
        int triangleIndex = 0;
        for (; triangleIndex < areas.Count - 1; ++triangleIndex)
        {
            rand -= areas[triangleIndex];
            if (rand <= 0.0f)
            {
                break;
            }
        }

        {
            // Get the vertices of the chosen triangle
            var i0 = triangulation.indices[triangleIndex * 3];
            var i1 = triangulation.indices[triangleIndex * 3 + 1];
            var i2 = triangulation.indices[triangleIndex * 3 + 2];
            var v0 = triangulation.vertices[i0];
            var v1 = triangulation.vertices[i1];
            var v2 = triangulation.vertices[i2];

            // Calculate a random point on that triangle
            float u = Random.Range(0.0f, 1.0f);
            float v = Random.Range(0.0f, 1.0f);
            if (u + v > 1.0f)
            {
                if (u > v)
                {
                    u = 1.0f - u;
                }
                else
                {
                    v = 1.0f - v;
                }

            }
            return v0 + u * (v1 - v0) + v * (v2 - v0);
        }
    }
}
