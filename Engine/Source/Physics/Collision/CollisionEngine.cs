using System;
using System.Collections.Generic;
using System.Linq;

namespace MonolithEngine
{

    /// <summary>
    /// A class responsible for checking collisions and keeping track of
    /// the entities' states from collision perspective.
    /// Does not run on it's own, each entity calls it when it needs collision
    /// information in it's own loop.
    /// </summary>
    public class CollisionEngine
    {

        private Dictionary<Type, HashSet<IColliderEntity>> allColliders = new Dictionary<Type, HashSet<IColliderEntity>>();
        private Dictionary<Type, HashSet<Type>> collisionTypeCache = new Dictionary<Type, HashSet<Type>>();
        //private Dictionary<Type, HashSet<Type>> reversedCollisionTypeCache = new Dictionary<Type, HashSet<Type>>();

        private Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>> collisions = new Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>>();

        private Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>> triggers = new Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>>();

        //private Dictionary<IColliderEntity, Dictionary<StaticCollider, bool>> gridCollisions = new Dictionary<IColliderEntity, Dictionary<StaticCollider, bool>>();

        private HashSet<IGameObject> changedObjects = new HashSet<IGameObject>();

        public CollisionEngine()
        {

        }

        /// <summary>
        /// Call this when object's state is changed from collision perspective:
        /// trigger/collider added/removed, tag added/removed, collision setting
        /// updated/removed, etc...
        /// </summary>
        /// <param name="entity"></param>
        public void ObjectChanged(IGameObject entity)
        {
            changedObjects.AddIfMissing(entity);
        }

        public void CheckCollisions(IColliderEntity thisEntity = null)
        {
            if (changedObjects.Count == 0 && allColliders.Count == 0)
            {
                return;
            }

            HandleChangedObjects();

             
            if (!thisEntity.CollisionsEnabled || !collisionTypeCache.ContainsKey(thisEntity.GetType()))
            {
                return;
            }

            foreach (Type collision in collisionTypeCache[thisEntity.GetType()])
            {
                if (!allColliders.ContainsKey(collision))
                {
                    continue;
                }
                foreach (IColliderEntity otherEntity in allColliders[collision])
                {
#if DEBUG
                    if (thisEntity.Equals(otherEntity))
                    {
                        Logger.Error("Collisions was checked against itself: " + thisEntity);
                        continue;
                    }

                    if (otherEntity.IsDestroyed)
                    {
                        Logger.Error(thisEntity + " collisions was checked against a destroyed entity: " + otherEntity);
                        continue;
                    }
#endif

                    if (otherEntity.CanFireTriggers && thisEntity.GetTriggeredAgainst().Contains(otherEntity.GetType()))
                    {
                        CheckTriggers(thisEntity, otherEntity);
                    }

                    if (!otherEntity.CollisionsEnabled)
                    {
                        continue;
                    }

                    if (thisEntity.GetCollisionComponent() != null && thisEntity.GetCollidesAgainst().ContainsKey(otherEntity.GetType()) && otherEntity.GetCollisionComponent() != null )
                    {
                        CheckCollision(thisEntity, otherEntity, thisEntity.GetCollidesAgainst()[otherEntity.GetType()]);
                    }
                }
            }
            InactivateCollisionsAndTriggers(thisEntity);
        }

