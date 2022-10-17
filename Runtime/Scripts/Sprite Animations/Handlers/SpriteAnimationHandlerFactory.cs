using System;
using System.Collections.Generic;

namespace H2DT.SpriteAnimations.Handlers
{
    public class SpriteAnimationHandlerFactory
    {
        private Dictionary<Type, SpriteAnimationHandler> _handlers = new Dictionary<Type, SpriteAnimationHandler>();
        private SpriteAnimator _animator;

        public SpriteAnimationHandlerFactory(SpriteAnimator animator)
        {
            _animator = animator;
        }

        public SpriteAnimationHandler GetHandler(SpriteAnimation animation)
        {
            _handlers.TryGetValue(animation.GetType(), out SpriteAnimationHandler handler);

            if (handler == null)
            {
                handler = FabricateHandler(animation);
            }

            return handler;
        }

        protected SpriteAnimationHandler FabricateHandler(SpriteAnimation animation)
        {
            SpriteAnimationHandler handler = Activator.CreateInstance(animation.handlerType) as SpriteAnimationHandler;
            handler.SetAnimator(_animator);

            _handlers.Add(animation.GetType(), handler);

            return handler;
        }
    }
}
