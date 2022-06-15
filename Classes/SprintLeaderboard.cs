using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinksFarm.Classes
{
    public struct SprintScore 
    {
        public double Time { get; set; }
    }

    internal class SprintLeaderboard : Leaderboard<SprintScore>
    {
        public SprintLeaderboard(int capacity)
            : base(capacity)
        {
        }

        public override List<SprintScore> OrderHighscores(List<SprintScore> highscores)
            => highscores.OrderBy(x => x.Time).ToList();
    }
}
