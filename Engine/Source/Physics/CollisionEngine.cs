using ForestPlatformerExample;
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

        private Dictionary<IColliderEntity, HashSet<IColliderEntity>> toCheckAgainst = new Dictionary<IColliderEntity, HashSet<IColliderEntity>>();
        //private HashSet<IColliderEntity> entities = new HashSet<IColliderEntity>();

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
        public void OnCollisionProfileChanged(IGameObject entity)
        {
            if (!(entity is IHasTrigger) && !(entity is IColliderEntity))
            {
                return;
            }
            if (!changedObjects.Contains(entity))
            {
                changedObjects.Add(entity);
            }
        }

        public void CheckCollisions(IColliderEntity thisEntity = null)
        {
            if (changedObjects.Count == 0 && toCheckAgainst.Count == 0)
            {
                return;
            }

            HandleChangedObjects();


            if (!thisEntity.CollisionsEnabled && thisEntity.GetTriggers().Count == 0)
            {
                return;
            }

            if (!toCheckAgainst.ContainsKey(thisEntity))
            {
                return;
            }

            foreach (IColliderEntity otherEntity in toCheckAgainst[thisEntity])
            {
                if (thisEntity.Equals(otherEntity))
                {
                    continue;
                }

                if (thisEntity.GetTriggers().Count > 0 && otherEntity.CanFireTriggers)
                {
                    CheckTriggers(thisEntity, otherEntity);
                }

                if (!otherEntity.CollisionsEnabled)
                {
                    continue;
                }

                if (thisEntity.GetCollisionComponent() != null && otherEntity.GetCollisionComponent() != null)
                {
                    CheckCollision(thisEntity, otherEntity, thisEntity.GetCollidesAgainst()[otherEntity.GetType()]);
                }
                
                    
                /*if (thisEntity.CheckGridCollisions)
                {
                    CheckGridCollisions(thisEntity);
                }*/
            }

            InactivateCollisionsAndTriggers(thisEntity);
        }

        private void HandleChangedObjects()
        {
            if (changedObjects.Count > 0)
            {
                if (toCheckAgainst.Count == 0)
                {
                    foreach (IColliderEntity changed in changedObjects)
                    {
                        foreach (IColliderEntity changed2 in changedObjects)
                        {
                            if (changed.Equals(changed2))
                            {
                                continue;
                            }
                            if (changed.GetCollidesAgainst().Count != 0 && changed.GetCollidesAgainst().ContainsKey(changed2.GetType()))
                            {
                                if (!toCheckAgainst.ContainsKey(changed))
                                {
                                    toCheckAgainst[changed] = new HashSet<IColliderEntity>();
                                }
                                if (!collisions.ContainsKey(changed))
                                {
                                    collisions[changed] = new Dictionary<IColliderEntity, bool>();
                                }
                                toCheckAgainst[changed].Add(changed2);
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
                    }
                
                        /*if (changed.GetCollisionComponent() == null && changed.GetTriggers().Count == 0)
                        {
                            entities.Remove(changed);
                            if (!changed.CanFireTriggers)
                            {
                                toCheckAgainst.Remove(changed);
                            }
                        }
                        else
                        {
                            if (changed.GetCollisionComponent() != null)
                            {
                                if (changed.GetCollidesAgainst().Count > 0)
                                {
                                    if (!collisions.ContainsKey(changed))
                                    {
                                        collisions[changed] = new Dictionary<IColliderEntity, bool>();
                                    }
                                    if (!entities.Contains(changed))
                                    {
                                        entities.Add(changed);
                                    }
                                }
                                else
                                {
                                    collisions.Remove(changed);
                                    entities.Remove(changed);
                                }

                                if (changed.GetTags().Count > 0)
                                {
                                    if (!toCheckAgainst.Contains(changed))
                                    {
                                        toCheckAgainst.Add(changed);
                                    }
                                }
                                else
                                {
                                    toCheckAgainst.Remove(changed);
                                }
                            }

                            /*if (changed.CheckGridCollisions)
                            {
                                gridCollisions[changed] = new Dictionary<StaticCollider, bool>();
                                if (!entities.Contains(changed))
                                {
                                    entities.Add(changed);
                                }
                            }*/

                        /*if (changed.GetTriggers().Count > 0)
                        {
                            if (!entities.Contains(changed))
                            {
                                entities.Add(changed);
                            }

                            triggers[changed] = new Dictionary<string, Dictionary<IGameObject, bool>>();
                            foreach (ITrigger trigger in changed.GetTriggers())
                            {
                                triggers[changed][trigger.GetTag()] = new Dictionary<IGameObject, bool>();
                            }
                        }

                        if (changed.CanFireTriggers && !toCheckAgainst.Contains(changed))
                        {
                            toCheckAgainst.Add(changed);
                        }
                    }*/
                    }
                    changedObjects.Clear();
            }
        }

        /*private void CheckGridCollisions(IColliderEntity thisEntity)
        {
            foreach ((StaticCollider, Direction) collision in GridCollisionChecker.Instance.HasGridCollisionAt(thisEntity, gridCollisionDirections))
            {
                if (!gridCollisions[thisEntity].ContainsKey(collision.Item1))
                {
                    thisEntity.CollisionStarted(collision.Item1, false);
                }
                gridCollisions[thisEntity][collision.Item1] = true;
            }
        }*/

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
                        //collisionsToRemove.Add((thisEntity, otherObject));
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
                            //triggersToRemove.Add((thisEntity, tag, otherEntity));
                            triggers[thisEntity][tag].Remove(otherEntity);
                        } else
                        {
                            triggers[thisEntity][tag][otherEntity] = false;
                        }
                    }
                }
            }

            /*foreach (IColliderEntity thisEntity in gridCollisions.Keys)
            {
                foreach (StaticCollider otherCollider in gridCollisions[thisEntity].Keys.ToList())
                {
                    if(!gridCollisions[thisEntity][otherCollider])
                    {
                        //gridCollisionsToRemove.Add((thisEntity, collider));
                        thisEntity.CollisionEnded(otherCollider);
                        gridCollisions[thisEntity].Remove(otherCollider);
                    } else
                    {
                        gridCollisions[thisEntity][otherCollider] = false;
                    }
                }
            }*/

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
