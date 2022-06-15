using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BinksFarm.Classes
{
    public abstract class Leaderboard<T>
    {
        public int Capacity { get; init; }
        public List<T> Highscores { get; set; }

        public Leaderboard(int capacity)
        {
            Capacity = capacity;
            Highscores = new List<T>(capacity);
        }

        public abstract List<T> OrderHighscores(List<T> highscores);

        public void Add(T score)
        {
            Highscores.Add(score);
            Highscores = OrderHighscores(Highscores);
            Highscores = Highscores.Take(Capacity).ToList();
        }

        public void Save(string filename)
        {
            var json = JsonSerializer.Serialize(this);
            var base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(json));
            File.WriteAllText(filename, base64);
        }
    }
}
