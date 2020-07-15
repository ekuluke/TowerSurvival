using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;


public class Tile : MonoBehaviour, ISubscriber<TileLogic.TileChangeHealthEvent>
{

    private TileLogic tileLogic;
    private TileSimulation tileSimulation;
    private Vector2 initialVelocity;

    private System.Guid guid = System.Guid.Empty;
    [SerializeField]
    private byte[] serializedGuid;

    private Sprite sprite;

    [XmlAttribute("name")]
    private string name;
    [XmlAttribute("health")]
    private float health;
    [XmlAttribute("armour")]
    private float armour;
    [XmlAttribute("mass")]
    private float mass;
    [XmlAttribute("shield")]
    private float shield;
    [XmlAttribute("density")]
    private float density;
    [XmlAttribute("rust")]
    private float rust;
    [XmlAttribute("friction")]
    private float friction;
    private float healthLastFrame;
    public TileLogic TileLogic { get => tileLogic; private set => tileLogic = value; }
    private TileSimulation TileSimulation { get => tileSimulation; set => tileSimulation = value; }
    public Vector2 InitialVelocity { get => initialVelocity; set => initialVelocity = value; }
    private string Name { get => name; set => name = value; }
    public float Health { get => health; set => health = value; }
    public float Armour { get => armour; set => armour = value; }
    public float Shield { get => shield; set => shield = value; }
    public float Density { get => density; set => density = value; }
    public float Rust { get => rust; set => rust = value; }
    public float Friction { get => friction; set => friction = value; }
    public float Mass { get => mass; set => mass = value; }
    public Guid Guid { get => guid; set => guid = value; }

    public byte[] SerializedGuid { get => serializedGuid; set => serializedGuid = value; }
    private float HealthLastFrame { get => healthLastFrame; set => healthLastFrame = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }

    public Tile(string spriteName)
    {

    }

    void Start()
    {
        GlobalMessageBus.Instance.Subscribe(this);
    }

    void ISubscriber<TileLogic.TileChangeHealthEvent>.OnEvent(TileLogic.TileChangeHealthEvent evt)
    {
    }


    void Awake()
    {
        TileSimulation = new TileSimulation();
        TileLogic = new TileLogic(this, TileSimulation);
        GetComponent<Explodable>().fragmentInEditor();
        OnAfterDeserialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health != HealthLastFrame)
        {
            //Init(); 
        }

        HealthLastFrame = Health;
        


    }

    private void Init()
    {
        Explodable explodable = GetComponent<Explodable>();
        if (explodable.fragments == null)
            explodable.extraPoints = 5;
            explodable.fragmentInEditor();
        {
        foreach (var i in explodable.fragments) 
            {
                if (i != null)
                {
                    var tex = GetComponent<SpriteRenderer>().sprite.TextureFromSprite();
                    var polySlice = i.GetComponent<PolygonCollider2D>().points;
                    Destroy(i.GetComponent<MeshRenderer>());
                    Destroy(i.GetComponent<MeshFilter>());
                    
                    // Finds the furtherest points
                    Vector2 maxPoints = Vector2.zero;
                    Vector2 minPoints = Vector2.zero; 
                    foreach(var point in polySlice)
                    {
                        if(point.x > maxPoints.x )
                        {
                            maxPoints.x = point.x;                                
                        } 
                        else if(point.y > maxPoints.y)
                        {
                            maxPoints.y = point.y;
                        }
                        if(point.x > minPoints.x )
                        {
                            minPoints.x = point.x;                                
                        } 
                        else if(point.y > minPoints.y)
                        {
                            minPoints.y = point.y;
                        }
                        
                    }
                    Rect rectSlice = new Rect(0, 0, maxPoints.x - minPoints.x, maxPoints.y - maxPoints.y);
                    print(rectSlice);
                    Sprite fragmentSprite = Sprite.Create(tex, new Rect(0,0, 15, 15), new Vector2(0.5f, 0.5f), 32.0f);
                    SpriteRenderer sr;
                    GameObject finalFragment = new GameObject();
                    finalFragment.transform.SetParent(transform);
                    finalFragment.transform.position = transform.position;
                    //Destroy(i);
                    sr = finalFragment.AddComponent<SpriteRenderer>();
                    var pc = finalFragment.AddComponent<PolygonCollider2D>();
                    pc.points = polySlice;
                    sr.sprite = fragmentSprite;
                    print(fragmentSprite);
                    
                }
            }
            for (int i = 0; i < explodable.fragments.Count; i++)
            {
                // Destroy(explodable.fragments[i]);
            }
        }
    }

    private void OnApplicationQuit()
    {
        OnBeforeSerialize();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        TileLogic.Hit(col);
    }

    public void OnBeforeSerialize()
    {
        if (Guid != System.Guid.Empty)
        {
            SerializedGuid = Guid.ToByteArray();
        }
    }

    // On load, we can go ahead and restore our system guid for later use
    public void OnAfterDeserialize()
    {
        if (SerializedGuid != null && SerializedGuid.Length == 16)
        {
            Guid = new System.Guid(SerializedGuid);
        }
    }
}

