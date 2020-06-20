namespace qsrv.Config
{
    public class WamsrvDevelopmentConfig
    {
        public readonly bool BlockResponses;

        public WamsrvDevelopmentConfig(bool blockResponses)
        {
            BlockResponses = blockResponses;
        }
    }
}