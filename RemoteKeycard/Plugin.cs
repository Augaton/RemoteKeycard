using System;
using Exiled.API.Features;
using AugatonLib.Runtime;
using RemoteKeycard.Handlers;

namespace RemoteKeycard
{
    public sealed class Plugin : Plugin<Config.Config>
    {
        public override string Name => "RemoteKeycard";

        public override string Author => "Zone-Shilari (base: Beryl)";

        public override string Prefix => "remotekeycard";

        public override Version Version => new Version(4, 0, 0);

        public override Version RequiredExiledVersion => new Version(9, 14, 2);

        public static Plugin Instance { get; private set; }

        private KeycardHandlers keycardHandlers;

        public override void OnEnabled()
        {
            Instance = this;

            keycardHandlers = new KeycardHandlers(Config);
            keycardHandlers.Start();

            PluginDirectory.Register(this, Capability.DoorLock);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            keycardHandlers?.Stop();

            PluginDirectory.Unregister(this);

            keycardHandlers = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
