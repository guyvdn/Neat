namespace GuyVdN.Neat
{
    public interface IMutationHelper
    {
        bool MutateConnectionWeights { get; }
    }

    public class MutationHelper: IMutationHelper
    {
        private readonly IGetRandom getRandom;
        private const double ChanceOfMutatingConnectionWeights = 0.8;
        

        public MutationHelper(IGetRandom getRandom)
        {
            this.getRandom = getRandom;
        }
        
        public bool MutateConnectionWeights => getRandom.Bool(ChanceOfMutatingConnectionWeights) ;
    }
}