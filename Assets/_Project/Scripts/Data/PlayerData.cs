using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
public class PlayerData : CharacterData
{
    [field: SerializeField] public float doTweenScaleDuration { get; private set; }
    [field: SerializeField] public float doTweenMoveDuration { get; private set; }

    [SerializeField] public SerializableDictionary<EStrategyType, float> changePricePersentDictionaryWhenBuy;
    [SerializeField] public SerializableDictionary<EStrategyType, float> changePriceDictionaryWhenBuy;
    [SerializeField] public SerializableDictionary<EStrategyType, float> changePricePersentDictionaryWhenSell;
    [SerializeField] public SerializableDictionary<EStrategyType, float> changePriceDictionaryWhenSell;
}