using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer lineRenderer;

    private Vector3 startPosition;
    public void SetWorldPos(Vector3 wp)
    {
        Vector3 direction = -(startPosition - arrowHead.transform.position).normalized;
        lineRenderer.SetPosition(1, wp - direction * 0.5f);
        arrowHead.transform.position = wp;
        arrowHead.transform.right = direction;

    }

    public void Show(Vector3 startPosition)
    {
        gameObject.SetActive(true);
        this.startPosition = startPosition;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, MouseUtils.GetMouseWp());
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}