
using GameConfig;
public class ChangeAttributeCMD : LevelCommand
{
    public AttrType type;
    public float val;
    public ChangeAttributeCMD(AttrType attributeType, float attributeValue)
    {
        this.type = attributeType;
        this.val = attributeValue;
    }
}