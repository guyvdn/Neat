using System;

namespace GuyVdN.Neat
{
    public class NewPlayerAction
    {
        private Action<Genome> action;
        public void Execute(Genome playerGenome)
        {
            action.Invoke(playerGenome);
        }

        public static readonly NewPlayerAction AddRandomConnection = new NewPlayerAction
        {
            action = playerGenome => playerGenome.MutateAddConnection()
        };

        public static readonly NewPlayerAction FullyConnect = new NewPlayerAction
        {
            action = playerGenome => playerGenome.FullyConnect()
        };
    }
}