public enum EStockAttrApplyTiming
{
    RoundStart,         //回合开始时(三维修改)
    BeforeSettlement,   //计算股价前(业绩增幅与缩放)
    AfterSettlement     //计算股价后(属性成长)
}
public class ApplyStockAttrCMD : LevelCommand
{
    public EStockAttrApplyTiming timing;
    public ApplyStockAttrCMD(EStockAttrApplyTiming timing)
    {
        this.timing = timing;
    }
}