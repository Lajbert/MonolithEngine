﻿using Microsoft.Xna.Framework;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class AbstractDestroyable : PhysicalEntity
    {

        private readonly string DESTROY_AMINATION = "Destroy";

        protected float RotationRate = 0f;

        private bool hasDestroyAnimation = false;

        protected Random random;

        public AbstractDestroyable(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            random = new Random();
        }

        public virtual void Die()
        {
            if (!hasDestroyAnimation)
            {
                HorizontalFriction = .99f;
                VerticalFriction = .99f;
                int rand = random.Next(0, 11);
                Vector2 bump = new Vector2(0.1f, -0.1f);
                RotationRate = 0.1f;
                if (rand % 2 == 0)
                {
                    bump.X *= -1;
                    RotationRate *= -1;
                }
                CheckGridCollisions = false;
                RemoveCollisions();
                Transform.Velocity += bump;
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
