

namespace Litkey.InventorySystem
{
    public interface ICraftable
    {

    }
    /// <summary> ���� ������ - ���� ������ </summary>
    public class CraftItem : CountableItem
    {
        public CraftItem(CraftItemData data, string id, int amount = 1) : base(data, id, amount) { }

        public bool Use()
        {
            // �ӽ� : ���� �ϳ� ����
            //Amount--;

            return true;
        }


    }
}
