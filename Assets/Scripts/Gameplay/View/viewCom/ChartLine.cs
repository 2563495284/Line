using UnityEngine;
public class ChartBridge
{
    public ChartCoord last;
    public ChartCoord next;
    public float Delta => next.price - last.price;
}
public class ChartLine : MonoBehaviour
{
    public ChartBridge info;
    public LineRenderer lineRenderer;
    public void Init(ChartCoord last, ChartCoord next)
    {
        info = new ChartBridge { last = last, next = next };
    }
}