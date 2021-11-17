using MonolithEngine;
using Microsoft.Xna.Framework;

namespace ForestPlatformerExample
{
    class Spring : AbstractInteractive
    {

        public int Power;
        public Spring(AbstractScene scene, Vector2 position, int power) : base(scene, position)
        {

            Active = true;

            Power = power;

            //CollisionComponent = new CircleCollisionComponent(this, 10, new Vector2(2, 10));
            AddComponent(new BoxCollisionComponent(this, 16, 9, new Vector2(0, 7)));
            //(GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            //DEBUG_SHOW_PIVOT = true;


            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(8, 8);

            SpriteSheetAnimation springAnim = new SpriteSheetAnimation(this, Assets.GetAnimationTexture("SpringAnim"), 24)
            {
                Looping = false
            };
            Animations.RegisterAnimation("Bounce", springAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);
        }

        public void Bounce()
        {
            GetComponent<AnimationStateMachine>().PlayAnimation("Bounce");
            AudioEngine.Play("SpringBounceSound");
        }
    }
}
