# RemoteKeycard 4.0


> Portage EXILED 9.14.2 d'un plugin de **Beryl**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Ouvre portes, generateurs, casiers SCP et panneau d'ogive sans avoir la carte en
main : il suffit de la porter dans son inventaire.

**EXILED 9.14.2** — `dotnet build -c Release RemoteKeycard/RemoteKeycard.csproj`

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `amnesia_matters` | `true` | L'effet Amnesie bloque l'ouverture a distance |
| `affect_doors` | `true` | Agit sur les portes |
| `affect_generators` | `true` | Agit sur les generateurs |
| `affect_warhead_panel` | `true` | Agit sur le panneau de l'ogive |
| `affect_scp_lockers` | `true` | Agit sur les casiers SCP |
| `ignored_roles` | `[]` | Roles ne beneficiant jamais de l'ouverture a distance |
| `require_held_keycard` | `false` | Exige la carte en main plutot que dans l'inventaire |
| `extras.enable_events` | `false` | Expose l'evenement `UsingKeycard` aux autres plugins |

## API

Les plugins tiers peuvent intercepter chaque ouverture a distance :

```csharp
RemoteKeycard.API.Events.UsingKeycard += ev =>
{
    if (ev.Target == RemoteTarget.WarheadPanel)
        ev.IsAllowed = false;
};
```

`ev.Target` indique le type d'objet (`Door`, `Generator`, `WarheadPanel`,
`ScpLocker`), `ev.OriginalEvent` porte l'`EventArgs` EXILED d'origine. Le
systeme est desactive par defaut : l'activer coute une allocation par
interaction, inutile si aucun plugin ne l'ecoute.

## Dependances

Ce plugin depend de **ZoneShilari.Common**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `RemoteKeycard.dll` | `Plugins/7777/` |
| `ZoneShilari.Common.dll` | `Plugins/dependencies/` |
| HintServiceMeow | `Plugins/7777/` |

`ZoneShilari.Common.dll` ne va **jamais** dans `Plugins/7777/` : EXILED
tenterait de le charger comme plugin. Il doit etre deploye avant ce plugin et
mis a jour en meme temps.

Pour compiler ce depot isolement, cloner
[ZoneShilari.Common](https://github.com/Augaton/ZoneShilari.Common) a cote,
ou passer `-p:CommonProject=chemin/vers/ZoneShilari.Common.csproj`.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `remotekeycard status` | `remotekeycard.manage` | Cibles actives et garde-fous |

Alias `rk`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Note de portage

La version 3.x ciblait EXILED 4.1.0. Le portage touche surtout au systeme de
permissions : le rework des cartes de la 14.0 a remplace `KeycardPermissions`
par `DoorPermissionFlags`. Le type est aliase en un seul point par fichier
(`using PermissionFlags = ...`) pour qu'un changement d'API ne se propage pas.

Les exceptions ne sont plus masquees derriere une option de config : elles
partent systematiquement en `Log.Error`.
