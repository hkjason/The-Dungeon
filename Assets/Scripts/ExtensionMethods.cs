using UnityEngine;
public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        if ((to1 - from1) == 0 || (to2 - from2) == 0)
            return 0;
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static bool[] ListOfBool(int size, int avoid)
    {
        bool hasOneTrue = false;
        bool[] arr = new bool[size];
        while (!hasOneTrue)
        { 
            for(int i = 0; i< size; i++)
            {
                if (i == avoid)
                {
                    arr[i] = false;
                    continue;
                }
                int randomNum = Random.Range(0, 2);
                arr[i] = randomNum!=0;
                if (randomNum != 0)
                {
                    hasOneTrue = true;
                }
            }
        }
        return arr;
    }
}