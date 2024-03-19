using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Town", menuName = "Litkey/Town")]
public class Town : ScriptableObject
{
    [SerializeField] private SpriteRenderer mapMarker;
    public Vector2 townPosition;
    // 마을마다의 상품, 일단 마을에서 뭘 할수 있을지 정하기
    public string townName;
}
