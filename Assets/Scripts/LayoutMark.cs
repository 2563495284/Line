using System.Collections.Generic;
using UnityEngine;
public enum ELayoutDir
{
    LeftToRight,
    RightToLeft,
}
public class LayoutMark : MonoBehaviour
{
    public float width = 1;
    public float height = 1;
    public Color color = new Color(255, 169, 169, 90);
    public ELayoutDir layoutDir = ELayoutDir.LeftToRight;
    public SidenoteLayoutInfo GetSidenoteLayoutInfo()
    {
        return new SidenoteLayoutInfo()
        {
            pos = transform.position,
            heightLimit = height,
            layoutDir = layoutDir
        };
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        if (layoutDir == ELayoutDir.LeftToRight)
        {
            Vector3[] pointList = new Vector3[]
            {
                transform.position+new Vector3(width,0),
                transform.position +new Vector3(width,-height),
                transform.position +new Vector3(width,-height),
                transform.position +new Vector3(0,-height),
                transform.position +new Vector3(0,-height),
                transform.position+new Vector3(0,0),
                transform.position+new Vector3(0,0),
                transform.position+new Vector3(width,0),
            };
            Gizmos.DrawLineList(pointList);
        }
        else
        {
            Vector3[] pointList = new Vector3[]
            {
                transform.position+new Vector3(-width,0),
                transform.position +new Vector3(0,0),
                transform.position +new Vector3(0,0),
                transform.position +new Vector3(0,-height),
                transform.position +new Vector3(0,-height),
                transform.position+new Vector3(-width,-height),
                transform.position+new Vector3(-width,-height),
                transform.position+new Vector3(-width,0),
            };
            Gizmos.DrawLineList(pointList);
        }
    }
}