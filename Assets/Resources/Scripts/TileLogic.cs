using Packages.Rider.Editor.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileLogic
{
    // Start is called before the first frame update
    private TileSimulation tileSimulation;
    private Tile tile;
    private Collision2D currentCol;


    public TileSimulation TileSimulation { get => tileSimulation; private set => tileSimulation = value; }
    public Tile Tile { get => tile; set => tile = value; }
    public Collision2D CurrentCol { get => currentCol; set => currentCol = value; }

    public TileLogic(Tile tile, TileSimulation tileSimulation)
    {
        Tile = tile;
        TileSimulation = tileSimulation;
    }

    public class TileDestroyedEvent
    {
        Collision2D col;
        public TileDestroyedEvent(Collision2D col)
        {
            this.col = col;
        }
    }

    void Start()
    {

    }
    public void Hit(Collision2D col)
    {
        CurrentCol = col;
        UpdateHealth(CurrentCol);

    }

    private void DestroyTile()
    {
        // split up
        var explodedable = tile.GetComponent<Explodable>();

        explodedable.explode();
        GlobalMessageBus.Instance.PublishEvent<TileDestroyedEvent>(new TileDestroyedEvent(CurrentCol));
        GameObject.Destroy(Tile.gameObject);
        Debug.Log("destroying");
    }
    private void UpdateHealth(Collision2D col)
    {
        var impactForce = TileSimulation.GetImpactForce(col);
        var projectile = col.collider.GetComponent<Projectile>();
        var rb = col.otherCollider.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            var baseDamage = projectile != null ? projectile.BaseDamage : rb.mass;
            var damage = (Tile.Health) - impactForce * baseDamage - Tile.Armour;
            if (damage > 0)
            {
                Tile.Health -= damage;
                if (Tile.Health < 0f) { DestroyTile();
                }
            }
            else
            {
                Tile.Health += damage;
                GlobalMessageBus.Instance.PublishEvent<TileChangeHealthEvent>(new TileChangeHealthEvent(Tile.Guid));
            }
        }
    }

    public class TileChangeHealthEvent {
        private System.Guid tileGuid;

        public Guid TileGuid { get => tileGuid; set => tileGuid = value; }

        public TileChangeHealthEvent(Guid tileGuid)
        {
            this.TileGuid = tileGuid;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
