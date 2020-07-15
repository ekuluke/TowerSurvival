using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class UIController : MonoBehaviour
{
    private List<UnityEngine.UI.Button> btn;

    private List<GameObject> subMenus;
    private List<GameObject> basicShapes;
    private List<GameObject> basicRooms;
    private List<GameObject> basicProjectiles;
    private bool buildCond = false;

    private GameObject buildItem;
    private GameObject roomItem;
    private GameObject projectileItem;
    private int zRotate = 0;

    private IEnumerable<float> scaleRange;

    private bool projectileTestCond;
    private Sprite selectedRoom;

    private Dictionary<string,bool> conditions = new Dictionary<string, bool>();
    public List<GameObject> SubMenus { get => subMenus; set => subMenus = value; }
    public List<GameObject> BasicShapes { get => basicShapes; set => basicShapes = value; }
    public bool BuildCond { get => buildCond; set => buildCond = value; }
    private GameObject BuildItem { get => buildItem; set => buildItem = value; }

    public IEnumerable<float> ScaleRange { get => scaleRange; set => scaleRange = value; }
    private int ZRotate { get => zRotate; set => zRotate = value; }
    public bool ProjectileTestCond { get => projectileTestCond; set => projectileTestCond = value; }
    public Dictionary<string, bool> Conditions { get => conditions; set => conditions = value; }
    public Sprite SelectedRoom { get => selectedRoom; set => selectedRoom = value; }
    public GameObject RoomItem { get => roomItem; set => roomItem = value; }
    public List<GameObject> BasicRooms { get => basicRooms; set => basicRooms = value; }
    public List<GameObject> BasicProjectiles { get => basicProjectiles; set => basicProjectiles = value; }
    public GameObject ProjectileItem { get => projectileItem; set => projectileItem = value; }

    public delegate void CurrentMode();
    CurrentMode currentMode = null;

    public GameObject BasicShapeMenu;
    public GameObject BasicRoomMenu;
    public GameObject BasicProjectileMenu;
    public GameObject PlacedObjectParent;
    public GameObject ProjectileParent;
    public GameObject ProjectileTest;
    public GameObject SubMenuSelector;
    public GameObject RoomDrawRect;


    // Start is called before the first frame update
    void Start()
    {

        BasicShapes = GetMenuObjects(BasicShapeMenu);
        BasicRooms = GetMenuObjects(BasicRoomMenu);
        BasicProjectiles = GetMenuObjects(BasicProjectileMenu);
        foreach(var i in BasicShapes)
        {
            print(i);
        }
 


        foreach (var item in BasicShapes)
        {
            if (!Conditions.ContainsKey(item.tag))
            {

                // Create if not exists in dictionary
                Conditions[item.tag] = false;
                print(Conditions[item.tag]);
            }
        }
        Conditions.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(print);

        AddListenersToUI();
        

}

    public void AddListenersToUI()
    {
        foreach (var i in BasicShapes)
        {
            
            if (i.tag == "RoomMode")
            {

                i.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ToggleMenuType(i.tag));
                i.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => BuildItem = i.GetComponent<UIFieldData>().Spawn());

            }
            else if (i.tag == "BuildMode") 
        

            {
                i.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ToggleMenuType(i.tag));
                i.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => BuildItem = i.GetComponent<UIFieldData>().Spawn());
            }
        }
        foreach(var i in BasicProjectiles)
        {
            if(i.tag == "ProjectileMode")
            {
                i.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ToggleMenuType(i.tag));
                i.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ProjectileItem = i.GetComponent<UIFieldData>().Spawn());
            }
        }
    }

    public void ToggleMenuType(string tagToToggleOn) // sets other conditions to false, except for the parameter condToTurnOn
    {
        Debug.Log("tmt");
        foreach(var key in Conditions.Keys.ToList())
        {
            print(Conditions[key]);
            if (key != tagToToggleOn)
            {
                Conditions[key] = false;
                
            }
            else
            {
                Conditions[key] = true;
            }
        }

        if (Conditions["RoomMode"] == true)
        {    
            currentMode = RoomMode; 
        }
        if (Conditions["ProjectileMode"] == true)
        {
            currentMode = ProjectileMode;
        }
        if (Conditions["BuildMode"] == true)
        {
            currentMode = BuildMode;
        } 
    }
    
    

    // Update is called once per frame
    void Update()
    {
        currentMode?.Invoke();
        //print(currentMode);
    }

    private void ProjectileMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var load = (GameObject)Resources.Load("Prefabs/Projectile");
            GameObject newProjectile = Instantiate<GameObject>(load, new Vector2(-13.5f,3.69f),/*cursorPos.x, cursorPos.y), */ Quaternion.identity, ProjectileParent.transform);
            newProjectile.GetComponent<Projectile>().MoveTo(Vector2.right * 1000);
        }
    }

    
    
    private void BuildMode() // Handles the construction of new gameobjects
    {
        
        bool placeable = false;
        var sr = BuildItem.GetComponent<SpriteRenderer>();
        var originalItemColor = BuildItem.GetComponent<SpriteRenderer>().color; 
        sr.color = new Color(1f, 0f, 0f, 0.7f);
        var rb = BuildItem.GetComponent<Rigidbody2D>();
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        rb.MovePosition(new Vector2(cursorPos.x.RoundToScale(), cursorPos.y.RoundToScale()));
        ContactFilter2D cf = new ContactFilter2D();
        cf = cf.NoFilter();
        Collider2D[] results = new Collider2D[1];
        var buildItemCollider = BuildItem.GetComponent<BoxCollider2D>();
        int overlapCount = buildItemCollider.OverlapCollider(cf, results);
        if (overlapCount == 0) {
            sr.color = new Color(0f, 1f, 0f, 0.7f); // green
            placeable = true;
        }
        else{
            sr.color = new Color(1f, 0f, 0f, 1f); // red
            placeable = false;
        } 


        if(Input.GetKeyDown(KeyCode.Q)) {
            buildItem.transform.Rotate(0, 0, 22.5f);
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            buildItem.transform.Rotate(0, 0, -22.5f);
        }

        if (Input.GetMouseButtonDown(0) && placeable)
        {
            
            var placedItem = Instantiate<GameObject>(BuildItem, BuildItem.transform.position, Quaternion.identity, PlacedObjectParent.transform);
            var bcPlaced = placedItem.GetComponent<BoxCollider2D>();
            var srPlaced = placedItem.GetComponent<SpriteRenderer>();
            srPlaced.color = originalItemColor;
            var rbPlaced = placedItem.GetComponent<Rigidbody2D>();
            var explodable = placedItem.AddComponent<Explodable>();
            bcPlaced.enabled = true;
            bcPlaced.isTrigger = false;
            rbPlaced.bodyType = RigidbodyType2D.Dynamic;
            

            placedItem.transform.SetParent(PlacedObjectParent.transform);
            
        }
    }

    private void RoomMode()
    {

        GlobalMessageBus.Instance.PublishEvent(new BusEvents.isScalingEvent());
        var selectionBox = GetComponent<GUISelectionBox>();
        var sr = GetComponent<GUISelectionBox>().selectionBox.GetComponent<SpriteRenderer>();
        sr.sprite = SelectedRoom;
        Color defaultColor = sr.color;
        Color roomDraw = defaultColor;
        roomDraw.a = 0.18f;
        sr.color = roomDraw;



        
        
    }
    private List<GameObject> GetMenuObjects(GameObject go) // go = parent of child objects
    {
        List<GameObject> menuObjects = new List<GameObject>();
        foreach(Transform item in go.transform)
        {
            menuObjects.Add(item.gameObject);
        }
        return menuObjects;
    }

    /*private List<UnityEngine.UI.Button> GetMenuBtns(GameObject go) // go = parent of child objects
    {
        List<UnityEngine.UI.Button> menuBtns = new List<UnityEngine.UI.Button>();
        foreach (var item in go.GetComponentInChildren<UnityEngine.UI.Button>().transform)
        

        { 
            
            menuBtns.Add();
        }
        return menuBtns;
    }
    */
}
