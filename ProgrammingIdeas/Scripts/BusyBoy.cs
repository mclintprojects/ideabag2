using System;

namespace ProgrammingIdeas.Helpers
{
    public class BusyBoy : IDisposable
    {
        private Action after;

        public BusyBoy(Action after)
        {
            this.after = after;
        }

        public void Dispose()
        {
            after();
        }
    }

    public static class BusyHandler
    {
        public static BusyBoy Handle(Action after)
        {
            return new BusyBoy(after);
        }
    }
}