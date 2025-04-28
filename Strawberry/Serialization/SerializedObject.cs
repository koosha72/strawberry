using System.Text;

namespace Strawberry.Serialization
{
    public class SerializedObject
    {
        byte[] data;

        public SerializedObject(ReferenceObject obj)
        {
            SBSerializer s = new SBSerializer();
            data = s.Serialize(obj, new Dictionary<ulong, ReferenceObject>());
        }

        public void RestoreObject(ReferenceObject obj)
        {
            if (data != null)
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                    {
                        int pos = sizeof(int);
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
                        if (obj.GetType() != t)
                            return;
                        int propCount = reader.ReadInt32();
                        for (int j = 0; j < propCount; j++)
                        {
                            string propTypeName = reader.ReadString();
                            Type propType = Type.GetType(propTypeName, (name) =>
                            {
                                // Returns the assembly of the type by enumerating loaded assemblies
                                // in the app domain            
                                return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                            },
                            null,
                            true);
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
                                byte createNew = reader.ReadByte();
                                ulong val = reader.ReadUInt64();
                                if (createNew == 1)
                                {
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
                        stream.Seek(pos, SeekOrigin.Begin);
                    }
                }
            }
        }
    }
}
