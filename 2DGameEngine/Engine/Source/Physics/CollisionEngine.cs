using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
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

        private HashSet<IColliderEntity> changedObjects = new HashSet<IColliderEntity>();

        private static readonly CollisionEngine instance = new CollisionEngine();

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

        public void OnCollisionProfileChanged(IColliderEntity entity)
        {
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

            PrepareCollisions();

            foreach (IColliderEntity thisEntity in entities)
            {
                if (!thisEntity.CollisionsEnabled)
                {
                    continue;
                }
                foreach (IColliderEntity otherObject in toCheckAgainst)
                {
                    if (thisEntity.Equals(otherObject))
                    {
                        continue;
                    }

                    if (otherObject.GetTags().Count == 0 || !otherObject.CollisionsEnabled)
                    {
                        continue;
                    }

                    bool collidesWith = false;
                    foreach(string tag in otherObject.GetTags()) {
                        if (thisEntity.GetCollidesAgainst().Contains(tag)) {
                            collidesWith = true;
                            break;
                        }
                    }

                    if (!collidesWith)
                    {
                        continue;
                    }

                    if (thisEntity.GetCollisionComponent() != null && otherObject.GetCollisionComponent() != null)
                    {
                        CheckCollision(thisEntity, otherObject);
                    }
                    
                }
            }

            InactivateCollisions();

            if (changedObjects.Count > 0)
            {
                foreach (IColliderEntity changed in changedObjects)
                {
                    if (changed.GetCollisionComponent() == null)
                    {
                        entities.Remove(changed);
                        toCheckAgainst.Remove(changed);
                    } else
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
                        } else
                        {
                            toCheckAgainst.Remove(changed);
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

        private void PrepareCollisions()
        {
            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                foreach (IColliderEntity otherObject in collisions[thisEntity].Keys.ToList())
                {
                    collisions[thisEntity][otherObject] = false;
                }
            }
        }

        private void InactivateCollisions()
        {
            List<(IColliderEntity, IColliderEntity)> toRemove = new List<(IColliderEntity, IColliderEntity)>();

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
                        toRemove.Add((thisEntity, otherObject));
                    }
                }
            }
            foreach ((IColliderEntity, IColliderEntity) t in toRemove)
            {
                collisions[t.Item1].Remove(t.Item2);
            }
        }
    }
}
