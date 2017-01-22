using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class Helper {

//	public static T GetItem<T> (List<T> items)
//    {
//        if (items == null || items.Count == 0)
//        {
//            return default(T);
//        }

//        return items[Random.Range(0, items.Count)];
//    }
//}

public static class VectorExtension
{
    // Vector2
    public static Vector2 WithX(this Vector2 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }
    public static Vector2 WithY(this Vector2 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }

    // Vector3
    public static Vector3 WithX(this Vector3 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }
    public static Vector3 WithY(this Vector3 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }
    public static Vector3 WithZ(this Vector3 aVec, float aZValue)
    {
        aVec.z = aZValue;
        return aVec;
    }

    // Vector4
    public static Vector4 WithX(this Vector4 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }
    public static Vector4 WithY(this Vector4 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }
    public static Vector4 WithZ(this Vector4 aVec, float aZValue)
    {
        aVec.z = aZValue;
        return aVec;
    }
    public static Vector4 WithW(this Vector4 aVec, float aWValue)
    {
        aVec.w = aWValue;
        return aVec;
    }
}
