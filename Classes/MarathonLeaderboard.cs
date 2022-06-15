using BinksFarm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinksFarm.Classes
{
    public struct MarathonScore
    {
        public int Score { get; set; }
    }

    public class MarathonLeaderboard : Leaderboard<MarathonScore>
    {
        public MarathonLeaderboard(int capacity)
            : base(capacity)
        {

        }

        public override List<MarathonScore> OrderHighscores(List<MarathonScore> highscores) 
            => highscores.OrderBy(x => x.Score).ToList();
    }
}
