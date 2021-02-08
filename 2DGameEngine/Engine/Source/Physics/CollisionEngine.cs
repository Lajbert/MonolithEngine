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

        private HashSet<IPhysicsEntity> toCheckAgainst = new HashSet<IPhysicsEntity>();
        private HashSet<IPhysicsEntity> entities = new HashSet<IPhysicsEntity>();

        private Dictionary<IPhysicsEntity, Dictionary<CollisionType, Dictionary<IPhysicsEntity, bool>>> collisions = new Dictionary<IPhysicsEntity, Dictionary<CollisionType, Dictionary<IPhysicsEntity, bool>>>();

        private HashSet<IPhysicsEntity> changedObjects = new HashSet<IPhysicsEntity>();

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

        public void OnCollisionProfileChanged(IPhysicsEntity entity)
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

            foreach (IPhysicsEntity thisEntity in entities)
            {
                foreach (IPhysicsEntity otherObject in toCheckAgainst)
                {
                    if (otherObject.GetTags().Count == 0)
                    {
                        continue;
                    }

                    bool collidesWith = false;
                    if (otherObject.GetTags().Count > 0)
                    {
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
                    }
                    if (thisEntity.Equals(otherObject))
                    {
                        continue;
                    }

                    //UpdateRaycast(thisEntity, otherObject);
                    //UpdateGridCollisions(thisEntity, otherObject);
                    if (thisEntity.GetCircleCollisionComponent() != null && otherObject.GetCircleCollisionComponent() != null 
                        && otherObject.GetCircleCollisionComponent().IsCircleCollider)
                    {
                        UpdateCircleCollisions(thisEntity, otherObject);
                    }
                    
                    //UpdateBoxCollisions(thisEntity, otherObject);
                    //UpdatePointCollisions(thisEntity, otherObject);
                }
            }

            InactivateCollisions();

            if (changedObjects.Count > 0)
            {
                foreach (IPhysicsEntity changed in changedObjects)
                {
                    if (changed.GetCollisionProfile().Count == 0)
                    {
                        entities.Remove(changed);
                        toCheckAgainst.Remove(changed);
                    } else
                    {
                        foreach (CollisionType collType in changed.GetCollisionProfile())
                        {
                            if (!collisions.ContainsKey(changed))
                            {
                                collisions[changed] = new Dictionary<CollisionType, Dictionary<IPhysicsEntity, bool>>();
                            }
                            if (!collisions[changed].ContainsKey(collType))
                            {
                                collisions[changed][collType] = new Dictionary<IPhysicsEntity, bool>();
                            }
                        }
                        foreach (CollisionType collType in collisions[changed].Keys.ToList())
                        {
                            if (!changed.GetCollisionProfile().Contains(collType))
                            {
                                collisions[changed].Remove(collType);
                            }
                        }
                        if (!entities.Contains(changed))
                        {
                            entities.Add(changed);
                        }
                        if (!toCheckAgainst.Contains(changed))
                        {
                            Logger.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            Logger.Log("ONLY ADD THIS IF THERE IS ANYTHING COLLIDING WITH IT, USE CENTRAL COLLISION REGISTRATION");
                            Logger.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            toCheckAgainst.Add(changed);
                        }
                        /*if (!entities.Contains(changed))
                        {
                            entities.Add(changed);
                        }
                        if (!toCheckAgainst.Contains(changed))
                        {
                            toCheckAgainst.Add(changed);
                        }*/
                    }
                    /*if (changed.GetCollisionProfile().Contains(CollisionType.CIRCLE))
                    {
                        toCheckAgainst.Add(changed);
                    }*/
                }
            }
            changedObjects.Clear();
        }

        private void UpdateGridCollisions(IPhysicsEntity thisEntity, IPhysicsEntity otherObject)
        {

        }

        private void UpdateCircleCollisions(IPhysicsEntity thisEntity, IPhysicsEntity otherObject)
        {
            throw new Exception("Implement fast check to see if circle collision should even be checked!");
            if (thisEntity.GetCircleCollisionComponent().Overlaps(otherObject))
            {
                if (!collisions.ContainsKey(thisEntity) || !collisions[thisEntity].ContainsKey(CollisionType.CIRCLE) 
                    || !collisions[thisEntity][CollisionType.CIRCLE].ContainsKey(otherObject))
                {
                    collisions[thisEntity][CollisionType.CIRCLE] = new Dictionary<IPhysicsEntity, bool>();
                    thisEntity.OnCollisionStart(otherObject);
                }
                collisions[thisEntity][CollisionType.CIRCLE][otherObject] = true;
            }
        }

        private void UpdateBoxCollisions(IPhysicsEntity thisEntity, IPhysicsEntity otherObject)
        {

        }

        private void UpdatePointCollisions(IPhysicsEntity thisEntity, IPhysicsEntity otherObject)
        {

        }

        private void UpdateRaycast(IPhysicsEntity thisEntity, IPhysicsEntity otherObject)
        {

        }

        private void PrepareCollisions()
        {
            foreach (IPhysicsEntity thisEntity in collisions.Keys)
            {
                foreach (CollisionType type in collisions[thisEntity].Keys)
                {
                    foreach (IPhysicsEntity otherObject in collisions[thisEntity][type].Keys.ToList())
                    {
                        collisions[thisEntity][type][otherObject] = false;
                    }
                }
            }
        }

        private void InactivateCollisions()
        {
            List<(IPhysicsEntity, CollisionType, IPhysicsEntity)> toRemove = new List<(IPhysicsEntity, CollisionType, IPhysicsEntity)>();

            foreach (IPhysicsEntity thisEntity in collisions.Keys)
            {
                foreach (CollisionType type in collisions[thisEntity].Keys)
                {
                    foreach (IPhysicsEntity otherObject in collisions[thisEntity][type].Keys.ToList())
                    {
                        if (thisEntity.Equals(otherObject))
                        {
                            continue;
                        }
                        if(!collisions[thisEntity][type][otherObject])
                        {
                            thisEntity.OnCollisionEnd(otherObject);
                            toRemove.Add((thisEntity, type, otherObject));
                        }
                    }
                }
            }
            foreach ((IPhysicsEntity, CollisionType, IPhysicsEntity) t in toRemove)
            {
                collisions[t.Item1][t.Item2].Remove(t.Item3);
                /*if (collisions[t.Item1][t.Item2].Count() == 0)
                {
                    collisions[t.Item1].Remove(t.Item2);
                }
                if (collisions[t.Item1].Count() == 0)
                {
                    collisions.Remove(t.Item1);
                }*/
            }
        }
    }
}
