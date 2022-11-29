namespace Salony.Enums
{
    public static class AllEnums
    {
        public enum TypeUser
        {
            admin = 0,
            client = 1,
            provider = 2
        }

        public enum SliderType
        {
            home = 0,
            salons = 1
        }

        public enum OrderStates
        {
            waiting = 0,
            accepted = 1,
            finished = 2,
            refused = 3,
            canceled = 4
        }

        public enum TypePay
        {
            cash = 0,
            online = 1,
            wallet = 2
        }

        public enum Roles
        {
            Admin = 0,
            Mobile = 1,
            SuperAdmin = 2,
            Sliders = 3,
            Cities = 4,
            Settings = 5,
            Users = 6,
            Categories = 7,
            ContactUs = 8,
            Copons = 9,
            Notifications = 10,
            BankAccounts = 11,
            Orders = 12,
            SpacePrice = 13
            //Packages = 5,
            //requestVip = 6,
            //Orders = 10,
            //Clients = 11
        }

        public enum FileTypeChat
        {
            text = 0,
            img = 1,
            audio = 2,
            file = 3
        }

        public enum FcmType
        {
            blockUser = -1,
            dashboard = 0,
            //newOrder = 1,
            //acceptOrder = 2,
            //refuseOrder = 3,
            //finishOrder = 4,
            chat = 5,
            //rate = 6
        }

        public enum SalonType
        {
            salon = 1,
            home = 2,
            both = 3
        }

        public enum SalonUsersType
        {
            both = 0,
            men = 1,
            women = 2
        }


        public enum BranchName
        {
            NoBranch,
            Salony,
            MashagilAwammer,
            CarePackage,
            MashaghilSherifa,
            //Salony,
            Eleklil,
            Haidy,
            MySpa,
            Lady,
            ToGo,
            Show
            //Hope not to Add More
        }

        public enum WeekDays
        {
            Saturday, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday
        }
    }
}