using database_lib.DbHelpers;

namespace database_lib
{
    public static class GlobalValues
    {
        public static int HelpersTechnologyType
        {
            get 
            {
                return (int)DbHelpersFactory.HelperTechTypes.SimpleDataReader;
                //return (int)DbHelpersFactory.HelperTechTypes.EntityFramework;
            }
        }
    }
}
