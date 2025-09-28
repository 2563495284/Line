using GameConfig;

public class ChangeAttributeGA : GameAction
{
    public AttrType attributeType;
    public float attributeValue;
    public ChangeAttributeGA(AttrType attributeType, float attributeValue)
    {
        this.attributeType = attributeType;
        this.attributeValue = attributeValue;
    }
}