using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Level.Collision;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Physics.Trigger;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine2D.Engine.Source.Physics
{
    public class CollisionEngine
    {

        private HashSet<IColliderEntity> toCheckAgainst = new HashSet<IColliderEntity>();
        private HashSet<IColliderEntity> entities = new HashSet<IColliderEntity>();

        private Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>> collisions = new Dictionary<IColliderEntity, Dictionary<IColliderEntity, bool>>();

        private Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>> triggers = new Dictionary<IHasTrigger, Dictionary<string, Dictionary<IGameObject, bool>>>();

        private Dictionary<IColliderEntity, Dictionary<StaticCollider, bool>> gridCollisions = new Dictionary<IColliderEntity, Dictionary<StaticCollider, bool>>();

        private HashSet<IGameObject> changedObjects = new HashSet<IGameObject>();

        private static readonly CollisionEngine instance = new CollisionEngine();

        private List<Direction> gridCollisionDirections = new List<Direction>() { Direction.SOUTH, Direction.NORTH, Direction.EAST, Direction.WEST };

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
                    
                    if (thisEntity.CheckGridCollisions)
                    {
                        CheckGridCollisions(thisEntity);
                    }
                }
            }

            InactivateCollisionsAndTriggers();

            if (changedObjects.Count > 0)
            {
                foreach (IColliderEntity changed in changedObjects)
                {
                    if (changed.GetCollisionComponent() == null && changed.GetTriggers().Count == 0 && !changed.CheckGridCollisions)
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

                        if (changed.CheckGridCollisions)
                        {
                            gridCollisions[changed] = new Dictionary<StaticCollider, bool>();
                            if (!entities.Contains(changed)) {
                                entities.Add(changed);
                            }
                        }

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

        private void CheckGridCollisions(IColliderEntity thisEntity)
        {
            foreach ((StaticCollider, Direction) collision in GridCollisionChecker.Instance.HasGridCollisionAt(thisEntity, gridCollisionDirections))
            {
                if (!gridCollisions[thisEntity].ContainsKey(collision.Item1))
                {
                    thisEntity.OnCollisionStart(collision.Item1);
                }
                gridCollisions[thisEntity][collision.Item1] = true;
            }
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

        private void InactivateCollisionsAndTriggers()
        {

            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                foreach (IColliderEntity otherObject in collisions[thisEntity].Keys.ToList())
                {
                    if (thisEntity.Equals(otherObject))
                    {
                        continue;
                    }
                    if(!collisions[thisEntity][otherObject])
                    {
                        thisEntity.OnCollisionEnd(otherObject);
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

            foreach (IColliderEntity thisEntity in gridCollisions.Keys)
            {
                foreach (StaticCollider otherCollider in gridCollisions[thisEntity].Keys.ToList())
                {
                    if(!gridCollisions[thisEntity][otherCollider])
                    {
                        //gridCollisionsToRemove.Add((thisEntity, collider));
                        thisEntity.OnCollisionEnd(otherCollider);
                        gridCollisions[thisEntity].Remove(otherCollider);
                    } else
                    {
                        gridCollisions[thisEntity][otherCollider] = false;
                    }
                }
            }

        }
    }
}
