using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Interfaces;
using MonolithEngine.Engine.Source.Level.Collision;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Interface;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using MonolithEngine.Entities;

namespace MonolithEngine.Engine.Source.Physics
{
    public class CollisionEngine
    {

        private HashSet<IColliderEntity> toCheckAgainst = new HashSet<IColliderEntity>();
        private HashSet<IColliderEntity> entities = new HashSet<IColliderEntity>();

        private Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>> collisions = new Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>>();

        private Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>> triggers = new Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>>();

        //private Dictionary<IColliderEntity, Dictionary<StaticCollider, bool>> gridCollisions = new Dictionary<IColliderEntity, Dictionary<StaticCollider, bool>>();

        private HashSet<IGameObject> changedObjects = new HashSet<IGameObject>();

        public CollisionEngine()
        {

        }

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

        public void Update(IColliderEntity thisEntity = null)
        {
            if (changedObjects.Count == 0 && (entities.Count == 0 || toCheckAgainst.Count == 0 ))
            {
                return;
            }

            HandleChangedObjects();


            if (!thisEntity.CollisionsEnabled && thisEntity.GetTriggers().Count == 0)
            {
                return;
            }

            foreach (IColliderEntity otherEntity in toCheckAgainst)
            {
                if (thisEntity.Equals(otherEntity))
                {
                    continue;
                }

                if (thisEntity.GetTriggers().Count > 0 && otherEntity.CanFireTriggers)
                {
                    CheckTriggers(thisEntity, otherEntity);
                }

                if (otherEntity.GetTags().Count == 0 || !otherEntity.CollisionsEnabled)
                {
                    continue;
                }

                bool possibleCollision = false;
                bool allowOverlap = false;
                foreach(string tag in otherEntity.GetTags()) {
                    if (thisEntity.GetCollidesAgainst().ContainsKey(tag)) {
                        possibleCollision = true;
                        allowOverlap = thisEntity.GetCollidesAgainst()[tag];
                        break;
                    }
                }

                if (!possibleCollision)
                {
                    continue;
                }

                if (thisEntity.GetCollisionComponent() != null && otherEntity.GetCollisionComponent() != null)
                {
                    CheckCollision(thisEntity, otherEntity, allowOverlap);
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
                foreach (IColliderEntity changed in changedObjects)
                {
                    if (changed.GetCollisionComponent() == null && changed.GetTriggers().Count == 0)
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
                                    Logger.Warn("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                    Logger.Warn("ONLY ADD THIS IF THERE IS ANYTHING COLLIDING WITH IT, USE CENTRAL COLLISION REGISTRATION");
                                    Logger.Warn("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
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

                        if (changed.GetTriggers().Count > 0)
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
                    }
                }
            }

            changedObjects.Clear();
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
                    if (thisEntity.Equals(otherObject) || !otherObject.CollisionsEnabled)
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

        public void Destroy()
        {
            HandleChangedObjects();
            Logger.Info("Collision engine data:");
            Logger.Info("Entities: " + string.Join(", ", entities));
            Logger.Info("To check against: " + string.Join(", ", toCheckAgainst));
        }
    }
}
