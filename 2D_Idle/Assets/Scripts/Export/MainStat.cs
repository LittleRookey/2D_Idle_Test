using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Litkey.InventorySystem;


namespace Litkey.Stat
{

    public struct Damage
    {
        public float damage;
        public bool isCrit;
        public Damage(float dmg, bool crit)
        {
            damage = dmg;
            isCrit = crit;
        }
    }

    [System.Serializable]  
    public abstract class MainStat
    {

        public string StatName; // ���� ID
        [HideInInspector] public int BaseStat;

        //private int _addedStat;

        //  ���ν����� �÷��ִ� ���꽺�ݵ�
        private List<SubStat> substats;

        private int _levelAddedStats;


        [HideInInspector] public UnityEvent OnValueChanged;
        public float Value => FinalValue;


        public int FinalValue
        {
            get
            {
                return BaseStat + _levelAddedStats;
            }
            set
            {
                int origin = FinalValue;
                if (origin != value)
                    OnValueChanged?.Invoke();
            }
        }
        public MainStat(string statName, int value)
        {
            this.StatName = statName;
            this.BaseStat = value;
            this._levelAddedStats = 0;
            this.substats = new List<SubStat>();
            //OnValueChanged.AddListener(() => Debug.Log($"{StatName} : {}"));
        }

        // Increases main stat by 1
        public void IncreaseStat(int addedVal=1)
        {
            if (addedVal <= 0) return;
            _levelAddedStats += addedVal;
            //Debug.Log($"{StatName} increased by {addedVal} from {FinalValue - addedVal} to {FinalValue}");
            //  TODO apply substat based on full stat
            // apply mainstat changes to substat, 
            // each substat will apply changes to each mainstat that belongs to
            UpdateValue();
        }

        
        

        // apply mainstat changes to substat, 
        // each substat will apply changes to each mainstat that belongs to
        private void UpdateValue()
        {
            foreach (SubStat stat in substats)
            {
                stat.ApplyChange(this, FinalValue);
            }
        }

        public void AddSubStatAsChild(SubStat ss)
        {
            if (substats.Contains(ss)) return;
            substats.Add(ss);
            UpdateValue();
        }

        public int GetFinalValue()
        {
            return FinalValue;
        }

        public override string ToString()
        {
            return StatName;
        }

        public void DisplayAllSubstats()
        {
            string s = "";
            foreach (SubStat ss in substats)
            {
                s+= $"{ss.ToString()}: {ss.GetFinalValue()}\n";
            }
            Debug.Log(s);
        }

    }

    [System.Serializable]
    public class SubStat
    {
        // ���ν��� Influencer���� ���꽺�ݵ鿡 ���ϴ� ����

        //public float Value => _finalValue;

        [SerializeField] 
        private string displayName;

        public eSubStatType statType;

        private float _finalValue;
        
        public float FinalValue
        {
            get
            {
                UpdateFinalValue();
                return _finalValue;
            }
            set
            {
                float origin = _finalValue;
                if (origin != value)
                    OnValueChanged?.Invoke();
            }
        }


        private bool _isPercentage; // is substat shown as percantage

        private string shownFinalValue => _isPercentage ? _finalValue.ToString() + percantage : _finalValue.ToString();
        
        private List<Influencer> influencers; // mainstats that affect substat

        string percantage = "%";

        // �ʱ� ĳ���� ���� ��
        private float BaseStat;

        // ������ ���Է� ���� ���� ��
        // ( ���(+) + ����(+) + ����(+) )
        private float _plusValue
        {
            get
            {
                return _plusStatValue + _plusEquipValue + _plusBuffValue;
            }
        } 
        // ������ ���� ���� ���� ��
        // ( ���(*) + ����(*) + ����(*) )
        private float _multipliedValue
        {
            get
            {
                return _multipliedStatValue + _multipliedEquipValue + _multipliedBuffValue;
            }
        } 

        // ����, ��� 
        private List<StatModifier> buffStats;
        public Dictionary<string, List<StatModifier>> equipStats { get; private set; }

        private float _plusStatValue; // ���� �������� �߰��� + ����
        private float _plusEquipValue; // ��� �������� �߰��� + ����
        private float _plusBuffValue; // ������������ �߰��� + ����

