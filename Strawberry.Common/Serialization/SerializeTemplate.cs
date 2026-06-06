/*
 * Strawberry Game Engine
 * File: SerializeTemplate.cs
 * Author: Koosha Aabedini Nassab
 *
 * Abstract serializer template for engine serialization.
 */

namespace Strawberry.Serialization
{
    public abstract class SerializeTemplate
    {
        public abstract byte[] GetBytes(object obj);

        public abstract object GetObjectBack(byte[] bytes);
    }
}
