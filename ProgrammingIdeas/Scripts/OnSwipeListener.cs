using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using static Android.Views.GestureDetector;

namespace ProgrammingIdeas.Helpers
{
    internal enum Swipe
    {
        Left,
        Right
    }

    public class OnSwipeListener : Java.Lang.Object, View.IOnTouchListener
    {
        private static GestureDetector detector;

        public event EventHandler OnSwipeLeft;

        public event EventHandler OnSwipeRight;

        public OnSwipeListener(Context ctx)
        {
            detector = new GestureDetector(ctx, new GestureListener(SwipeRecieved));
        }

        private void SwipeRecieved(Swipe swipe)
        {
            switch (swipe)
            {
                case Swipe.Left:
                    OnSwipeLeft(this, new EventArgs());
                    break;

                case Swipe.Right:
                    OnSwipeRight(this, new EventArgs());
                    break;
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return detector.OnTouchEvent(e);
        }

        private class GestureListener : SimpleOnGestureListener
        {
            private static int SWIPE_THRESHOLD = 150;
            private static int SWIPE_VELOCITY_THRESHOLD = 80;
            private Action<Swipe> baseListener;

            public GestureListener(Action<Swipe> listener)
            {
                baseListener = listener;
            }

            public override bool OnDown(MotionEvent e)
            {
                return true;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                bool result = false;
                try
                {
                    // Get vertical swipe distance.
                    float diffY = e2.GetY() - e1.GetY();

                    // Get horizontal swipe distance.
                    float diffX = e2.GetX() - e1.GetX();

                    // calculating swipe for x-axis.
                    if (Math.Abs(diffX) > Math.Abs(diffY) && (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD))
                    {
                        if (diffX > 0)
                            SwipeRight();
                        else
                            SwipeLeft();
                        result = true;
                    }
                }
                catch
                {
                    Toast.MakeText(Application.Context, "Something went wrong.", ToastLength.Short).Show();
                }
                return result;
            }

            private void SwipeLeft()
            {
                baseListener(Swipe.Left);
            }

            private void SwipeRight()
            {
                baseListener(Swipe.Right);
            }
        }
    }
}