        private float _multipliedStatValue; // ���� �������� �߰��� * ����
        private float _multipliedEquipValue; // ��� �������� �߰��� * ����
        private float _multipliedBuffValue; // ���� �������� �߰��� * ����

        [HideInInspector] public UnityEvent OnValueChanged;

        // Constructor that has initial value
        public SubStat(string displayName, float initValue, eSubStatType statType, bool isPercantage = false)
        {

            this._isPercentage = isPercantage;
            this.BaseStat = initValue;
            this._finalValue = initValue;

            this._plusStatValue = 0;
            this._plusEquipValue = 0;
            this._plusBuffValue = 0;

            this._multipliedStatValue = 0;
            this._multipliedEquipValue = 0;
            this._multipliedBuffValue = 0;

            this.displayName = displayName;
            this.statType = statType;
            this.equipStats = new Dictionary<string, List<StatModifier>>();
            influencers = new List<Influencer>();

            buffStats = new List<StatModifier>();
            //equipStats = new List<StatModifier>();

        }
        /// <summary>
        /// �ϳ��� ������ �����Ų��, ������ ������ �� �������� ������ �д�
        /// </summary>
        /// <param name="equipmentID">��� ���� ���̵�</param>
        /// <param name="stat">����� ���� �ϳ�</param>
        public void AddEquipValue(string equipmentID, StatModifier stat)
        {
            if (!equipStats.ContainsKey(equipmentID))
            {
                equipStats[equipmentID] = new List<StatModifier>();
            }

            equipStats[equipmentID].Add(stat);

            if (stat.oper == OperatorType.plus) _plusEquipValue += stat.value;
            else if (stat.oper == OperatorType.subtract) _plusEquipValue -= stat.value;
            else if (stat.oper == OperatorType.multiply) _multipliedEquipValue += stat.value;
            else if (stat.oper == OperatorType.divide) _multipliedEquipValue -= stat.value;
        }
        

        // �� ����� ��� ������ �����ϱ�
        public void UnEquipValue(string equipmentID, StatModifier stat)
        {
            if (equipStats[equipmentID].Remove(stat))
            {
                if (stat.oper == OperatorType.plus) _plusEquipValue -= stat.value;
                else if (stat.oper == OperatorType.subtract) _plusEquipValue += stat.value;
                else if (stat.oper == OperatorType.multiply) _multipliedEquipValue -= stat.value;
                else if (stat.oper == OperatorType.divide) _multipliedEquipValue += stat.value;
            }
        }


        public void AddBuffValue(StatModifier statModifier)
        {
            buffStats.Add(statModifier);
            //if (oper == BuffOperator.Add)
            //{
            //    _addedValue += addedVal;

            //}  else if (oper == BuffOperator.Multiply)
            //{
            //    _multipliedValue += addedVal;
            //} 
            UpdateFinalValue();
        }
        
        public void RemoveBuffValue(StatModifier statModifier)
        {
            buffStats.Remove(statModifier);

            UpdateFinalValue();
        }
        // Required to use when creating substats
        public void AddAsInfluencer(StatPerValue spv)
        {
            influencers.Add(new Influencer(
                spv.baseMainStat, 
                spv.baseMainStat.GetFinalValue(), 
                spv.perStat, 
                spv.increasedStat));
            UpdateFinalValue();
        }

        // if one of the main stat values changed, find that main stat and apply changes to that influencer
        public void ApplyChange(MainStat mainStat, int changedMainStatValue)
        {
            // substat can't have repetitive main stat in influencer
            Influencer inf = influencers.Find((Influencer inf) => inf._mainStat.ToString() == mainStat.ToString());
            // apply the changed substat value
            // + ���ݰ��� ����
            _plusStatValue = inf.ApplyChange(changedMainStatValue);
            // update final value based on change
            UpdateFinalValue();
        }

        public void AddStatValue(int value)
        {
            _plusStatValue += value;
            UpdateFinalValue();
        }

        private float UpdateFinalValue()
        {
            // �ʱ� ����
            _finalValue = this.BaseStat;

            // ���� ���� ��
            //foreach(Influencer inf in influencers)
            //{
            //    _plusStatValue += inf.GetFinalValue();
            //}

            _finalValue += _plusValue;
            _finalValue *= (1f + _multipliedValue);
            return _finalValue;
        }

        public float GetAddedStatValue()
        {
            float val = 0;
            foreach (Influencer inf in influencers)
            {
                val += inf.GetFinalValue();
            }
            return val;
        }

