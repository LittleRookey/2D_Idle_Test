using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

[CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack/PlayerBasicAttack")]
public class PlayerBasicAttack : BasicAttack
{
    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {


        
        var dmg = allyStat.GetDamageAgainst(target);
        MasterAudio.PlaySound("Į�´¼Ҹ�");
        target.GetComponent<Health>().TakeDamage(allyStat.GetComponent<LevelSystem>(), new List<Damage> { dmg });
        OnApplyEffect?.Invoke();
    }
}
