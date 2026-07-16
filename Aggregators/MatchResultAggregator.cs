using Akasha.Data;
using Akasha.Events;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;

namespace Akasha.Aggregators
{
    public class MatchResultAggregator : IAggregator
    {
        private IEventBus _eventbus;
        private SortieManager _sortieManager;
        private PlayerSavedDataManager _savedDataManager;

        public MatchResultAggregator(IEventBus eventbus,
            SortieManager sortieManager,
            PlayerSavedDataManager savedDataManager)
        {
            _eventbus = eventbus;
            _sortieManager = sortieManager;
            _savedDataManager = savedDataManager;
        }

        public void Initialize()
        {
            _eventbus.Subscribe<MissionEndedEvent>(LogEnd);
        }

        public void LogEnd(MissionEndedEvent e)
        {
            GetWinnerAndLoser(e.declarant, e.endType, out var winner, out var loser);


        }

        // this will be broken only if devs rename/add factions
        private void GetWinnerAndLoser(FactionHQ declarant, EndType endType,
            out FactionHQ winner, out FactionHQ loser)
        {
            winner = null;
            loser = null;

            if (endType == EndType.Victory)
            {
                if (declarant.faction.factionName == "Primeva")
                {
                    winner = FactionRegistry.HqFromName("Primeva");
                    loser = FactionRegistry.HqFromName("Boscali");
                }
                else
                {
                    winner = FactionRegistry.HqFromName("Boscali");
                    loser = FactionRegistry.HqFromName("Primeva");
                }
            }
            else
            {
                if (declarant.faction.factionName == "Primeva")
                {
                    winner = FactionRegistry.HqFromName("Boscali");
                    loser = FactionRegistry.HqFromName("Primeva");
                }
                else
                {
                    winner = FactionRegistry.HqFromName("Primeva");
                    loser = FactionRegistry.HqFromName("Boscali");
                }
            }
        }
    }
}

    