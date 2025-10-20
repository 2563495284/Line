using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class EffectBase
{
    public float probablity = 1;
    public int growId = 0;
    public string effectKey = "";
    public string[] args = new string[0];
    public EffectBase(string effectKey, string[] args, float probablity = 1, int growId = 0)
    {
        this.probablity = probablity;
        this.growId = growId;
        this.effectKey = effectKey;
        this.args = args;
    }
    protected IEnumerator ExeCMD(LevelCommand cmd)
    {
        return GM.Ins.Level.AwaitCMD(cmd);
    }
}
public static class EffectFactory
{
    public static List<EffectBase> GetEffects(IReadOnlyList<string> effectStr, EDynamicSerial serial)
    {
        string prefix = serial switch
        {
            EDynamicSerial.CardEffect => "CEffect_",
            EDynamicSerial.PlayerAttrEffect => "PEffect_",
            EDynamicSerial.StockAttrEffect => "SEffect_",
            _ => ""
        };
        if (string.IsNullOrEmpty(prefix))
            return new();
        List<EffectBase> res = new();
        List<EffectBase> grp = new();
        List<float> wei = new();
        for (int i = 0; i < effectStr.Count; i++)
        {
            IReadOnlyList<string> subStr = effectStr[i].Split('*');
            int growId = 0;
            if (subStr.Count > 1)
            {
                growId = int.Parse(subStr[1]);
            }
            subStr = subStr[0].Split('%');
            bool isInGrp = false;
            float grpWeight = 0;
            float probability = 1;
            if (subStr.Count > 1)
            {
                float val = float.Parse(subStr[1]);
                if (val >= 1)
                {
                    isInGrp = true;
                    grpWeight = val;
                }
                else
                    probability = val;
            }
            subStr = subStr[0].Split('/');
            string key = subStr[0];
            string[] args = subStr.Skip(1).ToArray();
            Type type = LoadManager.Ins.GetDynamicClass(serial, prefix + key);
            try
            {
                EffectBase effect = Activator.CreateInstance(type, key, (object)args, probability, growId) as EffectBase;
                if (isInGrp)
                {
                    grp.Add(effect);
                    wei.Add(grpWeight);
                }
                else
                    res.Add(effect);

            }
            catch (System.Exception)
            {
                Debug.Log("No");
            }
        }
        if (grp.Count > 0)
            res.Add(new EffectGroup(grp, wei));
        return res;
    }
}