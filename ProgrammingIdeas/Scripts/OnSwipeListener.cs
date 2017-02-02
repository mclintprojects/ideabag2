using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Views.GestureDetector;

namespace ProgrammingIdeas.Scripts
{
    enum Swipe
    {
        Left,
        Right
    }

    class OnSwipeListener : Java.Lang.Object, View.IOnTouchListener
    {
        private static GestureDetector detector;
        private event EventHandler OnSwipeLeft;
        private event EventHandler OnSwipeRight;
        public OnSwipeListener(Context ctx)
        {
            detector = new GestureDetector(ctx, new GestureListener(SwipeRecieved));
        }

        void SwipeRecieved(Swipe swipe)
        {
            switch(swipe)
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
            private static int SWIPE_THRESHOLD = 100;
            private static int SWIPE_VELOCITY_THRESHOLD = 100;
            Action<Swipe> listener;
            public GestureListener(Action<Swipe> listener)
            {
                this.listener = listener;
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
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                    {
                        if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                        {
                            if (diffX > 0)
                                SwipeRight();
                            else
                                SwipeLeft();
                        }
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
                listener(Swipe.Left);
            }

            private void SwipeRight()
            {
                listener(Swipe.Right);
            }
        }
    }
}