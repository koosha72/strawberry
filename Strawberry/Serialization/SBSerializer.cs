using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Serialization
{
    public static class Extensions
    {
        public static MethodInfo GetSetMethodOnDeclaringType(this PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod(true);
            return methodInfo ?? propertyInfo
                                    .DeclaringType
                                    .GetProperty(propertyInfo.Name)
                                    .GetSetMethod(true);
        }

        public static object GetDefault(this Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }

    public class ReferenceHelper
    {
        public PropertyInfo Property { get; set; }

        public ReferenceObject Object { get; set; }
    }

    public class SBSerializer
    {
        byte[] data;
        Dictionary<ulong, ReferenceObject> objects = new Dictionary<ulong, ReferenceObject>();
        Dictionary<ulong, ReferenceObject> deserializedObjects = new Dictionary<ulong, ReferenceObject>();

        public byte[] Data
        {
            get { return data; }
        }


        public byte[] Serialize(ReferenceObject obj, Dictionary<ulong, ReferenceObject> availableObjects)
        {
            byte[] result;
            if (objects.ContainsKey(obj.UniqueId))
                return null;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(mem, Encoding.UTF8))
                {
                    if (obj is ReferenceObject)
                    {
                        Type t = obj.GetType();
                        writer.Write(t.AssemblyQualifiedName);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.UTF8))
                            {
                                var props = t.GetProperties();
                                int propCount = 0;
                                foreach (var prop in props)
                                {
                                    if (prop.GetSetMethodOnDeclaringType() != null && !prop.GetCustomAttributes(true).Any(x => x is DoNotSerializeAttribute))
                                    {
                                        if (prop.PropertyType.IsPrimitive)
                                        {
                                            bw.Write(prop.PropertyType.AssemblyQualifiedName);
                                            bw.Write(prop.Name);
                                            byte[] propValueBytes = (byte[])typeof(BitConverter)
                                              .GetMethod("GetBytes", new Type[] { prop.PropertyType })
                                                .Invoke(null, new object[] { prop.GetValue(obj) });
                                            bw.Write(BitConverter.GetBytes(propValueBytes.Length), 0, sizeof(int));
                                            bw.Write(propValueBytes, 0, propValueBytes.Length);
                                            propCount++;
                                        }
                                        else if (prop.PropertyType == typeof(string))
                                        {
                                            string str = (string)prop.GetValue(obj);
                                            if (str != null)
                                            {
                                                bw.Write(prop.PropertyType.AssemblyQualifiedName);
                                                bw.Write(prop.Name);

                                                bw.Write(str);
                                                propCount++;
                                            }
                                        }
                                        else if (prop.PropertyType.IsEnum)
                                        {
                                            bw.Write(prop.PropertyType.AssemblyQualifiedName);
                                            bw.Write(prop.Name);
                                            bw.Write(prop.GetValue(obj).ToString());
                                            propCount++;
                                        }
                                        else if (prop.PropertyType.IsSubclassOf(typeof(ReferenceObject)))
                                        {
                                            if (prop.GetValue(obj) != null)
                                            {
                                                var uId = (prop.GetValue(obj) as ReferenceObject).UniqueId;
                                                bw.Write(typeof(ReferenceObject).AssemblyQualifiedName);
                                                bw.Write(prop.Name);
                                                if (availableObjects.ContainsKey(uId))
                                                    bw.Write((byte)1);
                                                else
                                                    bw.Write((byte)0);
                                                bw.Write(uId);
                                                propCount++;
                                            }
                                        }
                                        else if (SerializeTemplateRegistry.IsSerializerRegistered(prop.PropertyType))
                                        {
                                            bw.Write(prop.PropertyType.AssemblyQualifiedName);
                                            bw.Write(prop.Name);
                                            byte[] bytes = SerializeTemplateRegistry.Serialize(prop.PropertyType, prop.GetValue(obj));
                                            bw.Write(bytes.Length);
                                            bw.Write(bytes);
                                            propCount++;
                                        }
                                    }
                                }
                                writer.Write(propCount);

                                writer.Write(stream.ToArray(), 0, (int)stream.Length);
                            }
                        }
                    }
                    result = mem.ToArray();
                    objects.Add(obj.UniqueId, obj);
                }
                return result;
            }
        }

        public void Serialize(IList refs)
        {
            List<ReferenceObject> objects = new List<ReferenceObject>();
            Dictionary<ulong, ReferenceObject> objs = new Dictionary<ulong, ReferenceObject>();
            foreach (var r in refs)
            {
                ReferenceObject obj = r as ReferenceObject;
                objects.Add(obj);
                objs.Add(obj.UniqueId, obj);
            }
            this.objects.Clear();
            using (MemoryStream mem = new MemoryStream())
            {
                using (MemoryStream tempMem = new MemoryStream())
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        var obj = objects[i];
                        if (obj is ReferenceObject)
                        {
                            List<ReferenceObject> oo;
                            byte[] bytes = Serialize(obj as ReferenceObject, objs);
                            if (bytes != null)
                            {
                                tempMem.Write(BitConverter.GetBytes(bytes.Length + sizeof(int)), 0, sizeof(int));
                                tempMem.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }

                    mem.Write(BitConverter.GetBytes(objects.Count), 0, sizeof(int));
                    mem.Write(tempMem.ToArray(), 0, (int)tempMem.Length);
                    data = mem.ToArray();
                }
            }
            this.objects.Clear();
            objs.Clear();
        }

        private ReferenceObject SingleDeserialize(byte[] data)
        {
            ReferenceObject obj;
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                {
                    int size = reader.ReadInt32();
                    string typeName = reader.ReadString();
                    byte[] bbb = Encoding.UTF8.GetBytes(typeName);

                    Type t = Type.GetType(typeName, (name) =>
                    {
                        // Returns the assembly of the type by enumerating loaded assemblies
                        // in the app domain            
                        return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                    },
                    null,
                    true);
                    obj = (ReferenceObject)Activator.CreateInstance(t);
                    int propCount = reader.ReadInt32();
                    for (int j = 0; j < propCount; j++)
                    {
                        string propTypeName = reader.ReadString();
                        Type propType = Type.GetType(propTypeName);
                        string propName = reader.ReadString();
                        if (propType == typeof(string))
                        {
                            string val = reader.ReadString();
                            t.GetProperty(propName).SetValue(obj, val);
                        }
                        else if (propType.IsEnum)
                        {
                            string val = reader.ReadString();
                            t.GetProperty(propName).SetValue(obj, Enum.Parse(propType, val));
                        }
                        else if (propName == "UniqueId")
                        {
                            reader.ReadInt32();
                            ulong val = reader.ReadUInt64();
                        }
                        else if (propType == typeof(ReferenceObject))
                        {
                            ulong val = reader.ReadUInt64();
                            var r = ReferenceObject.FindObjectById(val);
                            if (r != null)
                                t.GetProperty(propName).SetValue(obj, r);
                        }
                        else
                        {
                            if (SerializeTemplateRegistry.IsSerializerRegistered(propType))
                            {
                                int length = reader.ReadInt32();
                                byte[] bytes = reader.ReadBytes(length);
                                object o;
                                if (SerializeTemplateRegistry.Deserialize(propType, bytes, out o))
                                    t.GetProperty(propName).SetValue(obj, o);
                            }
                        }
                    }
                }
            }

            return obj;
        }

        public List<ReferenceObject> Deserialize()
        {
            deserializedObjects.Clear();
            List<ReferenceObject> objects = new List<ReferenceObject>();
            Dictionary<ulong, List<ReferenceHelper>> waitingRefs = new Dictionary<ulong, List<ReferenceHelper>>();
            int count = 0;
            if (data != null)
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                    {
                        count = reader.ReadInt32();
                        int pos = sizeof(int);
                        for (int i = 0; i < count; i++)
                        {
                            int size = reader.ReadInt32();
                            string typeName = reader.ReadString();
                            byte[] bbb = Encoding.UTF8.GetBytes(typeName);

                            Type t = Type.GetType(typeName, (name) =>
                            {
                                // Returns the assembly of the type by enumerating loaded assemblies
                                // in the app domain            
                                return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                            },
                            null,
                            true);
                            ReferenceObject obj = (ReferenceObject)Activator.CreateInstance(t);
                            objects.Add(obj);
                            int propCount = reader.ReadInt32();
                            for (int j = 0; j < propCount; j++)
                            {
                                string propTypeName = reader.ReadString();
                                string propName = reader.ReadString();
                                Type propType = Type.GetType(propTypeName, (name) =>
                                {
                                    // Returns the assembly of the type by enumerating loaded assemblies
                                    // in the app domain            
                                    return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                                },
                                null,
                                true);
                                if (propType == typeof(string))
                                {
                                    string val = reader.ReadString();
                                    t.GetProperty(propName).SetValue(obj, val);
                                }
                                else if (propType.IsEnum)
                                {
                                    string val = reader.ReadString();
                                    t.GetProperty(propName).SetValue(obj, Enum.Parse(propType, val));
                                }
                                else if (propName == "UniqueId")
                                {
                                    reader.ReadInt32();
                                    ulong val = reader.ReadUInt64();
                                    deserializedObjects.Add(val, obj);
                                    if (waitingRefs.ContainsKey(val))
                                    {
                                        foreach (var rh in waitingRefs[val])
                                            rh.Property.SetValue(rh.Object, obj);
                                        waitingRefs.Remove(val);
                                    }
                                }
                                else if (propType == typeof(ReferenceObject))
                                {
                                    byte createNew = reader.ReadByte();
                                    ulong val = reader.ReadUInt64();
                                    if (createNew == 1)
                                    {
                                        if (deserializedObjects.ContainsKey(val))
                                            t.GetProperty(propName).SetValue(obj, deserializedObjects[val]);
                                        else
                                        {
                                            if (!waitingRefs.ContainsKey(val))
                                                waitingRefs.Add(val, new List<ReferenceHelper>());
                                            waitingRefs[val].Add(new ReferenceHelper() { Property = t.GetProperty(propName), Object = obj });
                                        }
                                    }
                                    else
                                        t.GetProperty(propName).SetValue(obj, ReferenceObject.FindObjectById(val));
                                }
                                else
                                {
                                    if (SerializeTemplateRegistry.IsSerializerRegistered(propType))
                                    {
                                        int length = reader.ReadInt32();
                                        byte[] bytes = reader.ReadBytes(length);
                                        object o;
                                        if (t.GetProperty(propName) != null)
                                        {
                                            if (SerializeTemplateRegistry.Deserialize(propType, bytes, out o))
                                                t.GetProperty(propName).SetValue(obj, o);
                                        }
                                    }
                                }
                            }
                            pos += size;
                            stream.Seek(pos, SeekOrigin.Begin);
                        }
                    }
                }
            }
            int r = 0;
            if (waitingRefs.Count > 0)
            {
                foreach (var l in waitingRefs.Values)
                    r += l.Count;
            }
            if (r > 0)
                System.Diagnostics.Debug.WriteLine("Cannot handle {0} references", r);

            deserializedObjects.Clear();
            return objects;
        }

        public List<ReferenceObject> Deserialize(byte[] data)
        {
            deserializedObjects.Clear();
            List<ReferenceObject> objects = new List<ReferenceObject>();
            Dictionary<ulong, List<ReferenceHelper>> waitingRefs = new Dictionary<ulong, List<ReferenceHelper>>();
            int count = 0;
            if (data != null)
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                    {
                        count = reader.ReadInt32();
                        int pos = sizeof(int);
                        for (int i = 0; i < count; i++)
                        {
                            int size = reader.ReadInt32();
                            string typeName = reader.ReadString();
                            byte[] bbb = Encoding.UTF8.GetBytes(typeName);

                            Type t = Type.GetType(typeName, (name) =>
                            {
                                // Returns the assembly of the type by enumerating loaded assemblies
                                // in the app domain            
                                return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                            },
                            null,
                            true);
                            ReferenceObject obj = (ReferenceObject)Activator.CreateInstance(t);
                            objects.Add(obj);
                            int propCount = reader.ReadInt32();
                            for (int j = 0; j < propCount; j++)
                            {
                                string propTypeName = reader.ReadString();
                                string propName = reader.ReadString();
                                Type propType = Type.GetType(propTypeName, (name) =>
                                {
                                    // Returns the assembly of the type by enumerating loaded assemblies
                                    // in the app domain            
                                    return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                                },
                                null,
                                true);
                                if (propType == typeof(string))
                                {
                                    string val = reader.ReadString();
                                    t.GetProperty(propName).SetValue(obj, val);
                                }
                                else if (propType.IsEnum)
                                {
                                    string val = reader.ReadString();
                                    t.GetProperty(propName).SetValue(obj, Enum.Parse(propType, val));
                                }
                                else if (propName == "UniqueId")
                                {
                                    reader.ReadInt32();
                                    ulong val = reader.ReadUInt64();
                                    deserializedObjects.Add(val, obj);
                                    if (waitingRefs.ContainsKey(val))
                                    {
                                        foreach (var rh in waitingRefs[val])
                                            rh.Property.SetValue(rh.Object, obj);
                                        waitingRefs.Remove(val);
                                    }
                                }
                                else if (propType == typeof(ReferenceObject))
                                {
                                    byte createNew = reader.ReadByte();
                                    ulong val = reader.ReadUInt64();
                                    if (createNew == 1)
                                    {
                                        if (deserializedObjects.ContainsKey(val))
                                            t.GetProperty(propName).SetValue(obj, deserializedObjects[val]);
                                        else
                                        {
                                            if (!waitingRefs.ContainsKey(val))
                                                waitingRefs.Add(val, new List<ReferenceHelper>());
                                            waitingRefs[val].Add(new ReferenceHelper() { Property = t.GetProperty(propName), Object = obj });
                                        }
                                    }
                                    else
                                        t.GetProperty(propName).SetValue(obj, ReferenceObject.FindObjectById(val));
                                }
                                else
                                {
                                    if (SerializeTemplateRegistry.IsSerializerRegistered(propType))
                                    {
                                        int length = reader.ReadInt32();
                                        byte[] bytes = reader.ReadBytes(length);
                                        object o;
                                        if (SerializeTemplateRegistry.Deserialize(propType, bytes, out o))
                                            t.GetProperty(propName).SetValue(obj, o);
                                    }
                                }
                            }
                            pos += size;
                            stream.Seek(pos, SeekOrigin.Begin);
                        }
                    }
                }
            }
            int r = 0;
            if (waitingRefs.Count > 0)
            {
                foreach (var l in waitingRefs.Values)
                    r += l.Count;
            }
            if (r > 0)
                System.Diagnostics.Debug.WriteLine("Cannot handle {0} references", r);

            deserializedObjects.Clear();
            return objects;
        }
    }
}