        // ���ݷ� ���� ����
        // �⺻ ���ݷ� * (1+ ���� ���ݷ�) + ���� ���ݷ�
        // ���� ���ݷ� = (���� �߰� ���ݷ� + ��� ���ݷ�)
        public float GetFinalValue()
        {
            float baseStat = this.BaseStat * (1 + _multipliedValue) + _plusValue;
            return _finalValue;
        }

        public override string ToString()
        {
            return displayName;
        }

        // ���ν��� x�� �ش� ���꽺���� �߰����� �����´�
        public float GetFutureStat(MainStat mainStat, int addedMainStatValue)
        {
            float addedValue = 0f;
            // substat can't have repetitive main stat in influencer
            Influencer inf = influencers.Find((Influencer inf) => inf._mainStat.ToString() == mainStat.ToString());
            // apply the changed substat value
            addedValue = inf.GetPreviewValue(addedMainStatValue);
            // update final value based on change
            return addedValue;
        }
    }
    
    public enum eRarity
    {
        F, FF, FFF, E, EE, EEE, D, DD, DDD, C, CC, CCC, B, BB, BBB, A, AA, AAA, S, SS, SSS, EX   
    }


    // class used for substat to apply changes to mainstats
    public class Influencer
    {
        public MainStat _mainStat { get; }
        private int _perMainStat;
        private float _increaseValue; // increase value per increased main stat of substat, ex) VIT 1 �� 100 ü��

        private int currentMainStat; // actual value of main stat assigned

        private float finalValue; // value used to apply changes to mainStat 
        public Influencer(MainStat mainStat, int currentMainStatValue, int perMainStat, float increaseValue)
        {
            _mainStat = mainStat;
            _perMainStat = perMainStat;
            _increaseValue = increaseValue;
            currentMainStat = currentMainStatValue;
            finalValue = 0f;
        }

        /// <summary>
        /// �ش� ������ ��� ������ ���ݰ��� ����
        /// </summary>
        /// <param name="mainStatValue"></param>
        /// <returns></returns>
        public float ApplyChange(int mainStatValue)
        {
            currentMainStat = mainStatValue;
            finalValue = (currentMainStat / _perMainStat) * _increaseValue;
            return finalValue;
        }

        // ������Ʈ ���� �ʰ� ���� �������� 
        public float GetPreviewValue(int addedMainStatValue)
        {
            float result = (addedMainStatValue / _perMainStat) * _increaseValue;
            return result;
        }

        public float GetFinalValue()
        {
            finalValue = (currentMainStat / _perMainStat) * _increaseValue;
            return finalValue;
        }


    }



    public struct StatPerValue
    {
        public MainStat baseMainStat;
        public int perStat;
        public float increasedStat;
        public StatPerValue(MainStat baseMainStat, int perStat, float increasedStat)
        {
            this.baseMainStat = baseMainStat;
            this.perStat = perStat;
            this.increasedStat = increasedStat;
        }
    }

    public class StatUtility
    {
        
        public static StatPerValue StatPerValue(MainStat mainStat, int perStat, float increaseValue)
        {
            return new StatPerValue(mainStat, perStat, increaseValue);
        }

        public static eSubStatType GetRandomStat()
        {
            int totalLength = System.Enum.GetValues((typeof(eSubStatType))).Length;

            int randNum = Random.Range(0, totalLength);
            return (eSubStatType)randNum;
        }
    }

    [System.Serializable]
    public class StatModifier
    {
        public eSubStatType statType;
        public float value;
        public OperatorType oper;

        public bool Compare(StatModifier stat)
        {
            return stat.statType == this.statType
                && stat.value == this.value
                && stat.oper == this.oper;
        }

        public bool IsStatType(eSubStatType statType)
        {
            return this.statType == statType;
        }

        public bool IsStatType(SubStat subStat)
        {
            return this.statType == subStat.statType;
        }
    }

}
public enum eMainStatType
{
    Str,
    Vit,
    Dex,
    Int,
    Wis
};

public enum eSubStatType
{
    health,
    mana,
    healthRegen,
    manaRegen,
    attack,
    critChance,
    defense,
    penetration,
    attackSpeed,
    cc_Resistance,
    moveSpeed,
    attackRange,
    critDamage,
    �߰����,
    �߰�����ġ
};