        private void HandleChangedObjects()
        {
            if (changedObjects.Count > 0)
            {
                foreach (IColliderEntity changed in changedObjects)
                {
                    if (changed.IsDestroyed)
                    {
                        if (allColliders.ContainsKey(changed.GetType()))
                        {
                            allColliders[changed.GetType()].RemoveIfExists(changed);
                            if (allColliders[changed.GetType()].Count == 0)
                            {
                                allColliders.Remove(changed.GetType());
                            }
                        }
                    }
                    else
                    {
                        if (changed.GetCollisionComponent() != null)
                        {

                            if (!collisions.ContainsKey(changed))
                            {
                                collisions[changed] = new Dictionary<IColliderEntity, bool>();
                            }

                            HashSet<IColliderEntity> set = allColliders.GetOrDefault(changed.GetType(), new HashSet<IColliderEntity>());
                            set.Add(changed);
                            allColliders[changed.GetType()] = set;

                            foreach (Type t in changed.GetCollidesAgainst().Keys)
                            {
                                HashSet<Type> collisions = collisionTypeCache.GetOrDefault(changed.GetType(), new HashSet<Type>());
                                collisions.Add(t);
                                collisionTypeCache[changed.GetType()] = collisions;

                                /*collisions = reversedCollisionTypeCache.GetOrDefault(t, new HashSet<Type>());
                                collisions.Add(changed.GetType());
                                reversedCollisionTypeCache[t] = collisions;*/
                            }
                        }

                        if (changed.GetTriggers().Count > 0)
                        {
                            triggers[changed] = new Dictionary<string, Dictionary<IGameObject, bool>>();
                            foreach (ITrigger trigger in changed.GetTriggers())
                            {
                                triggers[changed][trigger.GetTag()] = new Dictionary<IGameObject, bool>();
                            }
                        }

                        foreach (Type t in changed.GetTriggeredAgainst())
                        {
                            HashSet<Type> collisions = collisionTypeCache.GetOrDefault(changed.GetType(), new HashSet<Type>());
                            collisions.Add(t);
                            collisionTypeCache[changed.GetType()] = collisions;

                            /*collisions = reversedCollisionTypeCache.GetOrDefault(t, new HashSet<Type>());
                            collisions.Add(changed.GetType());
                            reversedCollisionTypeCache[t] = collisions;*/
                        }
                    }
                }
                changedObjects.Clear();
            }
        }

        private void CheckCollision(IColliderEntity thisEntity, IColliderEntity otherObject, bool allowOverlap)
        {
            if (thisEntity.GetCollisionComponent().CollidesWith(otherObject))
            {
                if (!collisions.ContainsKey(thisEntity) || !collisions[thisEntity].ContainsKey(otherObject))
                {
                    thisEntity.CollisionStarted(otherObject, allowOverlap);
                }
                collisions[thisEntity][otherObject] = true;
            }
        }

        private void CheckTriggers(IColliderEntity thisEntity, IGameObject otherObject)
        {
            foreach (ITrigger trigger in thisEntity.GetTriggers())
            {
                if (trigger.IsInsideTrigger(otherObject))
                {
                    if (!triggers[thisEntity][trigger.GetTag()].ContainsKey(otherObject))
                    {
                        thisEntity.OnEnterTrigger(trigger.GetTag(), otherObject);
                    }
                    triggers[thisEntity][trigger.GetTag()][otherObject] = true;
                }
            }
        }

        public List<IColliderEntity> GetCollidesWith(IColliderEntity collider)
        {
            List<IColliderEntity> result = new List<IColliderEntity>();

            if (collisions.ContainsKey(collider))
            {
                foreach (IColliderEntity e in collisions[collider].Keys)
                {
                    if (collisions[collider][e])
                    {
                        result.Add(e);
                    }
                }
            }

            return result;
        }

        private void InactivateCollisionsAndTriggers(IColliderEntity toUpdate)
        {

            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                if (thisEntity.IsDestroyed)
                {
                    continue;
                }
                if (toUpdate != null && !thisEntity.Equals(toUpdate))
                {
                    continue;
                }
                foreach (IColliderEntity otherObject in collisions[thisEntity].Keys.ToList())
                {
                    if (thisEntity.Equals(otherObject))
                    {
                        continue;
                    }
                    if(!collisions[thisEntity][otherObject])
                    {
                        thisEntity.CollisionEnded(otherObject);
                        collisions[thisEntity].Remove(otherObject);
                    } else
                    {
                        collisions[thisEntity][otherObject] = false;
                    }
                }
            }

            foreach (IHasTrigger thisEntity in triggers.Keys)
            {
                if (thisEntity.IsDestroyed)
                {
                    continue;
                }
                if (toUpdate != null && !thisEntity.Equals(toUpdate))
                {
                    continue;
                }
                foreach (string tag in triggers[thisEntity].Keys)
                {
                    foreach (IHasTrigger otherEntity in triggers[thisEntity][tag].Keys.ToList())
                    {
                        if (!triggers[thisEntity][tag][otherEntity])
                        {
                            thisEntity.OnLeaveTrigger(tag, otherEntity);
                            triggers[thisEntity][tag].Remove(otherEntity);
                        } else
                        {
                            triggers[thisEntity][tag][otherEntity] = false;
                        }
                    }
                }
            }
        }


        public void PostUpdate()
        {

        }

        public void Destroy()
        {
            HandleChangedObjects();
        }
    }
}
