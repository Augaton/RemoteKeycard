using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PermissionFlags = Exiled.API.Enums.KeycardPermissions;

namespace RemoteKeycard.API.Extensions
{
    public static class PlayerExtensions
    {
        public static bool HasKeycardPermission(this Player player, PermissionFlags required, bool requiresAll = false)
        {
            if (player is null || required == 0)
                return false;

            Config.Config config = Plugin.Instance.Config;

            if (config.AmnesiaMatters && player.IsEffectActive<AmnesiaItems>())
                return false;

            if (config.IgnoredRoles.Contains(player.Role.Type))
                return false;

            if (config.RequireHeldKeycard)
                return player.CurrentItem is Keycard held && Matches(held, required, requiresAll);

            foreach (Item item in player.Items)
            {
                if (item is Keycard keycard && Matches(keycard, required, requiresAll))
                    return true;
            }

            return false;
        }

        private static bool Matches(Keycard keycard, PermissionFlags required, bool requiresAll)
        {
            PermissionFlags owned = keycard.Permissions;

            return requiresAll
                ? (owned & required) == required
                : (owned & required) != 0;
        }
    }
}
