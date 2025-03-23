using Strawberry.Graphics;

namespace Strawberry.Serialization
{
    public static class SerializeTemplateRegistry
    {
        static Dictionary<Type, SerializeTemplate> templates;

        static SerializeTemplateRegistry()
        {
            templates = new Dictionary<Type, SerializeTemplate>();
            RegisterSerializerForType<Math.Vector2>(new Vector2SerializeTemplate());
            RegisterSerializerForType<Color>(new ColorSerializeTemplate());
            RegisterSerializerForType<bool>(new BooleanSerializeTemplate());
            RegisterSerializerForType<short>(new Int16SerializeTemplate());
            RegisterSerializerForType<int>(new Int32SerializeTemplate());
            RegisterSerializerForType<long>(new Int64SerializeTemplate());
            RegisterSerializerForType<float>(new FloatSerializeTemplate());
            RegisterSerializerForType<double>(new DoubleSerializeTemplate());
            RegisterSerializerForType<ushort>(new UInt16SerializeTemplate());
            RegisterSerializerForType<uint>(new UInt32SerializeTemplate());
            RegisterSerializerForType<ulong>(new UInt64SerializeTemplate());
        }

        public static void RegisterSerializerForType<T>(SerializeTemplate template)
        {
            templates.Add(typeof(T), template);
        }

        public static SerializeTemplate GetSerializerForType<T>()
        {
            if (templates.ContainsKey(typeof(T)))
                return templates[typeof(T)];
            return null;
        }

        public static bool IsSerializerRegistered<T>()
        {
            return templates.ContainsKey(typeof(T));
        }

        public static void RegisterSerializerForType(Type t, SerializeTemplate template)
        {
            templates.Add(t, template);
        }

        public static SerializeTemplate GetSerializerForType(Type t)
        {
            if (templates.ContainsKey(t))
                return templates[t];
            return null;
        }


        public static bool IsSerializerRegistered(Type t)
        {
            if (t == null)
                return false;
            return templates.ContainsKey(t);
        }

        public static byte[] Serialize(Type t, object obj)
        {
            byte[] bytes = null;
            if (templates.ContainsKey(t))
            {
                bytes = templates[t].GetBytes(obj);
            }

            return bytes;
        }

        public static bool Deserialize(Type t, byte[] bytes, out object obj)
        {
            obj = t.GetDefault();
            if (templates.ContainsKey(t))
            {
                obj = templates[t].GetObjectBack(bytes);
                return true;
            }

            return false;
        }

        public static byte[] Serialize<T>(T obj)
        {
            byte[] bytes = null;
            if (templates.ContainsKey(typeof(T)))
            {
                bytes = templates[typeof(T)].GetBytes(obj);
            }

            return bytes;
        }

        public static bool Deserialize<T>(byte[] bytes, out T obj)
        {
            obj = default(T);
            if (templates.ContainsKey(typeof(T)))
            {
                obj = (T)templates[typeof(T)].GetObjectBack(bytes);
                return true;
            }

            return false;
        }
    }
}
