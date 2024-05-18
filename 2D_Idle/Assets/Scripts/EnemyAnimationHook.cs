using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAnimationHook : MonoBehaviour
{
    private Dictionary<string, List<Tween>> animationsDictionary = new Dictionary<string, List<Tween>>();
    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        LoadAnimations();
    }

    private void LoadAnimations()
    {
        // Get all DOTweenAnimation components in the GameObject
        DOTweenAnimation[] animations = GetComponents<DOTweenAnimation>();

        foreach (DOTweenAnimation animation in animations)
        {
            // Get all DOTween animations from the DOTweenAnimation component
            var tweens = animation.GetTweens();

            foreach (Tween tween in tweens)
            {
                // Check if the animation has an ID
                if (!string.IsNullOrEmpty(tween.stringId))
                {
                    // If the dictionary doesn't contain the ID, initialize a new list
                    if (!animationsDictionary.ContainsKey(tween.stringId))
                        animationsDictionary[tween.stringId] = new List<Tween>();

                    // Add the animation to the list for the corresponding ID
                    animationsDictionary[tween.stringId].Add(tween);
                }
            }
        }
        foreach (var am in  animationsDictionary.Keys)
        {

            Debug.Log("anim count key " + am +" Count: " + animationsDictionary[am].Count);
        }
    }

    private void OnEnable()
    {
        enemyAI.OnIdle.AddListener(Idle);
        enemyAI.OnHit.AddListener(Hit);
        enemyAI.OnAttack.AddListener(RandAttack);
        enemyAI.OnDead.AddListener(Dead);
    }

    private void OnDisable()
    {
        enemyAI.OnIdle.RemoveListener(Idle);
        enemyAI.OnHit.RemoveListener(Hit);
        enemyAI.OnAttack.RemoveListener(RandAttack);
        enemyAI.OnDead.RemoveListener(Dead);
    }

    public void Idle(Health targ)
    {
        if (animationsDictionary.ContainsKey("Idle"))
            RestartTweens(animationsDictionary["Idle"]);
    }

    public void Hit(Health targ)
    {
        if (animationsDictionary.ContainsKey("Hit"))
            RestartTweens(animationsDictionary["Hit"]);
    }

    private void RandAttack(Health targ)
    {
        var randNum = Random.Range(0f, 2f);
        if (randNum >= 1f) JumpAttack(targ);
        else FrontAttack(targ);
    }

    private void JumpAttack(Health target)
    {
        if (animationsDictionary.ContainsKey("JumpAttack"))
            RestartTweens(animationsDictionary["JumpAttack"]);
    }

    private void FrontAttack(Health target)
    {
        if (animationsDictionary.ContainsKey("FrontAttack"))
            RestartTweens(animationsDictionary["FrontAttack"]);
    }

    private void Dead(Health targ)
    {
        if (animationsDictionary.ContainsKey("Dead"))
            RestartTweens(animationsDictionary["Dead"]);
    }

    private void RestartTweens(List<Tween> tweens)
    {
        foreach (Tween tween in tweens)
            tween.Restart();
    }
}