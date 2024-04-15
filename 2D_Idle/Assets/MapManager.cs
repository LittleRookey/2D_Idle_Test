using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using Litkey.Utility;
using Sirenix.OdinInspector;

public class MapManager : MonoBehaviour
{
    private Vector2 currentPosition;
    [Header("Map")]
    public RectTransform playerMarker;
    public RectTransform destinationImage;

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
    public TextMeshProUGUI zoomInText;
    public int zoomIn = 5;
    [Header("Camera Movement")]
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform actualMap;
    [SerializeField] private RectTransform screenMap;
    [SerializeField] private int maxZoomMod = 5;
    [SerializeField] private int minZoomMod = 1;
    private Vector3 dragOrigin;
    public static float mapMinX, mapMaxX, mapMinY, mapMaxY;
    private float mapWidth, mapHeight;

    [Header("Map etc")]
    [SerializeField] private DOTweenAnimation mapTitleBG;
    [SerializeField] private TextMeshProUGUI mapTitleText;
    [SerializeField] private Transform areasParent;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Castle")]
    [SerializeField] private Transform Castle;
    [SerializeField] private Transform castleSpawnPosition;

    //public static float minX
    private bool canScroll = true;
    
    private readonly string townPath = "ScriptableObject/Town/";

    public UnityAction OnArriveDestination;

    private eCountry currentCountry;
    private eRegion currentRegion;
    private Area currentArea;
    public Area CurrentArea => currentArea;
    public eRegion CurrentRegion => currentRegion;
    [SerializeField] private Area startArea;
    private void Awake()
    {
        mapWidth = screenMap.rect.width/2;
        mapHeight = screenMap.rect.height/2;

        mapMinX = 0f - (step * mapWidth);
        mapMaxX = (step * mapWidth);
        mapMinY = 0f - (step * mapHeight);
        mapMaxY = (step * mapHeight);
        destinationImage.gameObject.SetActive(false);

        Debug.Log($"X: {mapMaxX}\nY:{mapMaxY}");


        currentPosition = startPos;
        actualMap.anchoredPosition = currentPosition;
        playerMarker.anchoredPosition = currentPosition;
        actualMap.rect.Set(actualMap.anchoredPosition.x, actualMap.anchoredPosition.y, screenMap.rect.width, screenMap.rect.height);

        currentPosText.SetText($"{currentPosition.x.ToString("F0")}, {currentPosition.y.ToString("F0")}");
        

        zoomIn = 1;
        zoomInText.SetText($"x{zoomIn}");
        DOTween.To(() => actualMap.transform.localScale, x => actualMap.transform.localScale = x, Vector3.one * zoomIn, 0.2f);

        var towns = Resources.LoadAll<Town>(townPath);

        // 맵에 마을 마커 달아주기 
        for (int i = 0; i < towns.Length; i++)
        {

        }

        var areas = areasParent.GetComponentsInChildren<Area>();
        foreach(var area in areas)
        {
            area.OnPlayerEnterArea += DisplayAreaUI;
        }

        
    }

    private void OnEnable()
    {
        currentArea = startArea;
    }
    public void EnterTown()
    {
        // 성문 스폰
        Castle.gameObject.SetActive(true);
        Castle.transform.position = castleSpawnPosition.position;
        Castle.parent = null;
        // 플레이어 걷기
        player.DOSmoothWalk();
    }

    public void DisplayAreaUI(Area area)
    {
        if (area.region == eRegion.Town)
        {
            SpawnManager.StopTimer();
            // show map enter
            //player.SmoothWalk();
            //EnterTown();
            return;
        }
        if (area.region == currentArea.region)
        {
            return;
        }
        SpawnManager.StartTimer();

        currentArea = area;
        currentRegion = area.region;
        mapTitleBG.GetComponent<Image>().color = Color.white;
        mapTitleText.color = Color.white;
        mapTitleBG.gameObject.SetActive(true);
        mapTitleBG.DORestartById("FadeIn");
        mapTitleText.SetText(area.areaName);
        mapTitleText.GetComponent<DOTweenAnimation>().DORestartById("FadeIn");
        Debug.Log("Entered display UI: " + area.areaName);
        //StartCoroutine(DisplayOff());
    }

