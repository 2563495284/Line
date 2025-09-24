
public class ChangeAttributeCMD : LevelCommand
{
    public EAttrType type;
    public float val;
    public ChangeAttributeCMD(EAttrType attributeType, float attributeValue)
    {
        this.type = attributeType;
        this.val = attributeValue;
    }
}