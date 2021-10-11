using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBuilder : MonoBehaviour
{
    [SerializeField] public GameObject hexesParent;
    [SerializeField] private GameObject hexObj;
    [SerializeField] private GameObject hexInterSectionPoint;
    [SerializeField] private GameObject interSectionPointParent;

    [SerializeField] public int gridWidth;
    [SerializeField] public int gridHeight;

    [SerializeField] List<Color> colors;
    public static GameBuilder Instance { get; private set; }

    public Dictionary<Vector2, Vector2> hexPosition;
    public Dictionary<Vector2, Hexagon> hexes;
    public int bombScore;

    private GameController gameController;

    private bool hasBuild;
    private bool setted;
    private bool hasBombSet;

    private float hexOffSetX = 3.5f;
    private float hexOffSetY = 4f;

    void Awake()
    {
        #region Singleton
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("HasBomb", 0);
        PlayerPrefs.SetInt("BombCounter", 0);
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
        gameController = GameObject.FindObjectOfType<GameController>();
        hexes = new Dictionary<Vector2, Hexagon>();
        hexPosition = new Dictionary<Vector2, Vector2>();
        BuildGridMap();
        HexInterSectionBuilder();
        #endregion
    }

    void Start()
    {
        while (gameController.DestroyableHexes(hexes).Count > 0)
        {
            DestroyMatchedHexes();
            RebuildGridMap();
        }
        setted = true;
    }

    void Update()
    {
        for (int i = 0; i < hexesParent.transform.childCount; i++)
        {
            var hex = hexesParent.transform.GetChild(i).GetComponent<Hexagon>();
            hex = hexes[new Vector2(hex.coordinateX, hex.coordinateY)];
            colors[hex._colorCode] = hexes[new Vector2(hex.coordinateX, hex.coordinateY)].GetComponent<SpriteRenderer>().color;
            hexes[new Vector2(hex.coordinateX, hex.coordinateY)].transform.GetChild(0).gameObject.SetActive(hex.hasBomb);
        }
    }
    public void DestroyMatchedHexes()
    {
        if (PlayerPrefs.GetInt("HasBomb") == 1)
        {
            var bombCt = PlayerPrefs.GetInt("BombCounter");
            bombCt++;
            PlayerPrefs.SetInt("BombCounter", bombCt);
            Debug.Log(bombCt);
        }
        var destroyableHexes = gameController.DestroyableHexes(hexes);

        foreach (var hex in destroyableHexes)
        {
            for (int i = 0; i < hexesParent.transform.childCount; i++)
            {
                var hex0 = hexesParent.transform.GetChild(i).GetComponent<Hexagon>();

                if (hex0 == hex)
                {
                    if (hex0.hasBomb)
                    {
                        PlayerPrefs.SetInt("BombCounter", 0);
                        PlayerPrefs.SetInt("HasBomb", 0);
                    }

                    hexes[new Vector2(hex.coordinateX, hex.coordinateY)] = null;
                    Destroy(hex0.gameObject);

                    if (setted)
                    {
                        var score = PlayerPrefs.GetInt("Score");
                        score += 5;
                        PlayerPrefs.SetInt("Score", score);
                    }
                }
            }
        }

        hasBuild = false;

    }
    public void RebuildGridMap()
    {
        hasBuild = true;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (y != gridHeight - 1)
                {
                    var hexCoord = new Vector2(x, y);

                    if (hexes[hexCoord] == null)
                    {
                        for (int y0 = y + 1; y0 < gridHeight; y0++)
                        {
                            if (hexes[new Vector2(x, y0)] != null)
                            {
                                hexes[hexCoord] = hexes[new Vector2(x, y0)];
                                hexes[hexCoord].coordinateY = y;
                                hexes[hexCoord].hasBomb = hexes[new Vector2(x, y0)].hasBomb;
                                hexes[hexCoord].colorCode = hexes[new Vector2(x, y0)].colorCode;
                                hexes[hexCoord].GetComponent<SpriteRenderer>().color = colors[hexes[hexCoord]._colorCode];
                                hexes[hexCoord].y = y;
                                hexes[new Vector2(x, y0)] = null;
                                y--;
                                break;
                            }
                        }
                    }
                }
            }
        }

        foreach (var hex in hexes)
        {
            if (hex.Value != null)
            {
                hex.Value.coordinateValueX = hexPosition[new Vector2(hex.Value.coordinateX, hex.Value.coordinateY)].x;
                hex.Value.coordinateValueY = hexPosition[new Vector2(hex.Value.coordinateX, hex.Value.coordinateY)].y;

                for (int i = 0; i < hexesParent.transform.childCount; i++)
                {
                    var hex0 = hexesParent.transform.GetChild(i).GetComponent<Hexagon>();

                    if (hex0.coordinateX == hex.Value.coordinateX && hex0.coordinateY == hex.Value.coordinateY)
                        hex0.transform.position = hexPosition[new Vector2(hex.Value.coordinateX, hex.Value.coordinateY)];
                }
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (hexes[new Vector2(x, y)] == null)
                {
                    var hexPos = hexPosition[new Vector2(x, y)];
                    var hexObject = Instantiate(hexObj, hexPos, Quaternion.identity, hexesParent.transform);
                    var hex = hexObj.GetComponent<Hexagon>();
                    hexes[new Vector2(x, y)] = hex;
                    hexes[new Vector2(x, y)].coordinateX = x;
                    hexes[new Vector2(x, y)].coordinateY = y;
                    hexes[new Vector2(x, y)].coordinateValueX = hexPos.x;
                    hexes[new Vector2(x, y)].coordinateValueX = hexPos.y;
                    hexes[new Vector2(x, y)]._colorCode = Random.Range(0, colors.Count());

                    if (PlayerPrefs.GetInt("HasBomb") == 0 && PlayerPrefs.GetInt("Score") >= bombScore)
                        hexes[new Vector2(x, y)].hasBomb = true;
                    else
                        hexes[new Vector2(x, y)].hasBomb = false;

                    if (hexes[new Vector2(x, y)].hasBomb)
                    {
                        hexes[new Vector2(x, y)].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        PlayerPrefs.SetInt("HasBomb", 1);
                    }

                    hexes[new Vector2(x, y)].GetComponent<SpriteRenderer>().color = colors[hexes[new Vector2(x, y)]._colorCode];
                }
            }
        }
    }
    private void BuildGridMap()
    {
        float initialX = gridWidth % 2 == 0 ? (gridWidth / 2 * -hexOffSetX) + hexOffSetX / 2f : (Mathf.Floor(gridWidth / 2f)) * -hexOffSetX;
        float initialY = (Mathf.Floor(gridHeight / 2f)) * -hexOffSetY;
        float xPos = initialX;
        float yPos = initialY;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var hexObject = Instantiate(hexObj, new Vector2(xPos, yPos), Quaternion.identity, hexesParent.transform);
                InitHex(hexObject.GetComponent<Hexagon>(), x, y, xPos, yPos, Random.Range(0, colors.Count));
                hexPosition.Add(new Vector2(x, y), new Vector2(xPos, yPos));
                hexes.Add(new Vector2(x, y), hexObject.GetComponent<Hexagon>());

                yPos += hexOffSetY;
            }

            if ((x + 1) % 2 == 0)
                yPos = initialY;
            else
                yPos = initialY + 2;

            xPos += hexOffSetX;
        }
    }
    private void InitHex(Hexagon hex, int coordX, int coordY, float coordValueX, float coordValueY, int colorCode)
    {
        hex.coordinateX = coordX;
        hex.coordinateY = coordY;
        hex.coordinateValueX = coordValueX;
        hex.coordinateValueY = coordValueY;
        hex._colorCode = colorCode;
        hex.GetComponent<SpriteRenderer>().color = colors[colorCode];

    }
    private void HexInterSectionBuilder()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (x != gridWidth - 1)
                {
                    if (x % 2 == 0)
                    {
                        if (y == 0)
                        {
                            var origin = FindIntersectionCoordinate(hexes[new Vector2(x, y)], hexes[new Vector2(x, y + 1)], hexes[new Vector2(x + 1, y)]);
                            var hexIntersectionPoint = Instantiate(hexInterSectionPoint, origin, Quaternion.identity, interSectionPointParent.transform).GetComponent<HexagonInterSectionPoint>();
                            InitHexIntersectionPoint(hexIntersectionPoint, 180f);
                        }
                        else if (y != gridHeight - 1)
                        {
                            var origin = FindIntersectionCoordinate(hexes[new Vector2(x, y)], hexes[new Vector2(x + 1, y - 1)], hexes[new Vector2(x + 1, y)]);
                            var hexIntersectionPoint = Instantiate(hexInterSectionPoint, origin, Quaternion.identity, interSectionPointParent.transform).GetComponent<HexagonInterSectionPoint>();
                            InitHexIntersectionPoint(hexIntersectionPoint, 0f);

                            var origin0 = FindIntersectionCoordinate(hexes[new Vector2(x, y)], hexes[new Vector2(x, y + 1)], hexes[new Vector2(x + 1, y)]);
                            var hexIntersectionPoint0 = Instantiate(hexInterSectionPoint, origin0, Quaternion.identity, interSectionPointParent.transform).GetComponent<HexagonInterSectionPoint>();
                            InitHexIntersectionPoint(hexIntersectionPoint0, 180f);
                        }
                        else
                        {
                            var origin = FindIntersectionCoordinate(hexes[new Vector2(x, y)], hexes[new Vector2(x + 1, y - 1)], hexes[new Vector2(x + 1, y)]);
                            var hexIntersectionPoint = Instantiate(hexInterSectionPoint, origin, Quaternion.identity, interSectionPointParent.transform).GetComponent<HexagonInterSectionPoint>();
                            InitHexIntersectionPoint(hexIntersectionPoint, 0f);
                        }
                    }
                    else
                    {
                        if (y != gridHeight - 1)
                        {
                            var origin = FindIntersectionCoordinate(hexes[new Vector2(x, y)], hexes[new Vector2(x + 1, y)], hexes[new Vector2(x + 1, y + 1)]);
                            var hexIntersectionPoint = Instantiate(hexInterSectionPoint, origin, Quaternion.identity, interSectionPointParent.transform).GetComponent<HexagonInterSectionPoint>();
                            InitHexIntersectionPoint(hexIntersectionPoint, 0f);

                            var origin0 = FindIntersectionCoordinate(hexes[new Vector2(x, y)], hexes[new Vector2(x, y + 1)], hexes[new Vector2(x + 1, y + 1)]);
                            var hexIntersectionPoint0 = Instantiate(hexInterSectionPoint, origin0, Quaternion.identity, interSectionPointParent.transform).GetComponent<HexagonInterSectionPoint>();
                            InitHexIntersectionPoint(hexIntersectionPoint0, 180f);
                        }
                    }
                }
            }
        }
    }
    private Vector2 FindIntersectionCoordinate(Hexagon hex1, Hexagon hex2, Hexagon hex3)
    {
        var totalX = hex1.coordinateValueX + hex2.coordinateValueX + hex3.coordinateValueX;
        var totalY = hex1.coordinateValueY + hex2.coordinateValueY + hex3.coordinateValueY;

        return new Vector2(totalX / 3, totalY / 3);
    }
    private void InitHexIntersectionPoint(HexagonInterSectionPoint hexIntersection, float _rotationZValue)
    {
        hexIntersection.rotationZValue = _rotationZValue;
    }
}
