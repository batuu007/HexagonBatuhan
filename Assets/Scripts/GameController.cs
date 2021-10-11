using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject hexFrameObject;
    [SerializeField] Text scoreText;

    private Vector3 hexFrameObjectLastPos;
    private Vector3 firstMousePos;
    private Vector3 lastMousePos;
    private Transform hexFrameObjectInitialTransform;
    private Quaternion targetRot;
    private bool dragging;
    private bool waitForCalculate;
    private int roundMultiplier = 2;
    private float selectedIntersectionPointRot;

    void Start()
    {
        hexFrameObjectLastPos = hexFrameObject.transform.position;
        hexFrameObjectInitialTransform = hexFrameObject.transform;
        targetRot = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        ScorePanel();
        CoordinatorDistance();
    }

    private void CoordinatorDistance()
    {
        if (hexFrameObjectLastPos != hexFrameObject.transform.position)
        {
            var x = FindSelectedInterSectionPoint();
            selectedIntersectionPointRot = x.GetComponent<HexagonInterSectionPoint>().rotationZValue;

            if (x != null)
            {
                SetInitialParent();
                var nearestHexes = FindIntersectionsNearestHexes(x);
                var hexesParent = GameBuilder.Instance.hexesParent.transform;

                foreach (var hex in nearestHexes)
                {
                    for (int i = 0; i < hexesParent.childCount; i++)
                    {
                        if (hexesParent.transform.GetChild(i).GetComponent<Hexagon>().coordinateX == hex.GetComponent<Hexagon>().coordinateX &&
                            hexesParent.transform.GetChild(i).GetComponent<Hexagon>().coordinateY == hex.GetComponent<Hexagon>().coordinateY)
                        {
                            hexesParent.transform.GetChild(i).parent = hexFrameObject.transform;
                        }
                    }
                }
            }
            hexFrameObjectLastPos = hexFrameObject.transform.position;
        }
        if (Vector3.Distance(firstMousePos, lastMousePos) > 10)
        {
            if (selectedIntersectionPointRot == 0f)
            {
                RotateHexFrameType();
            }
            else
            {
                RotateHexFrameType1();
            }
        }
    }

    private void RotateHexFrameType()
    {
        #region TurnCount
        Vector3 difference = lastMousePos - hexFrameObject.transform.position;
        difference.Normalize();
        var interval = Mathf.Atan2(lastMousePos.x, lastMousePos.y);

        if (Mathf.Atan2(lastMousePos.x, lastMousePos.y) > interval)
        {
            hexFrameObject.transform.rotation = Quaternion.RotateTowards(hexFrameObject.transform.rotation, Quaternion.Euler(0f, 0f, -60 * roundMultiplier), 500 * Time.deltaTime);

            if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, -120f).eulerAngles && roundMultiplier == 2 && !waitForCalculate)
            {
                RotateColorsClockWise(1);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, -240f).eulerAngles && roundMultiplier == 4 && !waitForCalculate)
            {
                RotateColorsClockWise(1);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, -360f).eulerAngles && roundMultiplier == 6 && !waitForCalculate)
            {
                RotateColorsClockWise(1);
                StartCoroutine(RotateHexFrame());
            }
        }
        else
        {
            hexFrameObject.transform.rotation = Quaternion.RotateTowards(hexFrameObject.transform.rotation, Quaternion.Euler(0f, 0f, 60 * roundMultiplier), 500 * Time.deltaTime);

            if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 120f).eulerAngles && roundMultiplier == 2 && !waitForCalculate)
            {
                RotateColorsCountClockWise(1);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 240f).eulerAngles && roundMultiplier == 4 && !waitForCalculate)
            {
                RotateColorsCountClockWise(1);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 360f).eulerAngles && roundMultiplier == 6 && !waitForCalculate)
            {
                RotateColorsCountClockWise(1);
                StartCoroutine(RotateHexFrame());
            }
        }
        #endregion 
    }
    private void RotateHexFrameType1()
    {
        #region TurnCount
        Vector3 difference = lastMousePos - hexFrameObject.transform.position;
        difference.Normalize();
        var interval = Mathf.Atan2(lastMousePos.x, lastMousePos.y);

        if (Mathf.Atan2(lastMousePos.x, lastMousePos.y) > interval)
        {
            hexFrameObject.transform.rotation = Quaternion.RotateTowards(hexFrameObject.transform.rotation, Quaternion.Euler(0f, 0f, -60 * roundMultiplier), 500 * Time.deltaTime);

            if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, -60f).eulerAngles && roundMultiplier == 2 && !waitForCalculate)
            {
                RotateColorsClockWise(2);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, -60f).eulerAngles && roundMultiplier == 4 && !waitForCalculate)
            {
                RotateColorsClockWise(2);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 180f).eulerAngles && roundMultiplier == 6 && !waitForCalculate)
            {
                RotateColorsClockWise(2);
                StartCoroutine(RotateHexFrame());
            }
        }
        else
        {
            hexFrameObject.transform.rotation = Quaternion.RotateTowards(hexFrameObject.transform.rotation, Quaternion.Euler(0f, 0f, 60 * roundMultiplier), 500 * Time.deltaTime);

            if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 120f).eulerAngles && roundMultiplier == 2 && !waitForCalculate)
            {
                RotateColorsCountClockWise(2);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 240f).eulerAngles && roundMultiplier == 4 && !waitForCalculate)
            {
                RotateColorsCountClockWise(2);
                StartCoroutine(RotateHexFrame());
            }
            else if (hexFrameObject.transform.rotation.eulerAngles == Quaternion.Euler(0f, 0f, 360f).eulerAngles && roundMultiplier == 6 && !waitForCalculate)
            {
                RotateColorsCountClockWise(2);
                StartCoroutine(RotateHexFrame());
            }
        }
        #endregion 
    }
    private void ScorePanel()
    {
        scoreText.text = PlayerPrefs.GetInt("Score").ToString();

        if (PlayerPrefs.GetInt("BombCounter") == 5)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Game Over" + scoreText);
        }
    }
    public bool FindHex(Hexagon hex)
    {
        var hexesParent = GameObject.Find("Hexagons").transform;

        for (int i = 0; i < hexesParent.childCount; i++)
        {
            var hexObj = hexesParent.GetChild(i);

            if (hexObj.GetComponent<Hexagon>().coordinateX== hex.coordinateX && hexObj.GetComponent<Hexagon>().coordinateY == hex.coordinateY)
                return true;
        }

        return false;
    }
    public GameObject FindSelectedInterSectionPoint()
    {
        var interSectionParent = GameObject.Find("HexInterSections").transform;

        for (int i = 0; i < interSectionParent.childCount; i++)
        {
            if (hexFrameObject.transform.position == interSectionParent.transform.GetChild(i).position)
            {
                return interSectionParent.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }
    private void SetInitialParent()
    {
        if (hexFrameObject.transform.childCount > 1)
        {
            var childCound = hexFrameObject.transform.childCount;
            for (int i = 0; i < childCound; i++)
            {
                var hex = hexFrameObject.transform.GetChild(1).GetComponent<Hexagon>();
                hex.transform.parent = GameBuilder.Instance.hexesParent.transform;
                hex.gameObject.transform.position = GameBuilder.Instance.hexPosition[new Vector2(hex.coordinateX, hex.coordinateY)];
            }
        }
    }
    public List<GameObject> FindIntersectionsNearestHexes(GameObject selectedIntersectionPoint)
    {
        var hexesParent = GameObject.Find("Hexagons").transform;
        var hexes = new List<GameObject>();

        for (int i = 0; i < hexesParent.childCount; i++)
        {
            var hexagonObj = hexesParent.transform.GetChild(i);
            var distance = Vector2.Distance(hexagonObj.position, selectedIntersectionPoint.transform.position);

            if (distance < 3f)
                hexes.Add(hexagonObj.gameObject);
        }

        return hexes;
    }
    private void RotateColorsClockWise(int type)
    {
        var hexes = GameBuilder.Instance.hexes;
        var hex1 = new Hexagon();
        var hex2 = new Hexagon();
        var hex3 = new Hexagon();

        var selectedHexes = new List<Hexagon>();

        for (int i = 0; i < 4; i++)
        {
            selectedHexes.Add(hexFrameObject.transform.GetChild(i).GetComponent<Hexagon>());
        }
        if (type == 1)
        {
            hex1 = selectedHexes.OrderBy(s => s.coordinateX).First();
            hex2 = selectedHexes.OrderByDescending(s => s.coordinateX).OrderBy(k => k.coordinateY).First();
            hex3 = selectedHexes.OrderByDescending(s => s.coordinateX).OrderByDescending(k => k.coordinateY).First();
        }
        else
        {
            hex1 = selectedHexes.OrderBy(s => s.coordinateX).OrderBy(k => k.coordinateY).First();
            hex2 = selectedHexes.OrderByDescending(s => s.coordinateX).First();
            hex3 = selectedHexes.OrderByDescending(s => s.coordinateY).OrderBy(k => k.coordinateX).First();
        }
        var temp = hex1._colorCode;
        hexes[new Vector2(hex1.coordinateX, hex1.coordinateY)]._colorCode = hex2._colorCode;
        hexes[new Vector2(hex2.coordinateX, hex2.coordinateY)]._colorCode = hex3._colorCode;
        hexes[new Vector2(hex3.coordinateX, hex3.coordinateY)]._colorCode = temp;
    }
    private void RotateColorsCountClockWise(int type)
    {
        var hexes = GameBuilder.Instance.hexes;
        var hex1 = new Hexagon();
        var hex2 = new Hexagon();
        var hex3 = new Hexagon();

        var selectedHexes = new List<Hexagon>();

        for (int i = 0; i < 4; i++)
        {
            selectedHexes.Add(hexFrameObject.transform.GetChild(i).GetComponent<Hexagon>());
        }
        if (type == 1)
        {
            hex1 = selectedHexes.OrderBy(s => s.coordinateX).First();
            hex2 = selectedHexes.OrderByDescending(s => s.coordinateX).OrderBy(k => k.coordinateY).First();
            hex3 = selectedHexes.OrderByDescending(s => s.coordinateX).OrderByDescending(k => k.coordinateY).First();
        }
        else
        {
            hex1 = selectedHexes.OrderBy(s => s.coordinateX).OrderBy(k => k.coordinateY).First();
            hex2 = selectedHexes.OrderByDescending(s => s.coordinateX).First();
            hex3 = selectedHexes.OrderByDescending(s => s.coordinateY).OrderBy(k => k.coordinateX).First();
        }
        var temp = hex1._colorCode;
        hexes[new Vector2(hex1.coordinateX, hex1.coordinateY)]._colorCode = hex3._colorCode;
        hexes[new Vector2(hex3.coordinateX, hex3.coordinateY)]._colorCode = hex2._colorCode;
        hexes[new Vector2(hex2.coordinateX, hex2.coordinateY)]._colorCode = temp;
    }
    IEnumerator RotateHexFrame()
    {
        waitForCalculate = true;
        yield return new WaitForSeconds(0.1f);
        roundMultiplier += 2;
        ResetMove();
        waitForCalculate = false;

        if (roundMultiplier==8)
        {
            roundMultiplier = 2;
            firstMousePos = new Vector3(0, 0, 0);
            lastMousePos = new Vector3(0, 0, 0);
        }
    }
    private void OnMouseDown()
    {
        firstMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMousePos = firstMousePos;
    }
    private void OnMouseUp()
    {
        lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void ResetMove()
    {
        while (DestroyableHexes(GameBuilder.Instance.hexes).Count>0)
        {
            SetInitialParent();
            roundMultiplier = 2;
            firstMousePos = new Vector3(0, 0, 0);
            lastMousePos = new Vector3(0, 0, 0);
            hexFrameObject.transform.GetChild(0).gameObject.SetActive(false);
            GameBuilder.Instance.DestroyMatchedHexes();
            GameBuilder.Instance.RebuildGridMap();
        }
    }
    public List<Hexagon> DestroyableHexes(Dictionary<Vector2, Hexagon> hexes)
    {
        var gridWidth = GameBuilder.Instance.gridWidth;
        var gridHeight = GameBuilder.Instance.gridHeight;
        List<Hexagon> destroyableHexes = new List<Hexagon>();

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
                            var hex1 = hexes[new Vector2(x, y)];
                            var hex2 = hexes[new Vector2(x, y + 1)];
                            var hex3 = hexes[new Vector2(x + 1, y)];

                            if ((hex1 != null && hex2 != null && hex3 != null) && hex1._colorCode == hex2._colorCode && hex1._colorCode == hex3._colorCode)
                            {
                                var colorCode = hex1._colorCode;
                                destroyableHexes.Add(hex1);
                                destroyableHexes.Add(hex2);
                                destroyableHexes.Add(hex3);
                                if (hexes[new Vector2(x + 1, y + 1)]._colorCode == colorCode)
                                {

                                }
                            }
                        }
                        else if (y != gridHeight - 1)
                        {
                            var hex1 = hexes[new Vector2(x, y)];
                            var hex2 = hexes[new Vector2(x + 1, y - 1)];
                            var hex3 = hexes[new Vector2(x + 1, y)];

                            if ((hex1 != null && hex2 != null && hex3 != null) && hex1._colorCode == hex2._colorCode && hex1._colorCode == hex3._colorCode)
                            {
                                destroyableHexes.Add(hex1);
                                destroyableHexes.Add(hex2);
                                destroyableHexes.Add(hex3);
                            }

                            var hex1a = hexes[new Vector2(x, y)];
                            var hex2a = hexes[new Vector2(x, y + 1)];
                            var hex3a = hexes[new Vector2(x + 1, y)];

                            if ((hex1a != null && hex2a != null && hex3a != null) && hex1a._colorCode == hex2a._colorCode && hex1a._colorCode == hex3a._colorCode)
                            {
                                destroyableHexes.Add(hex1a);
                                destroyableHexes.Add(hex2a);
                                destroyableHexes.Add(hex3a);
                            }
                        }
                        else
                        {
                            var hex1 = hexes[new Vector2(x, y)];
                            var hex2 = hexes[new Vector2(x + 1, y - 1)];
                            var hex3 = hexes[new Vector2(x + 1, y)];

                            if ((hex1 != null && hex2 != null && hex3 != null) && hex1._colorCode == hex2._colorCode && hex1._colorCode == hex3._colorCode)
                            {
                                destroyableHexes.Add(hex1);
                                destroyableHexes.Add(hex2);
                                destroyableHexes.Add(hex3);
                            }
                        }
                    }
                    else
                    {
                        if (y != gridHeight - 1)
                        {
                            var hex1 = hexes[new Vector2(x, y)];
                            var hex2 = hexes[new Vector2(x + 1, y)];
                            var hex3 = hexes[new Vector2(x + 1, y + 1)];

                            if ((hex1 != null && hex2 != null && hex3 != null) && hex1._colorCode == hex2._colorCode && hex1._colorCode == hex3._colorCode)
                            {
                                destroyableHexes.Add(hex1);
                                destroyableHexes.Add(hex2);
                                destroyableHexes.Add(hex3);
                            }

                            var hex1a = hexes[new Vector2(x, y)];
                            var hex2a = hexes[new Vector2(x, y + 1)];
                            var hex3a = hexes[new Vector2(x + 1, y + 1)];

                            if ((hex1a != null && hex2a != null && hex3a != null) && hex1a._colorCode == hex2a._colorCode && hex1a._colorCode == hex3a._colorCode)
                            {
                                destroyableHexes.Add(hex1a);
                                destroyableHexes.Add(hex2a);
                                destroyableHexes.Add(hex3a);
                            }
                        }
                    }
                }
            }
        }
        return destroyableHexes.ToList();
    }
    public void OnPlayAgainClicked()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
