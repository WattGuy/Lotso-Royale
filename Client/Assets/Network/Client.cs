using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Client : MonoBehaviour {

    public string ip = "127.0.0.1";
    public ushort port = 4296;

    public static Client instance;

    public GameObject player;

    public Dictionary<ushort, PlayerController> players = new Dictionary<ushort, PlayerController>();
    public Dictionary<ushort, GameObject> objects = new Dictionary<ushort, GameObject>();
    public Dictionary<ushort, GameObject> bullets = new Dictionary<ushort, GameObject>();
    public Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    private Host client;
    private ENet.Event netEvent;
    public Peer peer;
    private bool disconnected = true;

    private long when;
    private int? time = null;
    public ushort? id = null;
    public ushort? number = null;

    void Start () {
        instance = this;

        foreach (DestructibleType dt in Enum.GetValues(typeof(DestructibleType)).Cast<DestructibleType>()) {

            prefabs.Add(dt.ToString(), Resources.Load<GameObject>("Objects/" + dt.ToString()));

        }

        foreach (IndestructibleType it in Enum.GetValues(typeof(IndestructibleType)).Cast<IndestructibleType>())
        {

            prefabs.Add(it.ToString(), Resources.Load<GameObject>("Objects/" + it.ToString()));

        }

        foreach (Item i in Enum.GetValues(typeof(Item)).Cast<Item>())
        {

            prefabs.Add(i.ToString(), Resources.Load<GameObject>("Items/" + i.ToString()));

        }

        Connect();
    }

    public void Connect() {

        if (Menu.ip != null) {

            ip = Menu.ip;

        }

        port = Menu.port;

        Library.Initialize();

        client = new Host();
        Address address = new Address();

        address.SetHost(ip);
        address.Port = port;
        client.Create();

        peer = client.Connect(address);

        disconnected = false;
        StartCoroutine("Pinger");
        StartCoroutine("Zoner");
    }

    void Update() {
        if (disconnected) return;
        bool polled = false;

        while (!polled)
        {
            if (client.CheckEvents(out netEvent) <= 0)
            {
                if (client.Service(15, out netEvent) <= 0)
                    break;

                polled = true;
            }

            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    Debug.Log("Client connected to server");
                    when = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    Send(new Message() { MsgType = Tags.PING }, netEvent.ChannelID);
                    break;

                case ENet.EventType.Timeout:
                case ENet.EventType.Disconnect:
                    Debug.Log("Client disconnected from server");
                    break;

                case ENet.EventType.Receive:
                    OnReceive();
                    netEvent.Packet.Dispose();
                    break;
            }
        }

    }

    public void Send(IMessage msg, byte channel) { Send(msg, channel, PacketFlags.None); }

    public void Send(IMessage msg, byte channel, PacketFlags flag)
    {
        Send(new Message { MsgType = msg.GetTag(), Msg = msg.ToByteString() }, channel, flag);
    }

    public void Send(Message msg, byte channel) { Send(msg, channel, PacketFlags.None); }

    public void Send(Message msg, byte channel, PacketFlags flag)
    {
        Packet packet = default(Packet);
        if (flag == PacketFlags.None) packet.Create(ProtobufEncoding.Encode(msg));
        else packet.Create(ProtobufEncoding.Encode(msg), flag);

        peer.Send(channel, ref packet);
    }

    public void OnReceive()
    {
        byte[] bs = new byte[1024];
        netEvent.Packet.CopyTo(bs);
        Message msg = ProtobufEncoding.Decode(bs) as Message;

        switch (msg.MsgType)
        {

            case Tags.ID:
                id = ushort.Parse(msg.Msg.ToStringUtf8());

                if (!players.ContainsKey((ushort) id)) return;

                PlayerController p = players[(ushort) id];
                p.controllable = true;
                p.Start();

                break;

            case Tags.PING:
                time = (int) (DateTimeOffset.Now.ToUnixTimeMilliseconds() - when);
                break;

            case Tags.OBJECTS_NUMBER:
                number = ushort.Parse(msg.Msg.ToStringUtf8());

                if (number != null && objects.Count >= (ushort)number)
                {
                    GameObject go2 = GameObject.FindGameObjectWithTag("LoadingScreen");

                    if (go2 == null) return;

                    go2.SetActive(false);

                }

                break;

            case Tags.PLAYER_SPAWN:
                onSpawn(msg);
                break;

            case Tags.PLAYER_DESPAWN:
                onDespawn(msg);
                break;

            case Tags.PLAYER_MOVE:
                onMove(msg);
                break;

            case Tags.DESTRUCTIBLE_SPAWN:
                OnDestructibleSpawn(msg);
                break;

            case Tags.INDESTRUCTIBLE_SPAWN:
                OnIndestructibleSpawn(msg);
                break;

            case Tags.DESTRUCTIBLE_SHOT:
                OnDestructibleShot(msg);
                break;

            case Tags.DESTRUCTIBLE_DESPAWN:
                OnDestructibleDespawn(msg);
                break;

            case Tags.BULLET_SHOT:
                OnPlayerShot(msg);
                break;

            case Tags.ITEM_SPAWN:
                OnItemSpawn(msg);
                break;

            case Tags.ITEM_DESPAWN:
                OnItemDespawn(msg);
                break;

            case Tags.PLAYER_INFO:
                OnInfo(msg);
                break;

            case Tags.BULLET_DESTRUCTIBLE_HIT:
                OnBulletDestructibleHit(msg);
                break;

            case Tags.PLAYER_DESTRUCTIBLE_SHOT:
                OnPlayerDestructibleShot(msg);
                break;

            case Tags.PLAYER_PLAYER_SHOT:
                OnPlayerPlayerShot(msg);
                break;

            case Tags.ZONE_ATTACK:
                OnZoneAttack(msg);
                break;

        }

    }

    public void onSpawn(Message msg) {
        PlayerSpawnMessage psm = new PlayerSpawnMessage(msg.Msg);

        GameObject go = Instantiate(player, psm.pos, new Quaternion(0f, 0f, psm.z, psm.w));

        PlayerController pc = go.GetComponent<PlayerController>();
        pc.id = psm.id;
        pc.SetHelmet(psm.helmet);
        pc.SetGun(psm.gun);
        pc.client = this;

        if (id != null && (ushort)id == psm.id)
        {

            pc.controllable = true;
            pc.Start();

        }

        players.Add(psm.id, pc);

    }

    public void onDespawn(Message msg){
        PlayerDespawnMessage pdm = new PlayerDespawnMessage(msg.Msg);
        if (!players.ContainsKey(pdm.id)) return;

        PlayerController pc = players[pdm.id];
        Destroy(pc.gameObject);
        if (id != null && pdm.id == (ushort)id)
        {

            pc.setHealth(0);
            foreach (Transform t in GameObject.FindGameObjectWithTag("Canvas").transform) {

                if (t.name == "DeathScreen") {

                    t.gameObject.SetActive(true);

                }

            }

        }

        players.Remove(pdm.id);

    }

    public void onMove(Message msg) {
        PlayerMoveMessage pmm = new PlayerMoveMessage(msg.Msg);
        if (!players.ContainsKey(pmm.id)) return;

        PlayerController pc = players[pmm.id];

        pc.lastPosition = pc.transform.position;
        pc.lastRotation = pc.transform.rotation;

        pc.toPosition = pmm.pos;
        pc.toRotation = new Quaternion(0f, 0f, pmm.z, pmm.w);

    }

    public void OnInfo(Message msg)
    {
        PlayerInfoMessage pim = new PlayerInfoMessage(msg.Msg);
        if (!players.ContainsKey(pim.id)) return;

        PlayerController pc = players[pim.id];

        if (pim.gun != pc.gun) pc.SetGun(pim.gun);
        if (pim.bullets != pc.bullets) pc.bullets = pim.bullets;
        if (pim.health != pc.health) pc.setHealth(pim.health);
        if (pim.helmet != pc.helmet) pc.SetHelmet(pim.helmet);

    }

    public void OnDestructibleSpawn(Message msg) {
        DestructibleSpawnMessage dsm = new DestructibleSpawnMessage(msg.Msg);
        if (objects.ContainsKey(dsm.id)) return;

        GameObject go = GameObject.Instantiate(prefabs[dsm.t.ToString()], dsm.pos, Quaternion.identity);
        Destructible d = go.GetComponent<Destructible>();
        d.id = dsm.id;
        d.setShots(dsm.shots);
        objects.Add(dsm.id, go);

        if (number != null && objects.Count >= (ushort)number)
        {
            GameObject go2 = GameObject.FindGameObjectWithTag("LoadingScreen");

            if (go2 == null) return;

            go2.SetActive(false);

        }

    }

    public void OnIndestructibleSpawn(Message msg){
        IndestructibleSpawnMessage ism = new IndestructibleSpawnMessage(msg.Msg);
        if (objects.ContainsKey(ism.id)) return;

        objects.Add(ism.id, GameObject.Instantiate(prefabs[ism.t.ToString()], ism.pos, Quaternion.identity));

        if (number != null && objects.Count >= (ushort)number)
        {
            GameObject go2 = GameObject.FindGameObjectWithTag("LoadingScreen");

            if (go2 == null) return;

            go2.SetActive(false);

        }

    }

    public void OnDestructibleShot(Message msg)
    {
        DestructibleShotMessage dsm = new DestructibleShotMessage(msg.Msg);
        if (!objects.ContainsKey(dsm.id)) return;

        Destructible d = objects[dsm.id].GetComponent<Destructible>();
        d.setShots(d.shots + 1);

    }

    public void OnDestructibleDespawn(Message msg)
    {
        DestructibleDespawnMessage ddm = new DestructibleDespawnMessage(msg.Msg);
        if (!objects.ContainsKey(ddm.id)) return;

        Destroy(objects[ddm.id]);
        objects.Remove(ddm.id);

    }

    public void OnPlayerShot(Message msg)
    {
        BulletShotMessage psm = new BulletShotMessage(msg.Msg);
        if (!players.ContainsKey(psm.id)) return;

        players[psm.id].playGun(true, psm.bullet);

    }

    public void OnItemSpawn(Message msg)
    {
        ItemSpawnMessage ism = new ItemSpawnMessage(msg.Msg);
        if (objects.ContainsKey(ism.id)) return;

        GameObject go = GameObject.Instantiate(prefabs[ism.t.ToString()], ism.pos, Quaternion.identity);
        go.GetComponent<ItemController>().id = ism.id;
        objects.Add(ism.id, go);

        if (number != null && objects.Count >= (ushort)number)
        {
            GameObject go2 = GameObject.FindGameObjectWithTag("LoadingScreen");

            if (go2 == null) return;

            go2.SetActive(false);

        }

    }

    public void OnItemDespawn(Message msg)
    {
        ItemDespawnMessage idm = new ItemDespawnMessage(msg.Msg);
        if (!objects.ContainsKey(idm.id)) return;

        Destroy(objects[idm.id]);
        objects.Remove(idm.id);

    }

    public void OnBulletDestructibleHit(Message msg) {
        BulletDestructibleHitMessage bdhm = new BulletDestructibleHitMessage(msg.Msg);
        if (!objects.ContainsKey(bdhm.dest)) return;

        if (bullets.ContainsKey(bdhm.id)) {

            Destroy(bullets[bdhm.id]);
            bullets.Remove(bdhm.id);

        }

        Destructible d = objects[bdhm.dest].GetComponent<Destructible>();
        d.setShots(d.shots + 1);

    }

    public void OnPlayerDestructibleShot(Message msg)
    {
        PlayerDestructibleShotMessage pdsm = new PlayerDestructibleShotMessage(msg.Msg);
        if ((pdsm.b && !objects.ContainsKey(pdsm.dest)) || !players.ContainsKey(pdsm.id)) return;

        players[pdsm.id].playGun();

        if (pdsm.b)
        {
            Destructible d = objects[pdsm.dest].GetComponent<Destructible>();
            d.setShots(d.shots + 1);
        }

    }

    public void OnPlayerPlayerShot(Message msg)
    {
        PlayerPlayerShotMessage ppsm = new PlayerPlayerShotMessage(msg.Msg);
        if (!players.ContainsKey(ppsm.id) || !players.ContainsKey(ppsm.enemy)) return;

        players[ppsm.id].playGun();

        PlayerController pc = players[ppsm.enemy];
        GameObject.Instantiate(pc.blood, pc.transform.position, Quaternion.identity, pc.transform);

    }

    public void OnZoneAttack(Message msg) {
        ZoneAttackMessage zam = new ZoneAttackMessage(msg.Msg);
        if (!players.ContainsKey(zam.id)) return;

        PlayerController pc = players[zam.id];
        ushort health = (ushort) (pc.health - zam.damage);
        if (health <= 0 || health > 100) health = 0;
        pc.setHealth(health);
        GameObject.Instantiate(pc.blood, pc.transform.position, Quaternion.identity, pc.transform);

    }

    public void Disconnect()
    {

        disconnected = true;
        peer.DisconnectNow(0);

        client.Flush();
        client.Dispose();

        Library.Deinitialize();

    }

    void OnApplicationQuit()
    {

        Disconnect();

    }

    private IEnumerator Pinger()
    {
        Text ping = GameObject.FindGameObjectWithTag("Ping").GetComponent<Text>();

        while (true)
        {
            if (disconnected) break;

            when = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Send(new Message() { MsgType = Tags.PING }, netEvent.ChannelID);

            if (time != null) ping.text = "Задержка: " + (int) time + " мс";

            yield return new WaitForSeconds(0.8f);
        }

    }

    private IEnumerator Zoner()
    {

        while (true) {
            if (disconnected) break;

            if (id != null)
            {
                if (!players.ContainsKey((ushort)id)) break;

                PlayerController p = players[(ushort)id];
                CircleCollider2D col = GameObject.FindGameObjectWithTag("Circle").GetComponent<CircleCollider2D>();
                if (!col.IsTouching(p.gameObject.GetComponent<CircleCollider2D>()))
                    Send(new ZoneAttackMessage(p.id, 0), netEvent.ChannelID, PacketFlags.Reliable);

            }

            yield return new WaitForSeconds(1f);
        }

    }

}
