using System;
using System.Collections.Generic;
using BeauUtil;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Runtime;
using RuleScript.Validation;
using UnityEngine;

[assembly : System.Runtime.CompilerServices.InternalsVisibleTo("RuleScript.Editor")]

namespace RuleScript
{
    static public class ScriptUtils
    {
        /// <summary>
        /// Attempts to match a string on the given identifier.
        /// Will accept wildcard character at the start or end.
        /// </summary>
        static public bool StringMatch(string inIdentifier, string inSearch)
        {
            return StringUtils.WildcardMatch(inIdentifier, inSearch);
        }

        /// <summary>
        /// Caches an object.
        /// </summary>
        static public ObjectType CacheObject<ObjectType, StorageType>(ref StorageType ioObject) where StorageType : class where ObjectType : class, StorageType, new()
        {
            if (ioObject == null || ioObject.GetType() != typeof(ObjectType))
            {
                ioObject = new ObjectType();
            }
            return (ObjectType) ioObject;
        }

        /// <summary>
        /// Enumerates all component types for the given runtime entity.
        /// </summary>
        static public IEnumerable<Type> GetRSComponentTypes(IRSRuntimeEntity inEntity)
        {
            Assert.True(inEntity != null, "Cannot get component types");
            foreach (var component in inEntity.GetRSComponents())
            {
                yield return component.GetType();
            }
        }

        /// <summary>
        /// Returns a new unique id.
        /// </summary>
        static public string NewId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns a hash of the given string.
        /// </summary>
        static public int Hash(string inId)
        {
            return Animator.StringToHash(inId);
        }

        #region Persistence

        #region Persist

        /// <summary>
        /// Generates persistent data from the given runtime entity.
        /// </summary>
        static public RSPersistEntityData Persist(RSLibrary inLibrary, IRSRuntimeEntity inEntity, int inFlags)
        {
            if (inEntity == null)
                return null;

            return Persist(inLibrary, inEntity, inEntity.GetRSComponents(), inFlags);
        }

        /// <summary>
        /// Generates persistent data from the given runtime entity and its components.
        /// </summary>
        static public RSPersistEntityData Persist(RSLibrary inLibrary, IRSRuntimeEntity inEntity, IReadOnlyList<IRSRuntimeComponent> inComponents, int inFlags)
        {
            RSPersistEntityData data = null;
            Persist(inLibrary, inEntity, inComponents, inFlags, ref data);
            return data;
        }

        /// <summary>
        /// Generates persistent data from the given runtime entity and its components.
        /// </summary>
        static public bool Persist(RSLibrary inLibrary, IRSRuntimeEntity inEntity, int inFlags, ref RSPersistEntityData outData)
        {
            if (inEntity == null)
                return false;

            return Persist(inLibrary, inEntity, inEntity.GetRSComponents(), inFlags, ref outData);
        }

        /// <summary>
        /// Generates persistent data from the given runtime entity and its components.
        /// </summary>
        static public bool Persist(RSLibrary inLibrary, IRSRuntimeEntity inEntity, IReadOnlyList<IRSRuntimeComponent> inComponents, int inFlags, ref RSPersistEntityData outData)
        {
            if (inEntity == null)
                return false;

            if (outData == null)
            {
                outData = new RSPersistEntityData();
            }

            outData.EntityId = inEntity.Id;
            outData.Active = inEntity.IsActive();
            inEntity.RuleTable?.Persist(ref outData.TableData);

            using(PooledList<IRSPersistListener> persistListeners = PooledList<IRSPersistListener>.Alloc())
            {
                IRSPersistListener entityPersistListener = inEntity as IRSPersistListener;
                if (entityPersistListener != null)
                {
                    entityPersistListener.OnPrePersist();
                    persistListeners.Add(entityPersistListener);
                }

                using(PooledList<RSPersistComponentData> persistedComponentData = PooledList<RSPersistComponentData>.Alloc())
                {
                    for (int i = 0, length = inComponents.Count; i < length; ++i)
                    {
                        IRSRuntimeComponent component = inComponents[i];
                        if (component == null)
                            continue;

                        IRSPersistListener componentListener = component as IRSPersistListener;
                        if (componentListener != null)
                        {
                            componentListener.OnPrePersist();
                            persistListeners.Add(componentListener);
                        }

                        RSComponentInfo componentInfo = inLibrary.GetComponent(component.GetType());
                        if (componentInfo == null)
                            continue;

                        RSPersistComponentData compData = outData.FindComponentData(componentInfo.IdHash);
                        componentInfo.Persist(component, inFlags, ref compData);
                        persistedComponentData.Add(compData);
                    }

                    Array.Resize(ref outData.ComponentData, persistedComponentData.Count);
                    for (int i = 0; i < outData.ComponentData.Length; ++i)
                    {
                        outData.ComponentData[i] = persistedComponentData[i];
                    }
                }

                IRSCustomPersistDataProvider customDataProvider = inEntity as IRSCustomPersistDataProvider;
                if (customDataProvider != null)
                {
                    customDataProvider.GetCustomPersistData(ref outData.CustomData, inFlags);
                }

                foreach (var listener in persistListeners)
                {
                    listener.OnPostPersist();
                }
            }

            return true;
        }

