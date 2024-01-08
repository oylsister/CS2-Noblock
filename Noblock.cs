using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace Noblock
{
    public class Noblock : BasePlugin
    {
        public override string ModuleName => "Noblock";
        public override string ModuleAuthor => "Oylsister";
        public override string ModuleVersion => "1.0";

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Post);
        }

        public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            var client = @event.Userid;

            ApplyNoBlock(client);

            return HookResult.Continue;
        }

        private void ApplyNoBlock(CCSPlayerController client, bool undo = false)
        {
            var clientPawn = client.PlayerPawn.Value;

            if (clientPawn == null)
                return;

            if (!clientPawn.IsValid || !client.PawnIsAlive)
                return;

            client.PrintToChat($"Attribute Group: {clientPawn.Collision.CollisionAttribute.CollisionGroup} \n" +
                $"Collision Group: {clientPawn.Collision.CollisionGroup}");

            AddTimer(0.0f, () =>
            {
                if (!undo)
                {
                    clientPawn.Collision.CollisionAttribute.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DEBRIS;
                    clientPawn.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DEBRIS;
                    CollisionRulesChanged(clientPawn);

                    client.PrintToChat($" {ChatColors.Green}[NoBlock]{ChatColors.Default} You have been applied no block. You will now can walk through other player!");
                }

                else
                {
                    clientPawn.Collision.CollisionAttribute.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_PLAYER;
                    clientPawn.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_PLAYER;
                    CollisionRulesChanged(clientPawn);

                    client.PrintToChat($" {ChatColors.Green}[NoBlock]{ChatColors.Default} You have been cancelled no block. You will now unable to walk through other player!");
                }
            });
        }

        private void CollisionRulesChanged(CCSPlayerPawn pawn)
        {
            VirtualFunction.CreateVoid<CCSPlayerPawn>(pawn.Handle, GameData.GetOffset("CollisionRulesChanged"))(pawn);
        }
    }
}
