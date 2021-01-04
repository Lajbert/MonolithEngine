using GameEngine2D.src.Entities.Animation;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GameEngine2D.Engine.src.Entities.Animations
{
    class AnimationStateMachine
    {
        private List<StateAnimation> animations;
        private StateAnimation currentAnimation = null;

        public AnimationStateMachine()
        {
            animations = new List<StateAnimation>();
        }

        public void RegisterAnimation(string state, AnimatedSpriteGroup animation, Func<bool> function = null, int priority = 0)
        {
            if (function == null)
            {
                function = () => (true);
            }
            StateAnimation anim = new StateAnimation(state, animation, function, priority);
            animations.Add(anim);
            animations.Sort((a, b) => a.priority.CompareTo(b.priority) * -1);
        }

        private class StateAnimation
        {

            public string state;
            public Func<bool> function;
            public AnimatedSpriteGroup animation;
            public int priority;

            public StateAnimation(string state, AnimatedSpriteGroup animation, Func<bool> function = null, int priority = 0)
            {
                this.state = state;
                this.animation = animation;
                this.priority = priority;
                this.function = function;
            }

        }

        public void Draw(GameTime gameTime)
        {
            if (animations.Count == 0)
            {
                return;
            }

            Play(gameTime);
        }

        private void Play(GameTime gameTime)
        {   
            StateAnimation nextAnimation = Pop();
            if (nextAnimation == null)
            {
                return;
            }
            if (nextAnimation != currentAnimation)
            {
                if (currentAnimation != null)
                {
                    currentAnimation.animation.Stop();
                }
                currentAnimation = nextAnimation;
                currentAnimation.animation.Init();
            }
            currentAnimation.animation.Play();
        }

        public void Update(GameTime gameTime)
        {
            if (animations.Count == 0 || currentAnimation == null)
            {
                return;
            }

            currentAnimation.animation.Update(gameTime);
        }

        private StateAnimation Pop()
        {
            foreach (StateAnimation anim in animations)
            {
                if (anim.function.Invoke())
                {
                    return anim;
                }
            }
            return null;
        }
    }
}
