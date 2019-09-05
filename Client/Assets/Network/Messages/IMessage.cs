using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class IMessage
{

    public static Dictionary<Type, List<Type>> types = new Dictionary<Type, List<Type>>();
    public List<object> objects = new List<object>();

    public IMessage() { }

    public IMessage(ByteString s)
    {
        Initialize(s.ToStringUtf8());
    }

    public IMessage(string s)
    {
        Initialize(s);
    }

    public void Initialize(string s) {
        string[] split = s.Split('|');
        List<Type> types = GetTypes();

        if (split.Length != types.Count) return;

        for (int i = 0; i < split.Length; i++) {

            if (types[i] == typeof(string))
            {

                objects.Add(split[i]);

            }
            else if (types[i] == typeof(byte))
            {

                try { objects.Add(byte.Parse(split[i])); }
                catch (Exception ignored) { return; }

            }
            else if (types[i] == typeof(int))
            {

                try { objects.Add(int.Parse(split[i])); }
                catch (Exception ignored) { return; }

            }
            else if (types[i] == typeof(float))
            {

                CultureInfo ci = (CultureInfo) CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";

                try { objects.Add(float.Parse(split[i], NumberStyles.Any, ci)); }
                catch (Exception ignored) { return; }

            }
            else if (types[i] == typeof(Gun)) {

                try { objects.Add(Enum.Parse(typeof(Gun), split[i])); }
                catch (Exception ignored) { return; }

            }
            else if (types[i] == typeof(ObjectType))
            {

                try { objects.Add(Enum.Parse(typeof(ObjectType), split[i])); }
                catch (Exception ignored) { return; }

            }

        }

    }

    public List<Type> GetTypes() {
        return types[GetType()];
    }

    public static void SetTypes(Type t, Type[] ts) {
        types.Add(t, ts.ToList());
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        List<Type> types = GetTypes();

        for (int i = 0; i < objects.Count; i++) {

            if (types[i] == typeof(float))
                sb.Append(string.Format("{0:0.0######}", (float) objects[i]).Replace(',', '.'));
            else
                sb.Append(objects[i]);

            if (i != objects.Count - 1) {

                sb.Append("|");

            }

        }

        return sb.ToString();

    }

    public ByteString ToByteString() {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    public abstract void Read();
    public void ReadSync()
    {
        UnityThread.executeInUpdate(() => Read());
    }

    public abstract void Write();

    public void WriteSync()
    {
        UnityThread.executeInUpdate(() => Read());
    }

}