    private IEnumerator DisplayOff()
    {
        yield return new WaitForSeconds(4f);
        mapTitleBG.DORestartById("FadeOut");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void ZoomIn()
    {
        if (zoomIn + 1 > maxZoomMod) zoomIn = 5;
        else zoomIn += 1;

        if (zoomIn > 1)
        {
            canScroll = true;
            //actualMap.anchoredPosition *= zoomIn;
            //if (actualMap.anchoredPosition.x >0)
            //{
            //    actualMap.anchoredPosition += (Vector2.right * (mapWidth * (zoomIn - 1)));
            //} else if (actualMap.anchoredPosition.x < 0)
            //{
            //    actualMap.anchoredPosition += (Vector2.left * (mapWidth * (zoomIn - 1)));
            //}
            //if (actualMap.anchoredPosition.y > 0)
            //{
            //    actualMap.anchoredPosition += (Vector2.up * (mapHeight * (zoomIn - 1)));
            //} else if (actualMap.anchoredPosition.y < 0)
            //{
            //    actualMap.anchoredPosition += (Vector2.down * (mapHeight * (zoomIn - 1)));
            //}
        }
        zoomInText.SetText($"x{zoomIn}");
        // change position based on scale
        //actualMap.transform.localScale = Vector3.one * zoomIn;
        DOTween.To(() => actualMap.transform.localScale, x => actualMap.transform.localScale = x, Vector3.one * zoomIn, 0.2f).OnComplete(()=>ClampMap());

    }

    public void ZoomOut()
    {
        if (zoomIn - 1 < minZoomMod) zoomIn = 1;
        else zoomIn -= 1;

        if (zoomIn == 1)
        {
            canScroll = false;
            actualMap.anchoredPosition = Vector3.zero;
        }
        zoomInText.SetText($"x{zoomIn}");
        //zoomIn--;
        //actualMap.transform.localScale = Vector3.one * zoomIn;
        DOTween.To(() => actualMap.transform.localScale, x => actualMap.transform.localScale = x, Vector3.one * zoomIn, 0.2f).OnComplete(() => ClampMap());
    }



    public void SetDestination(Vector2 destPos)
    {
        StopCoroutine(MoveTo());
        moveDir = (destPos - (Vector2)playerMarker.localPosition).normalized;
        Destination = destPos;
        //leftDist = (Destination - currentPosition).sqrMagnitude * step;
        leftDist = (Destination - (Vector2)playerMarker.localPosition).sqrMagnitude;
        Debug.Log("Set Destination: " + Destination);
        StartCoroutine(MoveTo());
    }

    [Button("randomDest")]
    public void SetRandomDestination()
    {
        var area = AreaManager.GetAreaOf(eRegion.One);
        if (area == null)
        {
            Debug.LogError("No Region: " + eRegion.One);
            return;
        }

        var pos = PolygonRandomPosition.GetRandomPositionOf(area.GetComponent<PolygonCollider2D>());
        destinationImage.gameObject.SetActive(true);

        destinationImage.transform.position = pos;

        //destinationImage.transform.position.Set(pos.x, pos.y, 0f);
        var destImagePos = destinationImage.GetComponent<RectTransform>();

        var newLocalPos = new Vector3(destImagePos.localPosition.x, destImagePos.localPosition.y, 0f);
        destImagePos.localPosition = newLocalPos;
        //destImagePos.anchoredPosition = new Vector3(destImagePos.anchoredPosition.x, destImagePos.anchoredPosition.y, 0f); 
        SetDestination(destinationImage.transform.localPosition);

    }
    public void SetCenter()
    {
        // 마커 * 스케ㅐ일 만큼 뺴기
        actualMap.anchoredPosition = Vector2.zero;
        actualMap.anchoredPosition -= (playerMarker.anchoredPosition*zoomIn);
        ClampMap();
    }

    private IEnumerator MoveTo()
    {
        var oldPosition = playerMarker.localPosition;
        yield return new WaitForSeconds(1f);
        var movedDist = moveDir * walkPerSec * 1/step;

        //DOTween.To(() => damaged.fillAmount, x => damaged.fillAmount = x, current / max, 0.2f);
        playerMarker.DOLocalMove(movedDist, 1f).SetEase(Ease.Linear).SetRelative(true);

        Vector2 curPos = oldPosition;
        DOTween.To(() => curPos, x => curPos = x, curPos + movedDist, 1f).OnUpdate(() =>
        {
            currentPosText.SetText($"{curPos.x.ToString("F0")}, {curPos.y.ToString("F0")}");
        }).SetEase(Ease.Linear);
        currentPosition += movedDist;
        //currentPosText.SetText($"{currentPosition.x.ToString("F0")}, {currentPosition.y.ToString("F0")}");

        leftDist -= ((Vector2)playerMarker.localPosition - (Vector2)oldPosition).sqrMagnitude;
        //leftDist -= dist; // need help here

        if (leftDist < 0)
        {
            leftDist = 0f;
            currentPosition = Destination;
            currentPosText.SetText($"{currentPosition.x.ToString("F0")}, {currentPosition.y.ToString("F0")}");
            OnArriveDestination?.Invoke();
            yield break;
        }
        StartCoroutine(MoveTo());
    }

    private void ClampMap()
    {
        actualMap.anchoredPosition = new Vector2(Mathf.Clamp(actualMap.anchoredPosition.x, -1 * mapWidth * (zoomIn - 1), mapWidth * (zoomIn - 1)),
                Mathf.Clamp(actualMap.anchoredPosition.y, -1 * mapHeight * (zoomIn - 1), mapHeight * (zoomIn - 1)));
    }
    private void PanCamera()
    {
        // Save position of mouse in world space when drag starts (first time click)
        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        // calculate distance between drag origin and new position if it is still held down
        if (Input.GetMouseButton(0))
        {
            Vector2 diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            print("origin " + dragOrigin + " new Position " + cam.ScreenToWorldPoint(Input.mousePosition) + "=difference " + diff);
            actualMap.anchoredPosition += diff;
            ClampMap();
            //cam.transform.position += diff;
        }
    }
    // Update is called once per frame
    void Update()
    {
        PanCamera();
    }
}
