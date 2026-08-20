using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RemoteKeycard.API;
using RemoteKeycard.API.Enums;
using RemoteKeycard.API.EventArgs;
using RemoteKeycard.API.Extensions;
using PermissionFlags = Exiled.API.Enums.KeycardPermissions;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace RemoteKeycard.Handlers
{
    public sealed class KeycardHandlers
    {
        private readonly Config.Config config;

        public KeycardHandlers(Config.Config config) => this.config = config;

        public void Start()
        {
            PlayerEvents.InteractingDoor += OnInteractingDoor;
            PlayerEvents.UnlockingGenerator += OnUnlockingGenerator;
            PlayerEvents.InteractingLocker += OnInteractingLocker;
            PlayerEvents.ActivatingWarheadPanel += OnActivatingWarheadPanel;
        }

        public void Stop()
        {
            PlayerEvents.InteractingDoor -= OnInteractingDoor;
            PlayerEvents.UnlockingGenerator -= OnUnlockingGenerator;
            PlayerEvents.InteractingLocker -= OnInteractingLocker;
            PlayerEvents.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            try
            {
                if (!config.AffectDoors || ev?.Player is null || ev.Door is null || ev.IsAllowed)
                    return;

                if (!ev.Player.HasKeycardPermission(ev.Door.KeycardPermissions))
                    return;

                if (!Approve(ev.Player, RemoteTarget.Door, ev))
                    return;

                ev.IsAllowed = true;
                Trace(ev.Player, "porte", ev.Door.Type.ToString());
            }
            catch (Exception e)
            {
                Log.Error($"OnInteractingDoor: {e}");
            }
        }

        private void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            try
            {
                if (!config.AffectGenerators || ev?.Player is null || ev.Generator is null || ev.IsAllowed)
                    return;

                if (!ev.Player.HasKeycardPermission(ev.Generator.KeycardPermissions))
                    return;

                if (!Approve(ev.Player, RemoteTarget.Generator, ev))
                    return;

                ev.IsAllowed = true;
                Trace(ev.Player, "generateur", ev.Generator.Room?.Type.ToString());
            }
            catch (Exception e)
            {
                Log.Error($"OnUnlockingGenerator: {e}");
            }
        }

        private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            try
            {
                if (!config.AffectWarheadPanel || ev?.Player is null || ev.IsAllowed)
                    return;

                if (!ev.Player.HasKeycardPermission(PermissionFlags.AlphaWarhead))
                    return;

                if (!Approve(ev.Player, RemoteTarget.WarheadPanel, ev))
                    return;

                ev.IsAllowed = true;
                Trace(ev.Player, "panneau d'ogive", null);
            }
            catch (Exception e)
            {
                Log.Error($"OnActivatingWarheadPanel: {e}");
            }
        }

        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            try
            {
                if (!config.AffectScpLockers || ev?.Player is null || ev.InteractingChamber is null || ev.IsAllowed)
                    return;

                if (!ev.Player.HasKeycardPermission(ev.InteractingChamber.RequiredPermissions, true))
                    return;

                if (!Approve(ev.Player, RemoteTarget.ScpLocker, ev))
                    return;

                ev.IsAllowed = true;
                Trace(ev.Player, "casier", null);
            }
            catch (Exception e)
            {
                Log.Error($"OnInteractingLocker: {e}");
            }
        }

        private bool Approve(Player player, RemoteTarget target, object original)
        {
            if (!config.Extras.EnableEvents)
                return true;

            UsingKeycardEventArgs args = new UsingKeycardEventArgs(player, target, original);
            Events.OnUsingKeycard(args);
            return args.IsAllowed;
        }

        private void Trace(Player player, string target, string detail)
        {
            if (!config.Debug)
                return;

            Log.Debug($"{player.Nickname} a ouvert un/une {target} a distance{(string.IsNullOrEmpty(detail) ? string.Empty : $" ({detail})")}.");
        }
    }
}
