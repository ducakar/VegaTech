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

using UnityEngine;

namespace VegaTech
{
  [KSPAddon(KSPAddon.Startup.MainMenu, true)]
  public class VegaTech : MonoBehaviour
  {
    // 10 Kerbal days to fully refill courage.
    public const float COURAGE_GAIN = 1.0f / 21600.0f / 10.0f;
    // The most courageous can last 1 Kerbal day.
    public const float COURAGE_DRAIN = 1.0f / 21600.0f;

    public static bool preserveRotation = true;
    public static bool takePropellantOnEva = true;
    public static float personalPropellantReserve = 1.00f;
    public static bool manageCourage = true;
    public static float evaCourage = 0.30f;

    public static int electricChargeId;
    public static int monoPropellantId;

    public void Awake()
    {
      electricChargeId = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;
      monoPropellantId = PartResourceLibrary.Instance.GetDefinition("MonoPropellant").id;

      ConfigNode config = GameDatabase.Instance.GetConfigNode("VegaTech");
      if (config != null)
      {
        Util.parse(config.GetValue("preserveRotation"), ref preserveRotation);
        Util.parse(config.GetValue("takePropellantOnEva"), ref takePropellantOnEva);
        Util.parse(config.GetValue("personalPropellantReserve"), ref personalPropellantReserve);
        Util.parse(config.GetValue("manageCourage"), ref manageCourage);
        Util.parse(config.GetValue("evaCourage"), ref evaCourage);
      }

      #if VT_COURAGE
      if (manageCourage)
      {
        foreach (AvailablePart part in PartLoader.LoadedPartsList)
        {
          if (part.name == "kerbalEVA" && part.partPrefab.GetComponent<VTEvaModule>() == null)
            part.partPrefab.gameObject.AddComponent<VTEvaModule>();
        }
      }
      #endif
    }
  }
}
