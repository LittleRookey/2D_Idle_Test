using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MapManager : MonoBehaviour
{
    private Vector2 currentPosition;

    public Transform playerMarker;

    public float totalMapSize;
    [SerializeField] private float markerMovePerMeter;
    public float walkPerSec;// 1초에 가는 거리
    public Vector2 startPos; // 시작점 
    public Vector2 moveDir;
    private Vector2 Destination;
    public float step; // 한번의 발걸음 거리, 1m = 1
    public float leftDist; // 목적지까지 남은 거리
    public Vector2 firstDestination;
    public TextMeshProUGUI currentPosText;
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = startPos;
        currentPosText.SetText($"{currentPosition.x.ToString("F0")}, {currentPosition.y.ToString("F0")}");
        SetDestination(firstDestination);

    }

    public void SetDestination(Vector2 destPos)
    {
        moveDir = (destPos - currentPosition).normalized;
        Destination = destPos;
        leftDist = (Destination - currentPosition).sqrMagnitude * step;
        StartCoroutine(MoveTo());
    }

    private IEnumerator MoveTo()
    {
        yield return new WaitForSeconds(1f);
        var movedDist = moveDir * walkPerSec;
        //DOTween.To(() => damaged.fillAmount, x => damaged.fillAmount = x, current / max, 0.2f);
        playerMarker.DOLocalMove(moveDir * markerMovePerMeter * walkPerSec, 1f).SetEase(Ease.Linear).SetRelative(true);
        currentPosition += movedDist;
        currentPosText.SetText($"{currentPosition.x.ToString("F0")}, {currentPosition.y.ToString("F0")}");
        leftDist = (Destination - currentPosition).sqrMagnitude * step; // need help here
        leftDist -= movedDist.sqrMagnitude;
        if (leftDist < 0)
        {
            leftDist = 0f;
            currentPosition = Destination;
            currentPosText.SetText($"{currentPosition.x.ToString("F0")}, {currentPosition.y.ToString("F0")}");
            yield break;
        }
        StartCoroutine(MoveTo());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
