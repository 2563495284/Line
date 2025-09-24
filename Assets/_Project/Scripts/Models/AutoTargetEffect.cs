using SerializeReferenceEditor;
using UnityEngine;

[System.Serializable]
public class AutoTargetEffect
{
    [field: SerializeReference, SR] public TargetMode TargetMode { get; private set; } = new NoTM();
    [field: SerializeReference, SR] public Effect2 Effect { get; private set; }
}