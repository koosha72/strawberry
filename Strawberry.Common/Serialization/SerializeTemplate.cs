namespace Strawberry.Serialization
{
    public abstract class SerializeTemplate
    {
        public abstract byte[] GetBytes(object obj);

        public abstract object GetObjectBack(byte[] bytes);
    }
}
