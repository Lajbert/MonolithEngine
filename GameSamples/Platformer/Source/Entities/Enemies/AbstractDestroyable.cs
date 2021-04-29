using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Source.Entities.Animation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies
{
    class AbstractDestroyable : PhysicalEntity
    {

        private readonly string DESTROY_AMINATION = "Destroy";

        protected float RotationRate = 0f;

        private bool hasDestroyAnimation = false;

        public AbstractDestroyable(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {

        }

        public virtual void Die()
        {
            if (!hasDestroyAnimation)
            {
                HorizontalFriction = .99f;
                VerticalFriction = .99f;
                int rand = MyRandom.Between(0, 10);
                Vector2 bump = new Vector2(0.1f, -0.1f);
                RotationRate = 0.1f;
                if (rand % 2 == 0)
                {
                    bump.X *= -1;
                    RotationRate *= -1;
                }
                CheckGridCollisions = false;
                RemoveCollisions();
                Velocity += bump;
                Timer.TriggerAfter(3000, Destroy);
            }
            else
            {
                if (GetComponent<AnimationStateMachine>() != null && GetComponent<AnimationStateMachine>().HasAnimation(DESTROY_AMINATION + CurrentFaceDirection))
                {
                    GetComponent<AnimationStateMachine>().PlayAnimation(DESTROY_AMINATION + CurrentFaceDirection);
                }
            }
        }

        public void SetDestroyAnimation(AbstractAnimation destroyAnimation, Direction direction = Direction.CENTER)
        {
            hasDestroyAnimation = true;
            if (GetComponent<AnimationStateMachine>() == null)
            {
                AddComponent(new AnimationStateMachine());
            }
            destroyAnimation.StartedCallback += () => RemoveCollisions();
            destroyAnimation.StoppedCallback += Destroy;
            destroyAnimation.Looping = false;
            GetComponent<AnimationStateMachine>().RegisterAnimation(DESTROY_AMINATION + direction.ToString(), destroyAnimation, () => false);
        }
    }
}
