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

#if VT_COURAGE
using System;
using System.Linq;
#endif

namespace VegaTech
{
  public class VTCrewModule : PartModule
  {
    [KSPField]
    public double powerPerKerbal = 0.02;
    #if VT_COURAGE
    [KSPField]
    public float courageGain = 0.0f;
    [KSPField] // The most courageous can last 2 Kerbal days.
    public float courageDrain = 1.0f / 21600.0f / 2.0f;
    [KSPField(guiActive = true, guiName = "Courage", guiUnits = "%")]
    public int courage = 0;
#endif

    public void FixedUpdate()
    {
      int crewCount = part.protoModuleCrew.Count;
      if (crewCount == 0)
        return;

#if VT_COURAGE
      float courageDelta;
      if (vessel.isSituationComfortable())
      {
        courageDelta = VegaTech.COURAGE_GAIN * TimeWarp.deltaTime;
      }
      else
      {
        double amount = crewCount * powerPerKerbal * TimeWarp.deltaTime;
        bool isAccomodated = part.RequestResource(VegaTech.electricChargeId, amount) > amount / 2.0;

        courageDelta = (isAccomodated ? courageGain : -courageDrain) * TimeWarp.fixedDeltaTime;
      }

      if (VegaTech.manageCourage)
      {
        float courageSum = Math.Max(0.0f, part.protoModuleCrew.Sum(k => k.courage) + courageDelta);
        float avgCourage = courageSum / part.protoModuleCrew.Count;

        foreach (ProtoCrewMember kerbal in part.protoModuleCrew)
          kerbal.courage = Math.Min(kerbal.maxCourage(), avgCourage);

        courage = (int) (100.0f * avgCourage);
      }
#else
      if (!vessel.isSituationComfortable())
      {
        double amount = crewCount * powerPerKerbal * TimeWarp.deltaTime;
        part.RequestResource(VegaTech.electricChargeId, amount);
      }
#endif
    }
  }
}
