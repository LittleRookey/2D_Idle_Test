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

        public bool Use()
        {
            if (Amount <= 0) return false;
            // 임시 : 개수 하나 감소
            Amount -= 1;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, ID, amount);
        }


    }
}