using System;
using System.Reflection;
using BeauData;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Metadata
{
    public abstract class RSMethodInfo : RSInfo
    {
        protected readonly MethodInfo m_MethodInfo;
        internal readonly MethodBinding m_MethodSettings = new MethodBinding();

        protected readonly ParameterInfo[] m_Parameters;
        protected readonly object[] m_CachedArguments;

        public RSParameterInfo[] Parameters { get; private set; }
        public RSMemberFlags Flags { get; protected set; }

        internal RSValue[] TempArgStorage;

        public RSMethodInfo(RSMemberAttribute inAttribute, MethodInfo inMethod) : base(inAttribute, inMethod)
        {
            m_MethodInfo = inMethod;

            m_Parameters = inMethod.GetParameters();
            m_CachedArguments = new object[m_Parameters.Length];
        }

        protected override void ConstructTooltip(System.Text.StringBuilder ioBuilder)
        {
            base.ConstructTooltip(ioBuilder);

            if (Parameters.Length > 0)
            {
                ioBuilder.Append("\n - Parameters:");
                foreach (var parameter in Parameters)
                {
                    ioBuilder.Append("\n   + ").Append(parameter.Tooltip);
                }
            }
        }

        internal bool PrepContext(IRSRuntimeEntity inTarget, ExecutionScope inScope, out object outThis)
        {
            return m_MethodSettings.Bind(inTarget, m_CachedArguments, inScope, out outThis);
        }

        internal void PrepArguments(RSValue[] inArgs, ExecutionScope inScope)
        {
            if (inArgs != null && inArgs.Length > 0)
            {
                for (int i = 0; i < inArgs.Length && i + m_MethodSettings.EditorArgsStartIndex < m_CachedArguments.Length; ++i)
                {
                    int idx = m_MethodSettings.EditorArgsStartIndex + i;
                    m_CachedArguments[idx] = RSInterop.ToObject(m_Parameters[idx].ParameterType, inArgs[i], inScope);
                }
            }
        }

        internal void GenerateParameters(RSTypeAssembly inAssembly)
        {
            int totalArgs = m_Parameters.Length - m_MethodSettings.EditorArgsStartIndex;
            Parameters = new RSParameterInfo[totalArgs];
            for (int i = 0; i < totalArgs; ++i)
            {
                int idx = i + m_MethodSettings.EditorArgsStartIndex;
                ParameterInfo paramInfo = m_Parameters[idx];
                RSParameterAttribute paramAttribute = paramInfo.GetCustomAttribute<RSParameterAttribute>();
                Parameters[i] = new RSParameterInfo(paramAttribute, paramInfo);
                Parameters[i].Link(inAssembly);
            }

            TempArgStorage = new RSValue[totalArgs];
        }

        public override JSON Export()
        {
            JSON baseExport = base.Export();
            JSON parameters = JSON.CreateArray();
            foreach (var paramInfo in Parameters)
            {
                JSON paramJSON = paramInfo.Export();
                parameters.Add(paramJSON);
            }
            baseExport["parameters"] = parameters;
            return baseExport;
        }
    }
}