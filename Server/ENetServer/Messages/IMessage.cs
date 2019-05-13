using Google.Protobuf;

public interface IMessage
{

    ushort GetTag();

    string ToString();

    ByteString ToByteString();

}
