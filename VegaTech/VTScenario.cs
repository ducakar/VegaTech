/*
 * Copyright © 2015 Davorin Učakar
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VegaTech
{
  [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.FLIGHT)]
  public class VTScenario : ScenarioModule
  {
    public static Dictionary<Guid, Vector3> angularVelocities = new Dictionary<Guid, Vector3>();

    public override void OnLoad(ConfigNode node)
    {
      angularVelocities.Clear();

      node = node.GetNode("AngularVelocities");
      if (node == null)
        return;

      foreach (ConfigNode.Value value in node.values)
        angularVelocities.Add(new Guid(value.name), ConfigNode.ParseVector3(value.value));
    }

    public override void OnSave(ConfigNode node)
    {
      node = node.AddNode("AngularVelocities");

      foreach (Vessel vessel in FlightGlobals.Vessels)
      {
        Vector3 angularVelocity;
        if (!angularVelocities.TryGetValue(vessel.id, out angularVelocity))
          angularVelocity = vessel.transform.rotation * vessel.angularVelocity;

        node.AddValue(vessel.id.ToString(), ConfigNode.WriteVector(angularVelocity));
      }
    }
  }
}
