using Exiled.Events.Features;
using RemoteKeycard.API.EventArgs;

namespace RemoteKeycard.API
{
    public static class Events
    {
        public static event CustomEventHandler<UsingKeycardEventArgs> UsingKeycard;

        public static void OnUsingKeycard(UsingKeycardEventArgs ev)
        {
            CustomEventHandler<UsingKeycardEventArgs> handler = UsingKeycard;

            if (handler is null)
                return;

            try
            {
                handler.Invoke(ev);
            }
            catch (System.Exception e)
            {
                Exiled.API.Features.Log.Error($"UsingKeycard: {e}");
            }
        }
    }
}
