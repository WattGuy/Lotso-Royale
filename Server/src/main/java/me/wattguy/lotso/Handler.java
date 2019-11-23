package me.wattguy.lotso;

import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.objects.Player;
import me.wattguy.lotso.packets.*;
import me.wattguy.lotso.utils.Utils;

public class Handler extends ChannelInboundHandlerAdapter {

    public Player p;
    public ChannelHandlerContext ctx = null;

    public Handler(Player p){
        this.p = p;
    }

    @Override
    public void channelActive(ChannelHandlerContext ctx){
        System.out.println("#" + p.getId() + " подключился к серверу!");

        this.ctx = ctx;
        Main.handlers.put(ctx, this);

        // ИНФОРМАЦИЯ
        Utils.packetInstance(ClientInfoPacket.class, p).write(p.getId(), Main.map.objects.size());

        // ИГРОКИ
        for(Player o : Main.players.values()){

            // ОТПРАВЛЯЕМ ВСЕМ О СПАВНЕ НОВОГО ИГРОКА
            Utils.packetInstance(PlayerSpawnPacket.class, o).write(p.getId(), p.getPosition().x, p.getPosition().y, p.getRotation().z, p.getRotation().w, p.getGun(), p.getHelmet());

            // ОТПРАВЛЯЕМ НОВОМУ ИГРОКУ О ИГРОКАХ КРОМЕ СЕБЯ (патамуша выше пасылаица)
            if (o == p) continue;

            Utils.packetInstance(PlayerSpawnPacket.class, p).write(o.getId(), o.getPosition().x, o.getPosition().y, o.getRotation().z, o.getRotation().w, o.getGun(), o.getHelmet());

        }

        Main.map.onConnect(p);

    }

    @Override
    public void channelInactive(ChannelHandlerContext ctx) {
        System.out.println("#" + p.getId() + " отключился от сервера!");

        for(Player p : Main.players.values()){

            Utils.packetInstance(ObjectDespawnPacket.class, p).write(this.p.getId(), ObjectType.PLAYER);

        }

        Main.handlers.remove(ctx);
        p.getCircle().dispose();
        Main.players.remove(p.getId());
        Player.ider.add(p.getId());
    }

    @Override
    public void channelRead(ChannelHandlerContext ctx, Object o) {
        if (!(o instanceof HotMessage.Packet)) return;

        HotMessage.Packet pt = (HotMessage.Packet) o;

        //System.out.println(p.getId() + " -> " + pt.getId() + ": " + pt.getMsg());

        Packet p2 = Utils.messageToPacket(pt, p);
        if (p2 != null) {
            try { p2.read(pt.getMsg()); }
            catch(Exception ignored) { }
        }

    }

    @Override
    public void channelReadComplete(ChannelHandlerContext ctx) {
        ctx.flush();
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) {
        cause.printStackTrace();
    }

}
