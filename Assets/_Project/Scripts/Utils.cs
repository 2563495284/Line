
using DG.Tweening;
using UnityEngine;

public static class Utils
{
    public static void ShakeCamera()
    {
        Camera.main.transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
    }
}