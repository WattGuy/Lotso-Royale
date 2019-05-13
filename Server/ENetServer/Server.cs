using ENet;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;

public class Server
{

    public static ushort port = 4296;
    public static string ip = "127.0.0.1";
    public static ushort slots = 15;

    public static Host server;
    public static Event netEvent;

    public static int tickrate = 20;

    public static Thread ReadThread;

    public static bool on = true;
    public static Scheduler ticker;

    public static Ider ider = new Ider();

    public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
    public static Dictionary<ushort, Bullet> bullets = new Dictionary<ushort, Bullet>();
    public static Dictionary<ushort, Scheduler> schedulers = new Dictionary<ushort, Scheduler>();
    public static MapManager map = new MapManager();
    public static ConfigManager configs = new ConfigManager();

    public static List<Vector2> points = new List<Vector2>();

    static void Main(string[] args)
    {
        Console.Title = "Lotso 1.0";
        configs.Initialize();
        Library.Initialize();

        server = new Host();
        Address address = new Address
        {
            Port = port
        };

        ticker = new Scheduler(() =>
        {

            foreach (Player p in players.Values) {
                if (!(p.npos != null && p.nrotation != null)) continue;
                bool updated = false;

                if (!p.npos.Equals(p.pos)) {

                    p.pos = p.npos;
                    updated = true;

                }

                if (!p.nrotation.Equals(p.rotation))
                {

                    p.rotation = p.nrotation;
                    updated = true;

                }

                if (!updated) continue;

                PlayerMoveMessage pmm = new PlayerMoveMessage(p.id, p.pos, p.rotation);

                foreach (Player o in players.Values.Where(x => x != p)) {

                    Send(o.peer, pmm, 0);

                }

            }

        }).RunTimer((double) 1 / tickrate);

        server.Create(address, 100);

        Console.WriteLine("Server started on " + ip + ":" + port);

        ReadThread = new Thread(new ThreadStart(Read));
        ReadThread.Start();

        while (on)
        {
            bool polled = false;

            while (!polled)
            {
                if (server.CheckEvents(out netEvent) <= 0)
                {
                    if (server.Service(15, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                switch (netEvent.Type)
                {

                    case EventType.None:
                        break;

                    case EventType.Connect:
                        Console.WriteLine("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        if (players.Count == slots)
                        {

                            netEvent.Peer.DisconnectNow(0);

                        }
                        else
                        {
                            OnConnect();
                            map.OnConnect();
                        }
                        break;

                    case EventType.Timeout:
                    case EventType.Disconnect:
                        Console.WriteLine("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        OnDisconnect();
                        break;

                    case EventType.Receive:
                        OnReceive();
                        netEvent.Packet.Dispose();
                        break;

                }
            }
        }

        server.Flush();

    }

    public static void Read()
    {

        while (true) {

            string s = Console.ReadLine().ToLower();
            if (s.StartsWith("exit") || s.StartsWith("stop"))
            {

                on = false;
                break;

            } else if (s.StartsWith("item")) {
                string[] ss = s.Split(' ');

                if (ss[1].Equals("add") && ss.Length == 5) {

                    Console.WriteLine("Предмет добавлен! ID: " + map.addItem(Item.FromString(ss[2].ToUpper()), new Vector2(float.Parse(ss[3]), float.Parse(ss[4]))).id);

                } else if (ss[1].Equals("remove") && ss.Length == 3) {
                    ushort id = ushort.Parse(ss[2]);

                    if (!map.objects.ContainsKey(id))
                    {

                        Console.WriteLine("Предмет с таким ID не найден");

                    }
                    else {

                        map.removeItem(id);
                        Console.WriteLine("Предмет #" + id + " удалён!");

                    }

                }

            }

        }

    }

    public static void OnConnect()
    {

        Vector2 point;
        if (points.Count - 1 >= players.Count) point = points[players.Count];
        else point = new Vector2(0f, 0f);

        Player p = new Player((ushort)netEvent.Peer.ID, netEvent.Peer, point);
        players.Add(p.id, p);
        PlayerSpawnMessage message = new PlayerSpawnMessage(p.id, p.pos, p.rotation, p.gun, p.helmet);

        Send(p.peer, new Message() { Msg = ByteString.CopyFromUtf8("" + p.id), MsgType = Tags.ID }, netEvent.ChannelID, PacketFlags.Reliable);

        foreach (Player o in players.Values)
        {

            Send(o.peer, message, netEvent.ChannelID, PacketFlags.Reliable);

        }

        foreach (Player o in players.Values.Where(x => x.id != p.id))
        {

            PlayerSpawnMessage message2 = new PlayerSpawnMessage(o.id, o.pos, o.rotation, o.gun, o.helmet);
            Send(p.peer, message2, netEvent.ChannelID, PacketFlags.Reliable);

        }

    }

    public static void OnDisconnect()
    {

        players.Remove((ushort) netEvent.Peer.ID);
        PlayerDespawnMessage message = new PlayerDespawnMessage((ushort)netEvent.Peer.ID);

        foreach (Player p in players.Values)
        {

            Send(p.peer, message, netEvent.ChannelID, PacketFlags.Reliable);

        }

    }

    public static void Send(Peer p, IMessage msg, byte channel) { Send(p, msg, channel, PacketFlags.None); }

    public static void Send(Peer p, IMessage msg, byte channel, PacketFlags flag)
    {
        Console.WriteLine(msg.GetType().ToString() + " -> " + p.ID);
        Send(p, new Message { MsgType = msg.GetTag(), Msg = msg.ToByteString() }, channel, flag);
    }

    public static void Send(Peer p, Message msg, byte channel) { Send(p, msg, channel, PacketFlags.None); }

    public static void Send(Peer p, Message msg, byte channel, PacketFlags flag)
    {
        Packet packet = default(Packet);
        if (flag == PacketFlags.None) packet.Create(ProtobufEncoding.Encode(msg));
        else packet.Create(ProtobufEncoding.Encode(msg), flag);

        p.Send(channel, ref packet);
    }

    public static void OnReceive()
    {
        byte[] bs = new byte[1024];
        netEvent.Packet.CopyTo(bs);
        Message msg = ProtobufEncoding.Decode(bs) as Message;

        switch (msg.MsgType)
        {

            case Tags.PING:
                Send(netEvent.Peer, new Message() { MsgType = Tags.PING }, netEvent.ChannelID);
                break;

            case Tags.PLAYER_MOVE:
                OnMove(msg);
                break;

            case Tags.BULLET_SHOT:
                OnShot(msg);
                break;

            case Tags.DESTRUCTIBLE_SHOT:
                OnDestructibleShot(msg);
                break;

            case Tags.PLAYER_PICKUP:
                OnPickup(msg);
                break;

            case Tags.BULLET_DESTRUCTIBLE_HIT:
                OnBulletDestructibleHit(msg);
                break;

            case Tags.BULLET_PLAYER_HIT:
                OnBulletPlayerHit(msg);
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

    public static void OnMove(Message msg) {
        PlayerMoveMessage pmm = new PlayerMoveMessage(msg.Msg);

        Player p = players[(ushort) netEvent.Peer.ID];
        p.npos = pmm.pos;
        p.nrotation = pmm.rotation;

    }

    public static void OnShot(Message msg) {
        BulletShotMessage psm = new BulletShotMessage(msg.Msg);
        psm.bullet = ider.getId();

        Player p = players[(ushort)netEvent.Peer.ID];
       
        if (p.bullets > 0)
        {

            foreach (Player o in players.Values)
            {

                Send(o.peer, psm, netEvent.ChannelID, PacketFlags.Reliable);

            }

        }

        bool infos = false;
        if (p.bullets - 1 <= 0)
        {
            p.bullets = 0;
            infos = true;
            p.gun = Gun.ARMS;

            PlayerInfoMessage pim = new PlayerInfoMessage(p.id, p.gun, p.bullets, p.health, p.helmet);

            foreach (Player o in players.Values)
            {

                Send(o.peer, pim, netEvent.ChannelID);

            }

        }
        else p.bullets--;

        bullets.Add(psm.bullet, new Bullet(psm.bullet, p.id));

        schedulers.Add(psm.bullet, new Scheduler(() => {

            bullets.Remove(psm.bullet);

        }).RunLater(2f));

        if (!infos) Send(netEvent.Peer, new PlayerInfoMessage(p.id, p.gun, p.bullets, p.health, p.helmet), netEvent.ChannelID, PacketFlags.Reliable);

    }

    public static void OnPlayerDestructibleShot(Message msg)
    {
        PlayerDestructibleShotMessage pdsm = new PlayerDestructibleShotMessage(msg.Msg);
        if (pdsm.b && !map.objects.ContainsKey(pdsm.dest)) return;

        if (pdsm.b)
        {

            Destructible dest = map.objects[pdsm.dest] as Destructible;
            dest.shots++;
            if (dest.shots >= (ushort)dest.t)
            {
                dest.Check();
                map.objects.Remove(pdsm.dest);
                DestructibleDespawnMessage ddm = new DestructibleDespawnMessage(pdsm.dest);

                foreach (Player p in players.Values)
                {

                    Send(p.peer, ddm, netEvent.ChannelID, PacketFlags.Reliable);

                }

                return;

            }

        }

        foreach (Player o in players.Values)
        {

            Send(o.peer, pdsm, netEvent.ChannelID, PacketFlags.Reliable);

        }

    }

    public static void OnPlayerPlayerShot(Message msg)
    {
        PlayerPlayerShotMessage ppsm = new PlayerPlayerShotMessage(msg.Msg);
        if (!players.ContainsKey(ppsm.id) || !players.ContainsKey(ppsm.enemy)) return;

        Player p = players[ppsm.enemy];
        ushort damage = Utils.getDamage(Gun.ARMS);

        foreach (Player o in players.Values)
        {

            Send(o.peer, ppsm, netEvent.ChannelID, PacketFlags.Reliable);

        }

        if (p.health - damage <= 0)
        {
            p.Death();
            return;
        }
        else p.health -= damage;

        PlayerInfoMessage pim = new PlayerInfoMessage(p.id, p.gun, p.bullets, p.health, p.helmet);
        Send(p.peer, pim, netEvent.ChannelID, PacketFlags.Reliable);

    }

    public static void OnDestructibleShot(Message msg)
    {
        DestructibleShotMessage dsm = new DestructibleShotMessage(msg.Msg);
        if (!map.objects.ContainsKey(dsm.id)) return;

        object o = map.objects[dsm.id];

        if (!(o is Destructible)) return;

        Destructible dest = (Destructible) o;

        Vector2 v2 = players[(ushort)netEvent.Peer.ID].pos;
        if (Utils.Distance(v2, dest.pos) > 3f) {

            return;

        }

        dest.shots++;
        if (dest.shots >= (ushort) dest.t) {
            dest.Check();
            map.objects.Remove(dsm.id);
            DestructibleDespawnMessage ddm = new DestructibleDespawnMessage(dsm.id);

            foreach (Player p in players.Values)
            {

                Send(p.peer, ddm, netEvent.ChannelID, PacketFlags.Reliable);

            }

            return;

        }

        foreach (Player p in players.Values)
        {

            Send(p.peer, dsm, netEvent.ChannelID, PacketFlags.Reliable);

        }

    }

    public static void OnPickup(Message msg)
    {
        PlayerPickupMessage ppm = new PlayerPickupMessage(msg.Msg);
        if (!map.objects.ContainsKey(ppm.id) || !(map.objects[ppm.id] is Item) || !players.ContainsKey((ushort) netEvent.Peer.ID)) return;

        Item i = (Item) map.objects[ppm.id];
        Player p = players[(ushort) netEvent.Peer.ID];
        if (Utils.Distance(i.pos, p.pos) > 3) return;

        i.AddTo(p);
        map.objects.Remove(ppm.id);
        ItemDespawnMessage idm = new ItemDespawnMessage(ppm.id);

        foreach (Player o in players.Values) {

            Send(o.peer, idm, netEvent.ChannelID, PacketFlags.Reliable);

        }

    }

    public static void OnBulletDestructibleHit(Message msg) {
        BulletDestructibleHitMessage bdhm = new BulletDestructibleHitMessage(msg.Msg);
        if (!map.objects.ContainsKey(bdhm.dest) || !bullets.ContainsKey(bdhm.id)) return;

        bullets.Remove(bdhm.id);
        Destructible dest = map.objects[bdhm.dest] as Destructible;

        dest.shots++;
        if (dest.shots >= (ushort)dest.t)
        {
            dest.Check();
            map.objects.Remove(bdhm.dest);
            DestructibleDespawnMessage ddm = new DestructibleDespawnMessage(bdhm.dest);

            foreach (Player p in players.Values)
            {

                Send(p.peer, ddm, netEvent.ChannelID, PacketFlags.Reliable);

            }

            return;

        }

        foreach (Player p in players.Values)
        {

            Send(p.peer, bdhm, netEvent.ChannelID, PacketFlags.Reliable);

        }

    }

    public static void OnBulletPlayerHit(Message msg)
    {
        BulletPlayerHitMessage bphm = new BulletPlayerHitMessage(msg.Msg);
        if (!players.ContainsKey(bphm.player) || !bullets.ContainsKey(bphm.id)) return;

        Player p = players[bullets[bphm.id].by];
        Player enemy = players[bphm.player];
        ushort damage = Utils.getDamage(p.gun);

        bullets.Remove(bphm.id);

        if (enemy.health - damage <= 0)
        {
            enemy.Death();
            return;
        }
        else enemy.health -= damage;

        Send(enemy.peer, new PlayerInfoMessage(enemy.id, enemy.gun, enemy.bullets, enemy.health, enemy.helmet), netEvent.ChannelID, PacketFlags.Reliable);

    }

    public static void OnZoneAttack(Message msg) {
        ZoneAttackMessage zam = new ZoneAttackMessage(msg.Msg);
        if (!players.ContainsKey(zam.id)) return;

        zam.damage = 3;
        Player p = players[zam.id];

        foreach (Player o in players.Values)
        {

            Send(o.peer, zam, netEvent.ChannelID, PacketFlags.Reliable);

        }

        if (p.health - zam.damage <= 0)
        {
            p.Death();
            return;
        }
        else p.health -= zam.damage;

        PlayerInfoMessage pim = new PlayerInfoMessage(p.id, p.gun, p.bullets, p.health, p.helmet);
        Send(p.peer, pim, netEvent.ChannelID, PacketFlags.Reliable);

    }

    public class Ider {

        public ushort id = 0;

        public ushort getId()
        {

            id += 1;
            return (ushort) (id - 1);

        }

    }

   

}