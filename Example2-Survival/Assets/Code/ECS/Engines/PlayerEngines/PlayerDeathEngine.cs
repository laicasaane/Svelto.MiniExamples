using Svelto.Common;
using Svelto.ECS.Example.Survive.Characters;
using Svelto.ECS.Example.Survive.Characters.Player;
using Svelto.ECS.Extensions;

namespace Svelto.ECS.Example.Survive
{
    [Sequenced(nameof(EnginesEnum.PlayerDeathEngine))]
    public class PlayerDeathEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public PlayerDeathEngine(IEntityFunctions functions, IEntityStreamConsumerFactory consumerFactory)
        {
            _functions           = functions;
            _consumerFactory = consumerFactory;
        }

        public EntitiesDB entitiesDB { get; set; }

        public void Ready()
        {
            _consumer = _consumerFactory.GenerateConsumer<DeathComponent>(ECSGroups.PlayersGroup, "PlayerDeathEngine", 1);
        }

        public void Step()
        {
            while (_consumer.TryDequeue(out _, out EGID id))
            {
                var playerEntityViewComponent = entitiesDB.QueryEntity<PlayerEntityViewComponent>(id);
                
                playerEntityViewComponent.rigidBodyComponent.isKinematic = true;
                    
                _functions.RemoveEntity<PlayerEntityDescriptor>(id);
                _functions.RemoveEntity<PlayerGunEntityDescriptor>(new EGID(id.entityID, ECSGroups.PlayersGunsGroup));
            }
        }

        public string name => nameof(PlayerDeathEngine);
        
        readonly IEntityFunctions             _functions;
        readonly IEntityStreamConsumerFactory _consumerFactory;
        Consumer<DeathComponent> _consumer;
    }
}