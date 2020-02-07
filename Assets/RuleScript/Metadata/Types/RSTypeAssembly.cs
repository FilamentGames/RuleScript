using System;
using System.Collections.Generic;
using RuleScript.Data;

namespace RuleScript.Metadata
{
    internal sealed class RSTypeAssembly
    {
        private readonly Dictionary<Type, RSTypeInfo> m_Types = new Dictionary<Type, RSTypeInfo>();
        private readonly List<RSTypeAssembly> m_Dependencies = new List<RSTypeAssembly>();

        public RSTypeAssembly(params RSTypeAssembly[] inDependencies)
        {
            m_Dependencies.AddRange(inDependencies);
        }

        public void AddTypeMeta(RSTypeInfo inType)
        {
            m_Types.Add(inType.SystemType, inType);
        }

        public void AddDependency(RSTypeAssembly inAssembly)
        {
            if (!m_Dependencies.Contains(inAssembly))
                m_Dependencies.Add(inAssembly);
        }

        public RSTypeInfo GetTypeMeta(Type inType)
        {
            RSTypeInfo meta;
            if (TryGetMeta(inType, out meta))
                return meta;

            if (TryCreateMeta(inType, out meta))
                return meta;

            throw new InvalidOperationException(string.Format("Unable to locate or create metadata for type {0}", inType.Name));
        }

        private bool TryGetMeta(Type inType, out RSTypeInfo outMetadata)
        {
            if (m_Types.TryGetValue(inType, out outMetadata))
                return true;

            foreach (var assembly in m_Dependencies)
            {
                if (assembly.TryGetMeta(inType, out outMetadata))
                    return true;
            }

            return false;
        }

        private bool TryCreateMeta(Type inType, out RSTypeInfo outMetadata)
        {
            if (inType.IsEnum)
            {
                Enum defaultVal = (Enum) Enum.ToObject(inType, 0);
                outMetadata = new RSTypeInfo(inType, null, RSValue.FromEnum(defaultVal));
                outMetadata.InitializeEnum();
                return true;
            }

            outMetadata = null;
            return false;
        }
    }
}