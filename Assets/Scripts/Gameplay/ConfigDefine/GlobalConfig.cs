using UnityEngine;

[CreateAssetMenu(fileName = "GlobalConfig", menuName = "ConfigData/GlobalConfig", order = 0)]
public class GlobalConfig : ScriptableObject
{
    public SerializableDictionary<EAttrType, AttrConfigItem> attributes = new();
    public AttrConfigItem GetAttrCfg(EAttrType type)
    {
        return attributes[type];
    }
}