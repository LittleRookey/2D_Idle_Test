

namespace Litkey.InventorySystem
{
    /*
        [상속 구조]
        Item : 기본 아이템
            - EquipmentItem : 장비 아이템
            - CountableItem : 수량이 존재하는 아이템
    */
    [System.Serializable]
    public abstract class Item
    {
        public ItemData Data { get; private set; }
        public string ID => _id; // 유니크 아이디

        private string _id;
        public Item(ItemData data, string id)
        {
            Data = data;
            this._id = id; 
        }


    }


}