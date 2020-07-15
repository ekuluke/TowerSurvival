using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public static class SingleExtensions
{
    public static float RoundToScale(this float num)
    {

        var scaleRange = Enumerable.Range(0, 6).Select(x => (x * 2f) * 0.1f).ToArray();
        float minDistance = 0f; //0 is fine here it is never read, it is just to make the compiler happy.
        int minIndex = -1;
        float dec = Math.Abs(num - (float)Math.Truncate(num));
        // Debug.Log("dist(dec): " + dec);
        float wholeNumber = (float)Math.Truncate(num);


        for (int i = 0; i < scaleRange.Length; i++)
        {
            var distance = Math.Abs(dec - scaleRange[i]);
            if (minIndex == -1 || (distance < minDistance))
            {
                minDistance = distance;
                minIndex = i;

                //Optional, stop testing if we find a exact match.
                if (minDistance == 0)
                    break;
            }
        }
        // print("whole: " + wholeNumber + "dec: " + scaleRange[minIndex]);
        if (num < 0)
        {
            return wholeNumber - scaleRange[minIndex];
        }
        else
        {


            return wholeNumber + scaleRange[minIndex];
        }

    }
}
