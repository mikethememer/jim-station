- using Content.Server._Viva.GameTicking.Rules.Components;
  using Content.Server.Antag;
  using Content.Server.GameTicking.Rules;
  using Content.Server.Mind;
  using Content.Shared.Roles;
  using Robust.Shared.Prototypes;

namespace Content.Server._Viva.GameTicking.Rules;

public sealed class ConspiracyRuleSystem : GameRuleSystem<ConspiracyRuleComponent>
{

    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly MindSystem _mind = default!;

    [ValidatePrototypeId<EntityPrototype>] static EntProtoId mindRole = "MindRoleConspiracy";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ConspiracyRuleComponent, AfterAntagEntitySelectedEvent>(AfterSelected);
    }

    private void AfterSelected(Entity<ConspiracyRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        TryMakeConspiracy(args.EntityUid, ent.Comp);
    }

    public bool TryMakeConspiracy(EntityUid target, ConspiracyRuleComponent rule)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return false;

        _role.MindAddRole(mindId, mindRole.Id, mind, true);

        _antag.SendBriefing(target, Loc.GetString("conspiracy-briefing"), null, null);

        return true;
    }
}
