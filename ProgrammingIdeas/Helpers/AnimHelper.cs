using Android.Animation;
using Android.Views;
using Android.Views.Animations;

namespace ProgrammingIdeas.Animation
{
    public static class AnimHelper
    {
        public static void Animate(View v, string property, long duration, IInterpolator interpolator, params float[] positions)
        {
            var anim = ObjectAnimator.OfFloat(v, property, positions);
            anim.SetDuration(duration);
            anim.SetInterpolator(interpolator);
            anim.Start();
        }
    }
}