        #endregion // Persist

        #region Restore

        /// <summary>
        /// Restores persistent data to the given runtime entity.
        /// </summary>
        static public bool Restore(RSEnvironment inEnvironment, RSPersistEntityData inData, IRSRuntimeEntity inEntity, int inFlags)
        {
            if (inEntity == null)
                return false;

            return Restore(inEnvironment, inData, inEntity, null, inFlags);
        }

        /// <summary>
        /// Restores persistent data to the given runtime entity and its components.
        /// </summary>
        static public bool Restore(RSEnvironment inEnvironment, RSPersistEntityData inData, IRSRuntimeEntity inEntity, IReadOnlyList<IRSRuntimeComponent> inComponents, int inFlags)
        {
            if (inEntity == null)
                return false;

            using(PooledList<IRSPersistListener> persistListeners = PooledList<IRSPersistListener>.Alloc())
            {
                IRSPersistListener entityPersistListener = inEntity as IRSPersistListener;
                if (entityPersistListener != null)
                {
                    entityPersistListener.OnPreRestore(inEnvironment);
                    persistListeners.Add(entityPersistListener);
                }

                inEntity.SetActiveWithoutNotify(inData.Active);
                inEntity.RuleTable?.Restore(inData.TableData);
                for (int i = 0, length = inData.ComponentData.Length; i < length; ++i)
                {
                    RSPersistComponentData componentData = inData.ComponentData[i];

                    RSComponentInfo componentInfo = inEnvironment.Library.GetComponent(componentData.ComponentType);
                    if (componentInfo == null)
                        continue;

                    IRSRuntimeComponent component = inEntity.GetRSComponent(componentInfo.OwnerType);
                    if (component == null)
                        continue;

                    if (inComponents != null)
                    {
                        bool bIncluded = false;
                        for (int allCompIdx = 0, allCompLength = inComponents.Count; allCompIdx < allCompLength; ++allCompIdx)
                        {
                            IRSRuntimeComponent includedComponent = inComponents[allCompIdx];
                            if (includedComponent == component)
                            {
                                bIncluded = true;
                                break;
                            }
                        }

                        if (!bIncluded)
                            continue;
                    }

                    IRSPersistListener componentListener = component as IRSPersistListener;
                    if (componentListener != null)
                    {
                        componentListener.OnPreRestore(inEnvironment);
                        persistListeners.Add(componentListener);
                    }

                    componentInfo.Restore(component, componentData, inEnvironment, inFlags);
                }

                IRSCustomPersistDataProvider customDataProvider = inEntity as IRSCustomPersistDataProvider;
                if (customDataProvider != null)
                {
                    customDataProvider.RestoreCustomPersistData(inData.CustomData, inFlags, inEnvironment);
                }

                foreach (var listener in persistListeners)
                {
                    listener.OnPostRestore(inEnvironment);
                }
            }

            return true;
        }

        #endregion // Restore

        #endregion // Persistence
    }
}