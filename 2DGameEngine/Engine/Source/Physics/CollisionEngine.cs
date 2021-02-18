using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Physics.Trigger;
using GameEngine2D.Global;
using GameEngine2D.Source.GridCollision;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics
{
    public class CollisionEngine
    {

        private HashSet<IColliderEntity> toCheckAgainst = new HashSet<IColliderEntity>();
        private HashSet<IColliderEntity> entities = new HashSet<IColliderEntity>();

        private Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>> collisions = new Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>>();

        private Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>> triggers = new Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>>();

        private HashSet<IGameObject> changedObjects = new HashSet<IGameObject>();

        private static readonly CollisionEngine instance = new CollisionEngine();

        private List<(IColliderEntity, IColliderEntity)> collisionsToRemove = new List<(IColliderEntity, IColliderEntity)>();
        private List<(IHasTrigger, string, IHasTrigger)> triggersToRemove = new List<(IHasTrigger, string, IHasTrigger)>();

        private CollisionEngine()
        {

        }

        static CollisionEngine()
        {
        }

        public static CollisionEngine Instance
        {
            get
            {
                return instance;
            }
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

        public void Update(GameTime gameTime)
        {
            if (changedObjects.Count == 0 && (entities.Count == 0 || toCheckAgainst.Count == 0 ))
            {
                return;
            }

            PrepareCollisionsAndTriggers();

            foreach (IColliderEntity thisEntity in entities)
            {

                if (!thisEntity.CollisionsEnabled && thisEntity.GetTriggers().Count == 0)
                {
                    continue;
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

                    bool collidesWith = false;
                    foreach(string tag in otherEntity.GetTags()) {
                        if (thisEntity.GetCollidesAgainst().Contains(tag)) {
                            collidesWith = true;
                            break;
                        }
                    }

                    if (!collidesWith)
                    {
                        continue;
                    }

                    if (thisEntity.GetCollisionComponent() != null && otherEntity.GetCollisionComponent() != null)
                    {
                        CheckCollision(thisEntity, otherEntity);
                    }
                    
                }
            }

            InactivateCollisionsAndTriggers();

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
                    } else
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

                        if (changed.GetTriggers().Count > 0 && !entities.Contains(changed))
                        {
                            entities.Add(changed);
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

        private void CheckCollision(IColliderEntity thisEntity, IColliderEntity otherObject)
        {
            if (thisEntity.GetCollisionComponent().Overlaps(otherObject))
            {
                if (!collisions.ContainsKey(thisEntity) || !collisions[thisEntity].ContainsKey(otherObject))
                {
                    thisEntity.OnCollisionStart(otherObject);
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

        private void PrepareCollisionsAndTriggers()
        {
            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                foreach (IColliderEntity otherObject in collisions[thisEntity].Keys.ToList())
                {
                    collisions[thisEntity][otherObject] = false;
                }
            }

            foreach (IHasTrigger thisEntity in triggers.Keys)
            {
                foreach (string tag in triggers[thisEntity].Keys)
                {
                    foreach(IHasTrigger otherEntity in triggers[thisEntity][tag].Keys.ToList())
                    {
                        triggers[thisEntity][tag][otherEntity] = false;
                    }
                }
            }
        }

        private void InactivateCollisionsAndTriggers()
        {
            collisionsToRemove.Clear();
            triggersToRemove.Clear();

            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                foreach (IColliderEntity otherObject in collisions[thisEntity].Keys)
                {
                    if (thisEntity.Equals(otherObject))
                    {
                        continue;
                    }
                    if(!collisions[thisEntity][otherObject])
                    {
                        thisEntity.OnCollisionEnd(otherObject);
                        collisionsToRemove.Add((thisEntity, otherObject));
                    }
                }
            }
            foreach ((IColliderEntity, IColliderEntity) t in collisionsToRemove)
            {
                collisions[t.Item1].Remove(t.Item2);
            }

            foreach (IHasTrigger thisEntity in triggers.Keys)
            {
                foreach (string tag in triggers[thisEntity].Keys)
                {
                    foreach (IHasTrigger otherEntity in triggers[thisEntity][tag].Keys.ToList())
                    {
                        if(!triggers[thisEntity][tag][otherEntity]) {
                            thisEntity.OnLeaveTrigger(tag, otherEntity);
                            triggersToRemove.Add((thisEntity, tag, otherEntity));
                        }
                    }
                }
            }

            foreach ((IHasTrigger, string, IHasTrigger) toRemove in triggersToRemove)
            {
                triggers[toRemove.Item1][toRemove.Item2].Remove(toRemove.Item3);
            }
        }
    }
}
