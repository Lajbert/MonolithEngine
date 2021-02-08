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

        private Dictionary<IColliderEntity, Dictionary<CollisionType, Dictionary<IColliderEntity, bool>>> collisions = new Dictionary<IColliderEntity, Dictionary<CollisionType, Dictionary<IColliderEntity, bool>>>();

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
                foreach (IColliderEntity otherObject in toCheckAgainst)
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
                foreach (IColliderEntity changed in changedObjects)
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
                                collisions[changed] = new Dictionary<CollisionType, Dictionary<IColliderEntity, bool>>();
                            }
                            if (!collisions[changed].ContainsKey(collType))
                            {
                                collisions[changed][collType] = new Dictionary<IColliderEntity, bool>();
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

        private void UpdateGridCollisions(IColliderEntity thisEntity, IColliderEntity otherObject)
        {

        }

        private void UpdateCircleCollisions(IColliderEntity thisEntity, IColliderEntity otherObject)
        {
            if (thisEntity.GetCircleCollisionComponent().Overlaps(otherObject))
            {
                if (!collisions.ContainsKey(thisEntity) || !collisions[thisEntity].ContainsKey(CollisionType.CIRCLE) 
                    || !collisions[thisEntity][CollisionType.CIRCLE].ContainsKey(otherObject))
                {
                    thisEntity.OnCollisionStart(otherObject);
                }
                collisions[thisEntity][CollisionType.CIRCLE][otherObject] = true;
            }
        }

        public List<IColliderEntity> GetCollidesWith(IColliderEntity collider)
        {
            List<IColliderEntity> colliders = new List<IColliderEntity>();

            if (collisions.ContainsKey(collider))
            {
                foreach (CollisionType collType in collisions[collider].Keys)
                {
                    colliders.AddRange(collisions[collider][collType].Keys);
                }
            }

            return colliders;
        }

        private void UpdateBoxCollisions(IColliderEntity thisEntity, IColliderEntity otherObject)
        {

        }

        private void UpdatePointCollisions(IColliderEntity thisEntity, IColliderEntity otherObject)
        {

        }

        private void UpdateRaycast(IColliderEntity thisEntity, IColliderEntity otherObject)
        {

        }

        private void PrepareCollisions()
        {
            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                foreach (CollisionType type in collisions[thisEntity].Keys)
                {
                    foreach (IColliderEntity otherObject in collisions[thisEntity][type].Keys.ToList())
                    {
                        collisions[thisEntity][type][otherObject] = false;
                    }
                }
            }
        }

        private void InactivateCollisions()
        {
            List<(IColliderEntity, CollisionType, IColliderEntity)> toRemove = new List<(IColliderEntity, CollisionType, IColliderEntity)>();

            foreach (IColliderEntity thisEntity in collisions.Keys)
            {
                foreach (CollisionType type in collisions[thisEntity].Keys)
                {
                    foreach (IColliderEntity otherObject in collisions[thisEntity][type].Keys.ToList())
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
            foreach ((IColliderEntity, CollisionType, IColliderEntity) t in toRemove)
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
