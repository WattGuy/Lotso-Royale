using Google.Protobuf;
using System;
using System.IO;

public class ProtobufEncoding
{

    public static byte[] Encode(Google.Protobuf.IMessage msg)
    {
        using (MemoryStream rawOutput = new MemoryStream())
        {
            CodedOutputStream output = new CodedOutputStream(rawOutput);
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
                Console.WriteLine(exception.Message);
            }
        }
        else
        {
            Console.WriteLine("gg");
        }

        return message;
    }

}