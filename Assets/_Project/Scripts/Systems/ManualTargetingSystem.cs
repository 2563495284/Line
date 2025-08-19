using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManualTargetingSystem : Singleton<ManualTargetingSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;

    public void StartTargeting(Vector3 startPosition)
    {
        arrowView.gameObject.SetActive(true);
        arrowView.SetupArrow(startPosition);
    }

    public LineView EndTargeting(Vector3 endPosition)
    {
        arrowView.gameObject.SetActive(false);
        RaycastHit[] hits = Physics.RaycastAll(endPosition, Vector3.forward, 10f, targetLayerMask);
        if (hits.Any(hit => hit.collider != null
            && hit.transform.TryGetComponent(out LineView lineView)))
        {
            return hits.First(hit => hit.collider != null
            && hit.transform.TryGetComponent(out LineView lineView)).transform.GetComponent<LineView>();
        }

        return null;
    }
}