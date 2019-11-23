package me.wattguy.lotso;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.channel.socket.nio.NioSocketChannel;
import io.netty.handler.codec.protobuf.ProtobufDecoder;
import io.netty.handler.codec.protobuf.ProtobufEncoder;
import io.netty.handler.codec.protobuf.ProtobufVarint32FrameDecoder;
import io.netty.handler.codec.protobuf.ProtobufVarint32LengthFieldPrepender;
import me.wattguy.lotso.enums.ItemType;
import me.wattguy.lotso.managers.ConfigManager;
import me.wattguy.lotso.managers.MapManager;
import me.wattguy.lotso.objects.Player;
import me.wattguy.lotso.packets.*;
import me.wattguy.lotso.utils.Ider;
import me.wattguy.lotso.utils.Rotation;
import me.wattguy.lotso.utils.Utils;
import me.wattguy.lotso.utils.Vector2;

import java.util.HashMap;
import java.util.Scanner;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import com.jogamp.opengl.GL;

public class Main {

    public static ConfigManager configs;

    public static final Float BULLET_SPEED = 6000f;

    public static HashMap<Integer, Class<? extends Packet>> packets = new HashMap<Integer, Class<? extends Packet>>(){{

        put(1, PingPacket.class);
        put(2, PlayerSpawnPacket.class);
        put(3, ClientInfoPacket.class);
        put(4, ObjectSpawnPacket.class);
        put(5, PlayerMovePacket.class);
        put(6, PlayerShotPacket.class);
        put(7, ObjectDespawnPacket.class);
        put(8, PlayerInfoPacket.class);
        put(9, CircleInfoPacket.class);
        put(10, PlayerPickupPacket.class);
        put(11, ObjectInfoPacket.class);

    }};

    public static HashMap<Integer, Player> players = new HashMap<>();
    public static HashMap<ChannelHandlerContext, Handler> handlers = new HashMap<>();
    public static Ider ider;
    public static MapManager map;

    public static boolean DEBUG = false;
    public static short TCP_PORT = 4296;
    public static int TIMEOUT = 5000;
    public static int PING_TIME = 200;

    public static ScheduledExecutorService circle;

    public static void main(String[] args) {
        configs = new ConfigManager();
        map = new MapManager();
        map.initialize();
        ider = new Ider();
        configs.map();

        try {

            new Thread(Main::tcp).start();

        }catch(Exception ex){
            ex.printStackTrace();
        }

        new Thread(Main::in).start();

        circle = Executors.newSingleThreadScheduledExecutor();
        circle.scheduleAtFixedRate(() -> {

            for(Player p : players.values()){
                if(map.circle.inByDistance(p.getPosition()) || p.thatsAll()) continue;

                p.setHealth(p.getHealth() - 3);

                Utils.writeAll(PlayerInfoPacket.class, p.getId(), p.getGun(), p.getHelmet(), p.getHealth(), p.getBullets());

            }

        }, 1, 1, TimeUnit.SECONDS);

        System.out.println("Сервер запущен | " + TCP_PORT);

    }

    private static void tcp() {
        EventLoopGroup bossGroup = new NioEventLoopGroup();
        EventLoopGroup workerGroup = new NioEventLoopGroup();

        try {
            ServerBootstrap b = new ServerBootstrap();
            b.group(bossGroup, workerGroup)
                    .channel(NioServerSocketChannel.class)
                    .childHandler(new ChannelInitializer<NioSocketChannel>(){

                        @Override
                        public void initChannel(NioSocketChannel ch) {
                            ch.pipeline().addLast(new ProtobufVarint32FrameDecoder());
                            ch.pipeline().addLast(new ProtobufDecoder(HotMessage.Packet.getDefaultInstance()));
                            ch.pipeline().addLast(new ProtobufVarint32LengthFieldPrepender());
                            ch.pipeline().addLast(new ProtobufEncoder());

                            Integer id = Player.ider.next();
                            Player p = new Player(id, new Vector2(), new Rotation());
                            Handler h = new Handler(p);
                            p.setHandler(h);
                            players.put(id, p);
                            ch.pipeline().addLast(h);
                        }

                    })
                    .childOption(ChannelOption.SO_KEEPALIVE, true)
                    .childOption(ChannelOption.TCP_NODELAY, true)
                    .bind(TCP_PORT).sync().channel().closeFuture().sync();
        } catch (InterruptedException e) {
            e.printStackTrace();
        } finally {
            workerGroup.shutdownGracefully();
            bossGroup.shutdownGracefully();
        }
    }

    private static void in(){

        while(true){

            String[] args = new Scanner(System.in).nextLine().split(" ");
            if (args.length <= 0) continue;
            
            try {

            // Изменение позиции круга
            // zone x y scale
            if(args[0].equalsIgnoreCase("zone") && args.length >= 4){

            	Float x = Float.parseFloat(args[1]);
                Float y = Float.parseFloat(args[2]);
                Float scale = Float.parseFloat(args[3]);

                map.circle.setPosition(new Vector2(x, y));
                map.circle.setScale(scale);

                for(Player p : players.values()){

                    map.sendCircle(p);

                }

                System.out.println("Зона успешно изменена!");

            }
            // Выдача предмета
            // item player item [number]
            else if (args[0].equalsIgnoreCase("item") && args.length >= 3){
            	
            	Player p = null;
            	try {
            		
            		Integer i = Integer.parseInt(args[1]);
            		
            		if (!players.containsKey(i)) throw new Exception();
            		
            		p = players.get(i);
            		
            	}
            	catch(Exception ignored) {
            		
            		System.out.println("Данный игрок не существует!");
            		continue;
            		
            	}

            	ItemType it = ItemType.fromString(args[2].toUpperCase());
            	if (it == null) {
            		
            		System.out.println("Неверно введен предмет!");
            		continue;
            		
            	}
            	
            	p.pickupItem(it);
            	
            	Integer i = null;
            	if (args.length >= 4) {
            		
                	try {
                		
                		i = Integer.parseInt(args[3]);
                		
                	}
                	catch(Exception ignored) {}
                	
                	if (i != null) {
                		
                		for (int g = 1; g <= i - 1; g++) {
                			
                			p.pickupItem(it);
                			
                		}
                		
                	}
            		
            	}

                if (i == null) System.out.println("Предмет '" + it.toString() + "' успешно выдан игроку #" + p.getId() + "!");
                else System.out.println("Предмет '" + it.toString() + "' успешно выдан игроку #" + p.getId() + " в количестве " + i + "!");

            }
            
            }catch(Exception ignored){}

        }

    }

}
