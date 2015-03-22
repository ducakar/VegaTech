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
using UnityEngine;

namespace VegaTech
{
  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class FlightManager : MonoBehaviour
  {
    void onCrewOnEva(GameEvents.FromToAction<Part, Part> action)
    {
      ProtoCrewMember kerbal = action.to.protoModuleCrew[0];
      Part kerbalPart = action.to;
      Part cabinPart = action.from;

      if (VegaTech.manageCourage && kerbal.courage < VegaTech.evaCourage
          && !cabinPart.vessel.isSituationComfortable())
      {
        kerbalPart.GetComponent<KerbalEVA>().BoardPart(cabinPart);
        cabinPart.CreateInternalModel();
        cabinPart.SpawnCrew();

        ScreenMessages.PostScreenMessage(kerbal.name + " is too scared to go to EVA", 5.0f,
                                         ScreenMessageStyle.UPPER_CENTER);
      }
      else if (VegaTech.takePropellantOnEva)
      {
        PartResource resource = kerbalPart.GetComponent<PartResource>();

        double crewCount = cabinPart.vessel.GetCrewCount();
        double crewReserve = crewCount * VegaTech.personalPropellantReserve;
        double fullPlusReserve = resource.maxAmount + crewReserve;
        double amount = cabinPart.RequestResource(VegaTech.monoPropellantId, fullPlusReserve);

        if (amount > VegaTech.personalPropellantReserve + crewReserve)
        {
          cabinPart.RequestResource(VegaTech.monoPropellantId, -crewReserve);
          resource.amount = amount - crewReserve;
        }
        else
        {
          double personShare = amount / (1.0 + crewCount);

          cabinPart.RequestResource(VegaTech.monoPropellantId, -crewCount * personShare);
          resource.amount = personShare;
        }
      }
    }

    void onCrewBoardVessel(GameEvents.FromToAction<Part, Part> action)
    {
      PartResource resource = action.from.GetComponent<PartResource>();
      if (resource != null)
        action.to.RequestResource(VegaTech.monoPropellantId, -resource.amount);
    }

    void onVesselGoOnRails(Vessel vessel)
    {
      Vector3 angularVelocity = vessel.transform.rotation * vessel.angularVelocity;
      VTScenario.angularVelocities[vessel.id] = angularVelocity;
    }

    void onVesselGoOffRails(Vessel vessel)
    {
      Vector3 angularVelocity;
      if (vessel.loaded && VTScenario.angularVelocities.TryGetValue(vessel.id, out angularVelocity))
      {
        VTScenario.angularVelocities.Remove(vessel.id);

        try
        {
          // Vessels sometimes go "off rails" twice at the beginning of the flight scene. The first
          // time something is not initialised yet obviously, so `vessel.findWorldCenterOfMass()`
          // throws null reference exception.
          Vector3 centreOfMass = vessel.findWorldCenterOfMass();

          foreach (Part p in vessel.parts)
          {
            Rigidbody body = p.rigidbody;
            if (body != null)
            {
              Vector3 partVelocity = Vector3.Cross(angularVelocity, body.position - centreOfMass);

              body.AddTorque(angularVelocity, ForceMode.VelocityChange);
              body.AddForce(partVelocity, ForceMode.VelocityChange);
            }
          }
        }
        catch (NullReferenceException)
        {
          Debug.Log("[VT] vessel.findWorldCenterOfMass() null ex");
        }
      }
    }

    #if false
    public void Update()
    {
      if (VegaTech.preserveRotation && TimeWarp.CurrentRate > 1.0f
          && TimeWarp.WarpMode == TimeWarp.Modes.HIGH)
      {
        Vessel vessel = FlightGlobals.ActiveVessel;
        Vector3 angularVelocity;

        if (vessel != null && !vessel.LandedOrSplashed
            && VTScenario.angularVelocities.TryGetValue(vessel.id, out angularVelocity))
        {
          float angle = angularVelocity.magnitude * Time.deltaTime * TimeWarp.CurrentRate / 2.0f;
          Quaternion deltaRotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, angularVelocity);

          vessel.SetRotation(vessel.GetTransform().rotation * deltaRotation);
        }
      }
    }
    #endif

    public void Awake()
    {
      if (VegaTech.takePropellantOnEva || VegaTech.manageCourage)
        GameEvents.onCrewOnEva.Add(onCrewOnEva);

      if (VegaTech.takePropellantOnEva)
        GameEvents.onCrewBoardVessel.Add(onCrewBoardVessel);

      if (VegaTech.preserveRotation)
      {
        GameEvents.onVesselGoOnRails.Add(onVesselGoOnRails);
        GameEvents.onVesselGoOffRails.Add(onVesselGoOffRails);
      }
    }

    public void OnDestroy()
    {
      if (VegaTech.takePropellantOnEva || VegaTech.manageCourage)
        GameEvents.onCrewOnEva.Remove(onCrewOnEva);

      if (VegaTech.takePropellantOnEva)
        GameEvents.onCrewBoardVessel.Remove(onCrewBoardVessel);

      if (VegaTech.preserveRotation)
      {
        GameEvents.onVesselGoOnRails.Remove(onVesselGoOnRails);
        GameEvents.onVesselGoOffRails.Remove(onVesselGoOffRails);
      }
    }
  }
}
