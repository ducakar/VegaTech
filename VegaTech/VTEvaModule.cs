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

namespace VegaTech
{
  public class VTEvaModule : PartModule
  {
    [KSPField(guiActive = true, guiName = "Courage", guiUnits = "%")]
    public int courage = 0;

    public override void OnStart(StartState state)
    {
      if (part.protoModuleCrew.Count == 0)
        return;

      ProtoCrewMember kerbal = part.protoModuleCrew[0];
      if (kerbal.isRobot())
        part.RemoveModule(this);
    }

    public void Update()
    {
      ProtoCrewMember kerbal = part.protoModuleCrew[0];
      if (part.protoModuleCrew.Count == 0)
        return;

      kerbal.courage = vessel.isSituationComfortable() ?
        Math.Min(kerbal.maxCourage(), kerbal.courage + VegaTech.COURAGE_GAIN * TimeWarp.deltaTime) :
        Math.Max(0.0f, kerbal.courage - VegaTech.COURAGE_DRAIN * TimeWarp.deltaTime);

      courage = (int) (100.0f * kerbal.courage);
    }
  }
}

#endif
