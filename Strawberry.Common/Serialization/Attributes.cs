/*
 * Strawberry Game Engine
 * File: Attributes.cs
 * Author: Koosha Aabedini Nassab
 *
 * Serialization attributes used by the engine serializer.
 */

namespace Strawberry.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DoNotSerializeAttribute : Attribute
    {

    }
}
