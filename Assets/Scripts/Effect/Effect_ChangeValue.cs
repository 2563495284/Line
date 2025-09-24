using System;
using System.Collections;
using UnityEngine;
public enum EffectValueType
{
    Stock,
    Attribute,
    Mana,
    Money,
}
[Serializable]
public class ValueChangeData
{
    public int id = 0;
    public float value = 0;
}
public class Effect_ChangeValue : Effect
{
    public SerializableDictionary<EffectValueType, ValueChangeData> valuesMap = new();
    public override IEnumerator Run(IEffectEmitter from)
    {
        for (int i = 0; i < valuesMap.Count; i++)
        {
            ValueChangeData data = valuesMap.values[i];
            switch (valuesMap.keys[i])
            {
                case EffectValueType.Stock:
                    yield return ExeCMD(new ChangeStockCMD((int)data.value, (EStockType)data.id));
                    break;
                case EffectValueType.Attribute:
                    yield return ExeCMD(new ChangeAttributeCMD((EAttrType)data.id, data.value));
                    break;
                case EffectValueType.Mana:
                    yield return ExeCMD(new ChangeManaCMD((int)data.value));
                    break;
                case EffectValueType.Money:
                    yield return ExeCMD(new ChangeMoneyCMD((int)data.value));
                    break;
            }
        }
    }
}