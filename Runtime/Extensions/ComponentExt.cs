using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class ComponentExt
    {
        private const BindingFlags MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly Dictionary<Type, MemberCache> MemberCacheByType = new();

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            if (go == null)
                throw new ArgumentNullException(nameof(go));

            if (toAdd == null)
                throw new ArgumentNullException(nameof(toAdd));

            return go.AddComponent<T>().GetCopyOf(toAdd);
        }

        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            if (comp == null)
                throw new ArgumentNullException(nameof(comp));

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var type = comp.GetType();
            if (type != other.GetType())
                throw new ArgumentException($"Type mismatch. Destination type '{type.Name}' does not match source type '{other.GetType().Name}'.", nameof(other));

            var cache = GetMemberCache(type);

            for (var i = 0; i < cache.Fields.Length; i++)
            {
                var field = cache.Fields[i];
                if (field.IsInitOnly)
                    continue;

                try
                {
                    field.SetValue(comp, field.GetValue(other));
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"GetCopyOf skipped field '{type.Name}.{field.Name}': {exception.Message}");
                }
            }

            for (var i = 0; i < cache.Properties.Length; i++)
            {
                var property = cache.Properties[i];
                try
                {
                    property.SetValue(comp, property.GetValue(other, null), null);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"GetCopyOf skipped property '{type.Name}.{property.Name}': {exception.Message}");
                }
            }

            return comp as T;
        }

        private static MemberCache GetMemberCache(Type type)
        {
            if (MemberCacheByType.TryGetValue(type, out var cache))
                return cache;

            cache = BuildMemberCache(type);
            MemberCacheByType[type] = cache;
            return cache;
        }

        private static MemberCache BuildMemberCache(Type type)
        {
            var fields = type.GetFields(MemberFlags);
            var properties = type.GetProperties(MemberFlags);

            var writableProperties = new List<PropertyInfo>(properties.Length);
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (!property.CanRead || !property.CanWrite)
                    continue;

                if (property.GetIndexParameters().Length > 0)
                    continue;

                writableProperties.Add(property);
            }

            return new MemberCache(fields, writableProperties.ToArray());
        }

        private sealed class MemberCache
        {
            public readonly FieldInfo[] Fields;
            public readonly PropertyInfo[] Properties;

            public MemberCache(FieldInfo[] fields, PropertyInfo[] properties)
            {
                Fields = fields;
                Properties = properties;
            }
        }
    }
}
