using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{

    /// <summary>
    /// A state machine to play animations automatically based on the state of entity it's assigned to.
    /// </summary>
    public class AnimationStateMachine : IComponent, IUpdatableComponent, IDrawableComponent
    {
        private List<StateAnimation> animations;

        // Frame transition between animations: having a transition defined means
        // that the new animation will start on the same frame the previous one ended.
        private HashSet<(string, string)> transitions = new HashSet<(string, string)>();

        private StateAnimation currentAnimation = null;

        private Vector2 offset = Vector2.Zero;

        // animation to play regardless of the object's current state
        private StateAnimation animationOverride = null;

        private int? transitionFrame = null;

        public Vector2 Offset {
            get => offset;
            set {
                offset = value;
                foreach (StateAnimation anim in animations) {
                    anim.animation.Offset = offset;
                }
            } }

        public bool UniquePerEntity { get; set; }

        public AnimationStateMachine()
        {
            animations = new List<StateAnimation>();
            UniquePerEntity = true;
        }

        /// <summary>
        /// Adds an animation to the state machine.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="animation">The animation itself.</param>
        /// <param name="playCondition">The condition to play the given animation</param>
        /// <param name="priority">Priority over other animations with when we could play multiple based on their condition.</param>
        public void RegisterAnimation(string stateName, AbstractAnimation animation, Func<bool> playCondition = null, int priority = 0)
        {
            if (playCondition == null)
            {
                playCondition = () => (true);
            }
            if (animation.Offset == Vector2.Zero)
            {
                animation.Offset = Offset;
            }
            StateAnimation anim = new StateAnimation(stateName, animation, playCondition, priority);
            animations.Add(anim);
            animations.Sort((a, b) => a.priority.CompareTo(b.priority) * -1);
        }

        public void PlayAnimation(string stateName)
        {
            foreach (StateAnimation anim in animations)
            {
                if (anim.state.Equals(stateName))
                {
                    animationOverride = anim;
                    animationOverride.animation.Init();
                    return;
                }
            }
            throw new Exception("Requested animation not found");
        }

        /// <summary>
        /// Internal helper class to store animation states.
        /// </summary>
        private class StateAnimation
        {

            public string state;
            public Func<bool> function;
            public AbstractAnimation animation;
            public int priority;
            public bool played;

            public StateAnimation(string state, AbstractAnimation animation, Func<bool> function = null, int priority = 0)
            {
                this.state = state;
                this.animation = animation;
                this.priority = priority;
                this.function = function;
                played = false;
            }

            public override bool Equals(object obj)
            {
                return obj is StateAnimation animation &&
                       state == animation.state &&
                       EqualityComparer<Func<bool>>.Default.Equals(function, animation.function) &&
                       EqualityComparer<AbstractAnimation>.Default.Equals(this.animation, animation.animation) &&
                       priority == animation.priority;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(state, function, animation, priority);
            }
        }

        public bool HasAnimation(string state)
        {
            foreach (StateAnimation anim in animations)
            {
                if (anim.state.Equals(state))
                {
                    return true;
                }
            }
            return false;
        }

        public AbstractAnimation GetAnimation(string state)
        {
            foreach (StateAnimation anim in animations)
            {
                if (anim.state.Equals(state))
                {
                    return anim.animation;
                }
            }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (animations.Count == 0)
            {
                return;
            }

            Play(spriteBatch);
        }

        /// <summary>
        /// Plays the current animations and calls callbacks (if any).
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void Play(SpriteBatch spriteBatch)
        {
            transitionFrame = null;
            if (animationOverride != null && animationOverride.animation.Finished())
            {
                animationOverride = null;
            }
            StateAnimation nextAnimation = Pop();
            if (nextAnimation == null)
            {
                return;
            }
            if (nextAnimation != currentAnimation)
            {
                if (currentAnimation != null)
                {
                    if (transitions.Contains((currentAnimation.state, nextAnimation.state))) {
                        transitionFrame = currentAnimation.animation.GetCurrentFrame();
                    }
                    currentAnimation.animation.Stop();
                    currentAnimation.animation.InvokeStoppedCallback();
                    currentAnimation.animation.AnimationSwitchCallback?.Invoke();
                }
                currentAnimation = nextAnimation;
                currentAnimation.animation.Init(transitionFrame);
                currentAnimation?.animation.InvokeStartedCallback();
            }
            currentAnimation.animation.Play(spriteBatch);
        }

        /// <summary>
        /// Registers a frame transition between 2 animations.
        /// </summary>
        /// <param name="anim1"></param>
        /// <param name="anim2"></param>
        public void AddFrameTransition(string anim1, string anim2)
        {
            transitions.Add((anim1, anim2));
            transitions.Add((anim2, anim1));
        }

        /// <summary>
        /// Updates the current animation.
        /// </summary>
        public void Update()
        {
            if (animations.Count == 0 || currentAnimation == null)
            {
                return;
            }

            currentAnimation.animation.Update();
        }

        /// <summary>
        /// Returns the next animation to play or the animation
        /// override.
        /// </summary>
        /// <returns></returns>
        private StateAnimation Pop()
        {
            if (animationOverride != null)
            {
                return animationOverride;
            }
            foreach (StateAnimation anim in animations)
            {
                if (anim.function.Invoke())
                {
                    return anim;
                }
            }
            return null;
        }

        public void Destroy()
        {
            foreach (StateAnimation anim in animations)
            {
                anim.animation.Destroy();
            }
        }

        public string GetCurrentAnimationState()
        {
            if (currentAnimation == null)
            {
                return "NULL";
            }
            return currentAnimation.state;
        }

        public void PreUpdate()
        {
        }

        public void PostUpdate()
        {
        }

        public Type GetComponentType()
        {
            return GetType();
        }
    }
}
