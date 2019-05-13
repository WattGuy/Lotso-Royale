using Google.Protobuf;
using System;
using System.IO;
using UnityEngine;


public class ProtobufEncoding
{

    public static byte[] Encode(Google.Protobuf.IMessage msg)
    {
        using (MemoryStream rawOutput = new MemoryStream())
        {
            CodedOutputStream output = new CodedOutputStream(rawOutput);
            //output.WriteRawVarint32((uint)len);
            output.WriteMessage(msg);
            output.Flush();
            byte[] result = rawOutput.ToArray();
            
            return result;
        }
    }

    public static Google.Protobuf.IMessage Decode(byte[] msg)
    {
        Google.Protobuf.IMessage message = null;

        CodedInputStream stream = new CodedInputStream(msg);

        int varint32 = (int)stream.ReadRawVarint32();
        if (varint32 <= (msg.Length - (int)stream.Position))
        {
            try
            {
                byte[] body = stream.ReadRawBytes(varint32);
                message = Message.Parser.ParseFrom(body);
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
            }
        }
        else
        {
            Debug.LogError("gg");
        }

        return message;
    }

    #region 异步解码
    public class PacketBuffer
    {
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
    }
    public static PacketBuffer buffer = new PacketBuffer();
    /// <summary>
    /// 将数据解码
    /// </summary>
    public static void DecodeAsync(byte[] msg)
    {
        if (msg.Length <= 0)
        {
            return;
        }
        //把收取上来的自己全部缓存到本地 buffer 中
        Array.Copy(msg, 0, buffer.Data, buffer.Length, msg.Length);
        buffer.Length += msg.Length;

        Debug.Log(string.Format("cache buff length: {0}, offset: {1}", buffer.Length, buffer.Offset));


        CodedInputStream stream = new CodedInputStream(buffer.Data);
        while (!stream.IsAtEnd)
        {
            //标记读取的Position, 在长度不够时进行数组拷贝，到下一次在进行解析
            int markReadIndex = (int)stream.Position;

            //data length,Protobuf 变长头, 也就是消息长度
            int varint32 = (int)stream.ReadRawVarint32();
            if (varint32 <= (buffer.Length - (int)stream.Position))
            {
                try
                {
                    byte[] body = stream.ReadRawBytes(varint32);

                    Message message = Message.Parser.ParseFrom(body);

                    Debug.Log("Response: " + message.ToString());

                    //TODO: dispatcher message, 这里就可以用多线程进行协议分发
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                }
            }
            else
            {
                //本次数据不够长度,缓存进行下一次解析
                byte[] dest = new byte[8192];
                int remainSize = buffer.Length - markReadIndex;
                Array.Copy(buffer.Data, markReadIndex, dest, 0, remainSize);

                /**
                 * 缓存未处理完的字节 
                 */
                buffer.Data = dest;
                buffer.Offset = 0;
                buffer.Length = remainSize;
                break;
            }
        }
    }
    #endregion
}