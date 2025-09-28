using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        Func<IEnumerator> del = Log5;
        // del += Log1;
        this.StartTrackedCoroutine(del.Invoke());
    }
    [TraceableCoroutine("Root")]
    IEnumerator Log1()
    {
        Debug.Log("1");
        yield return new WaitForSeconds(1);
        Debug.Log("2");
        yield return Log2();
        yield return new WaitForSeconds(2);
        Debug.Log("3");
        yield return new WaitForSeconds(1);
        yield return Log3();
        Debug.Log("end");
    }
    [TraceableCoroutine("log2")]
    IEnumerator Log2()
    {
        Debug.Log("2_1");
        yield return new WaitForSeconds(1);
        Debug.Log("2_2");
        yield return Log4();
        yield return new WaitForSeconds(2);
        Debug.Log("2_3");

    }
    [TraceableCoroutine("log3")]
    IEnumerator Log3()
    {
        Debug.Log("3_1");
        yield return new WaitForSeconds(1);
        Debug.Log("3_2");
        yield return new WaitForSeconds(2);
        Debug.Log("3_3");
    }
    [TraceableCoroutine("log4")]
    IEnumerator Log4()
    {
        Debug.Log("4_1");
        yield return new WaitForSeconds(1);
        Debug.Log("4_2");
        yield return new WaitForSeconds(2);
        Debug.Log("4_3");
    }
    [TraceableCoroutine("log5")]
    IEnumerator Log5()
    {
        Debug.Log("5_1");
        yield return new WaitForSeconds(1);
        Debug.Log("5_2");
        yield return new WaitForSeconds(2);
        Debug.Log("5_3");
    }
}
