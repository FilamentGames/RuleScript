using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using RuleScript.Data;
using RuleScript.Runtime;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Configuration for a method binding.
    /// </summary>
    internal class MethodBinding
    {
        public Type ComponentType;

        public InvocationTarget ThisParam;
        public InvocationTarget FirstParam;
        public InvocationTarget SecondParam;

        public bool IsInstance;
        public bool IsExtension;
        public int EditorArgsStartIndex;

        public Type BoundType;

        public void Configure(MethodInfo inMethod, ParameterInfo[] inParameters, bool inbAllowUnboundParams)
        {
            EditorArgsStartIndex = 0;

            if (inMethod.IsStatic)
            {
                ThisParam = InvocationTarget.Null;
                bool bIsExtension = inParameters.Length > 0 && inMethod.IsDefined(typeof(ExtensionAttribute));
                if (bIsExtension)
                {
                    IsExtension = true;
                    BoundType = inParameters[0].ParameterType;

                    if (!TryAssignTarget(inParameters[0].ParameterType, ref FirstParam, ref ComponentType))
                        throw InvalidBindException("Cannot bind extension method target", inParameters[0].ParameterType, inMethod);

                    ++EditorArgsStartIndex;
                    if (inParameters.Length > 1)
                    {
                        if (!TryAssignContext(inParameters[1].ParameterType, ref SecondParam))
                        {
                            if (!inbAllowUnboundParams)
                                throw InvalidBindException("Cannot bind extension method context", inParameters[1].ParameterType, inMethod);
                            return;
                        }

                        ++EditorArgsStartIndex;
                        if (!inbAllowUnboundParams && inParameters.Length > 2)
                            throw InvalidArgumentCountException("Invalid parameter count for query extension method", inParameters.Length, 2, inMethod);
                    }
                }
                else
                {
                    BoundType = null;

                    if (inParameters.Length > 0)
                    {
                        if (!TryAssignContext(inParameters[0].ParameterType, ref FirstParam))
                        {
                            if (!inbAllowUnboundParams)
                                throw InvalidBindException("Cannot bind static method context", inParameters[0].ParameterType, inMethod);
                            return;
                        }

                        ++EditorArgsStartIndex;
                        if (!inbAllowUnboundParams && inParameters.Length > 1)
                            throw InvalidArgumentCountException("Invalid parameter count for static method", inParameters.Length, 1, inMethod);
                    }
                }
            }
            else
            {
                IsInstance = true;
                BoundType = inMethod.DeclaringType;

                if (!TryAssignTarget(inMethod.DeclaringType, ref ThisParam, ref ComponentType))
                    throw InvalidBindException("Cannot bind instance method target", inMethod.DeclaringType, inMethod);

                if (inParameters.Length > 0)
                {
                    if (TryAssignContext(inParameters[0].ParameterType, ref FirstParam))
                    {
                        ++EditorArgsStartIndex;
                    }
                    else
                    {
                        if (!inbAllowUnboundParams)
                            throw InvalidBindException("Cannot bind instance method context", inParameters[0].ParameterType, inMethod);
                    }
                }
            }
        }

        public bool Bind(IRSRuntimeEntity inEntity, object[] ioArguments, IScriptContext inContext, out object outThis)
        {
            if (IsExtension)
            {
                if (ioArguments.Length == 0)
                {
                    outThis = null;
                    return false;
                }
            }

            if (ioArguments.Length > 0)
            {
                Bind(FirstParam, inEntity, inContext, ref ioArguments[0]);
                if (ioArguments.Length > 1)
                {
                    Bind(SecondParam, inEntity, inContext, ref ioArguments[1]);
                }
            }

            outThis = null;
            Bind(ThisParam, inEntity, inContext, ref outThis);

            if (IsExtension)
            {
                return !FirstParam.MustBeNotNull() || ioArguments[0] != null;
            }
            else if (IsInstance)
            {
                return outThis != null;
            }
            else
            {
                return true;
            }
        }

        private void Bind(InvocationTarget inTarget, IRSRuntimeEntity inEntity, IScriptContext inContext, ref object ioValue)
        {
            switch (inTarget)
            {
                case InvocationTarget.Entity:
                    ioValue = inEntity;
                    break;
                case InvocationTarget.Component:
                    ioValue = inEntity?.GetRSComponent(ComponentType);
                    break;
                case InvocationTarget.Context:
                    ioValue = inContext;
                    break;
            }
        }

        static private InvalidOperationException InvalidBindException(string inString, Type inType, MethodInfo inMethod)
        {
            return new InvalidOperationException(string.Format("{0} (type={1}) (method={2})", inString, inType.Name, inMethod.MemberName()));
        }

        static private InvalidOperationException InvalidArgumentCountException(string inString, int inArgumentCount, int inDesiredArgumentCount, MethodInfo inMethod)
        {
            return new InvalidOperationException(string.Format("{0} (expected {1}, got {2}) (method={3})", inString, inDesiredArgumentCount, inArgumentCount, inMethod.MemberName()));
        }

        static private bool TryAssignTarget(Type inType, ref InvocationTarget ioTarget, ref Type ioComponentType)
        {
            if (typeof(IRSEntity).IsAssignableFrom(inType))
            {
                ioTarget = InvocationTarget.Entity;
                return true;
            }
            else if (typeof(IRSComponent).IsAssignableFrom(inType))
            {
                ioTarget = InvocationTarget.Component;
                ioComponentType = inType;
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool TryAssignContext(Type inType, ref InvocationTarget ioTarget)
        {
            if (typeof(IScriptContext).IsAssignableFrom(inType))
            {
                ioTarget = InvocationTarget.Context;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}