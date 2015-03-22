/*
 * Copyright © 2013-2015 Davorin Učakar
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

//using System;
//using System.Diagnostics;

namespace VegaTech
{
  static class Util
  {
    //    public static Random random = new Random();

    /**
     * Print a log entry for TextureReplacer. `String.Format()`-style formatting is supported.
     */
    //    public static void log(string s, params object[] args)
    //    {
    //      Type callerClass = new StackTrace(1, false).GetFrame(0).GetMethod().DeclaringType;
    //      UnityEngine.Debug.Log("[VT." + callerClass.Name + "] " + String.Format(s, args));
    //    }

    public static void parse(string name, ref bool variable)
    {
      bool value;
      if (bool.TryParse(name, out value))
        variable = value;
    }

    //    public static void parse(string name, ref int variable)
    //    {
    //      int value;
    //      if (int.TryParse(name, out value))
    //        variable = value;
    //    }

    public static void parse(string name, ref float variable)
    {
      float value;
      if (float.TryParse(name, out value))
        variable = value;
    }

    //    public static void parse<E>(string name, ref E variable)
    //    {
    //      try
    //      {
    //        variable = (E) Enum.Parse(typeof(E), name, true);
    //      }
    //      catch (ArgumentException)
    //      {
    //      }
    //      catch (OverflowException)
    //      {
    //      }
    //    }

    /**
     * Whether vessel is in a situation, so Kerbals don't need accomodation.
     */
    public static bool isSituationComfortable(this Vessel vessel)
    {
      return vessel.IsRecoverable;
    }

    #if VT_COURAGE
    public static float maxCourage(this ProtoCrewMember kerbal)
    {
      return kerbal.experienceLevel * 0.2f;
    }

    /**
     * Whether a Kerbal is a robot (that is iff its name ends with a digit).
     */
    public static bool isRobot(this ProtoCrewMember kerbal)
    {
      return kerbal.name.Length != 0 && char.IsDigit(kerbal.name, kerbal.name.Length - 1);
    }
    #endif
  }
}
