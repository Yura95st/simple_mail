
namespace database_lib.DbHelpers
{
    public static class DbHelpersFactory
    {
        public enum HelperTechTypes
        { 
            SimpleDataReader,
            EntityFramework
        }

        public static IUserDbHelper GetUserDbHelper(int helperTechnologyType)
        {
            switch (helperTechnologyType)
            {
                case (int)HelperTechTypes.SimpleDataReader:
                    {
                        return UserDbHelper.Instance;
                    }

                case (int)HelperTechTypes.EntityFramework:
                    {
                        return UserDbHelper_EF.Instance;
                    }

                default:
                    {
                        return UserDbHelper.Instance;
                    }
            }
        }

        public static IMessageDbHelper GetMessageDbHelper(int helperTechnologyType)
        {
            switch (helperTechnologyType)
            {
                case (int)HelperTechTypes.SimpleDataReader:
                    {
                        return MessageDbHelper.Instance;
                    }

                case (int)HelperTechTypes.EntityFramework:
                    {
                        return MessageDbHelper_EF.Instance;
                    }

                default:
                    {
                        return MessageDbHelper.Instance;
                    }
            }
        }
    }
}
