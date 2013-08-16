using System;
using Craft.Net.Networking;
using Classic = Craft.Net.Classic.Networking;

namespace RetroCraft.ModernHandlers
{
    public static class PacketHandlers
    {
        public static void Register(Proxy proxy)
        {
            proxy.RegisterPacketHandler(HandshakePacket.PacketId, LoginHandlers.Handshake);
            proxy.RegisterPacketHandler(EncryptionKeyResponsePacket.PacketId, LoginHandlers.EncryptionKeyResponse);
            proxy.RegisterPacketHandler(ClientStatusPacket.PacketId, LoginHandlers.ClientStatus);

            proxy.RegisterPacketHandler(ChatMessagePacket.PacketId, ChatMessage);
            proxy.RegisterPacketHandler(ServerListPingPacket.PacketId, ServerListPing);

            proxy.RegisterPacketHandler(RightClickPacket.PacketId, RightClick);

            proxy.RegisterPacketHandler(PlayerPositionAndLookPacket.PacketId, PlayerPositionAndLook);
            proxy.RegisterPacketHandler(PlayerPositionPacket.PacketId, PlayerPosition);
        }

        private static void PlayerPosition(RemoteClient client, Proxy proxy, IPacket _packet)
        {
            var packet = (PlayerPositionPacket)_packet;
            client.SendClassicPacket(new Classic.PositionAndOrientationPacket(-1, (short)packet.X, (short)packet.Y, (short)packet.Z, client.Yaw, client.Pitch));
        }

        private static void PlayerPositionAndLook(RemoteClient client, Proxy proxy, IPacket _packet)
        {
            var packet = (PlayerPositionAndLookPacket)_packet;
            client.SendClassicPacket(new Classic.PositionAndOrientationPacket(-1, (short)packet.X, (short)packet.Y, (short)packet.Z, (byte)packet.Yaw, (byte)packet.Pitch));
        }

        private static void RightClick(RemoteClient client, Proxy proxy, IPacket _packet)
        {
            var packet = (RightClickPacket)_packet;
            short x = (short)packet.X;
            short y = packet.Y;
            short z = (short)packet.Z;
            switch (packet.Direction)
            {
                case 0:
                    y -= 1;
                    break;
                case 1:
                    y += 1;
                    break;
                case 2:
                    z -= 1;
                    break;
                case 3:
                    z += 1;
                    break;
                case 4:
                    x -= 1;
                    break;
                case 5:
                    x += 1;
                    break;
            }
            Console.WriteLine(String.Format("X:{0}, Y: {1}, Z: {2}, BlockID:{3}", x, y, z, packet.HeldItem.Id));
            client.SendClassicPacket(new Classic.ClientSetBlockPacket(x,y,z, true, (byte)packet.HeldItem.Id));
        }

        public static void ChatMessage(RemoteClient client, Proxy proxy, IPacket _packet)
        {
            var packet = (ChatMessagePacket)_packet;
            if (packet.Message.StartsWith("//"))
                proxy.HandleCommand(packet.Message, client);
            else
                client.SendClassicPacket(new Classic.ChatMessagePacket(packet.Message, -1));
        }

        public static void ServerListPing(RemoteClient client, Proxy proxy, IPacket _packet)
        {
            client.SendPacket(new DisconnectPacket(GetPingValue(proxy)));
        }

        private static string GetPingValue(Proxy proxy)
        {
            return "§1\0" + PacketReader.ProtocolVersion + "\0" +
                PacketReader.FriendlyVersion + "\0RetroCraft Proxy\00\00";
        }
    }
}

