namespace Litkey.InventorySystem
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItemData PortionItemData { get; private set; }

        public PortionItem(PortionItemData data, string id, int amount = 1) : base(data, id, amount) 
        {
            PortionItemData = data;
        }

        public bool Use(PlayerStatContainer playerStat)
        {
            if (Amount <= 0) return false;
            // 임시 : 개수 하나 감소
            Amount -= 1;

            playerStat.GetComponent<PlayerHealth>().AddCurrentHealth(PortionItemData.HealAmount);
            return true;

        }

        
    }
}