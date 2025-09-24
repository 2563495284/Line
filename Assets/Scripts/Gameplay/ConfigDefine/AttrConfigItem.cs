using UnityEngine;

[CreateAssetMenu(fileName = "AttrConfigItem", menuName = "ConfigData/AttrConfigItem", order = 0)]
public class AttrConfigItem : ScriptableObject
{

    public EAttrType attributeType;
    public string attributeName;
    public string description;
    public Sprite icon;
    public float originValue;
}