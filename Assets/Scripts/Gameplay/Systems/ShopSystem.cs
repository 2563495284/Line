using System.Collections;

public class ShopSystem : LevelSystem
{
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<FinishShoppingCMD>(FinishShoppingProcessor);
        BindProcessor<ShopRunCMD>(ShopProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<FinishShoppingCMD>();
        UnbindProcessor<ShopRunCMD>();
    }

    bool finishShop = false;
    private IEnumerator FinishShoppingProcessor(FinishShoppingCMD cmd)
    {
        finishShop = true;
        yield break;
    }
    private IEnumerator ShopProcessor(ShopRunCMD cmd)
    {
        yield return Perform(EventConst.EnterShopView);
        finishShop = false;
        while (!finishShop)
            yield return null;

    }
}