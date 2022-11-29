using System;

namespace Salony.PathUrl
{
    public class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Appointment
        {
            public const string AddAppoiment = Base + "/AddAppoiment";
            public const string AllAppointmentsBooked = Base + "/AllAppointmentsBooked";
            public const string AllAppointmentsAvailable = Base + "/AllAppointmentsAvailable";
            public const string AllAppointmentsAvailableDuringDay = Base + "/AllAppointmentsAvailableDuringDay";
            public const string AddVacation = Base + "/AddVacation";
        }
        public static class Identity
        {
            public const string RegisterClient = Base + "/registerClient";
            public const string DeleteExpiredOrders = Base + "/DeleteExpiredOrders";
            public const string RegisterProvider = Base + "/RegisterProvider";
            public const string ConfirmCodeRegister = Base + "/ConfirmCodeRegister";
            public const string Login = Base + "/login";
            public const string UpdateDataUser = Base + "/UpdateDataUser";
            public const string ChangePassward = Base + "/ChangePassward";
            public const string ForgetPassword = Base + "/ForgetPassword";
            public const string ResendCode = Base + "/ResendCode";
            public const string ChangePasswordByCode = Base + "/ChangePasswordByCode";
            public const string logout = Base + "/logout";
            public const string ChangeLanguage = Base + "/ChangeLanguage";
            public const string RemoveAccount = Base + "/RemoveAccount";

            public const string ConfirmCodeRegisterMashglShow = Base + "/ConfirmCodeRegisterMashglShow";

            //public const string ChangeNotify = Base + "/ChangeNotify";
            //public const string GetAddresses = Base + "/GetAddresses";
            //public const string AddNewAddress = Base + "/AddNewAddress";
            //public const string EditAddress = Base + "/EditAddress";
            //public const string DeleteAddresses = Base + "/DeleteAddresses";
        }

        public static class Client
        {
            public const string ListCategories = Base + "/ListCategories";
            public const string ListBoutiques = Base + "/ListBoutiques";
            public const string Search = Base + "/Search";
            public const string SearchByService = Base + "/SearchByService";
            public const string ListMainServices = Base + "/ListMainServices";
            public const string FilterBoutiques = Base + "/FilterBoutiques";
            public const string GetBoutique = Base + "/GetBoutique";
            public const string AddToCart = Base + "/AddToCart";
            public const string GetCart = Base + "/GetCart";
            public const string GetCartWithDetails = Base + "/GetCartWithDetails";
            public const string DeleteServiceFromCart = Base + "/DeleteServiceFromCart";
            public const string SaveOrder = Base + "/SaveOrder";
            public const string CheckOrder = Base + "/CheckOrder";
            public const string ListOrders = Base + "/ListOrders";
            public const string ListAllOrders = Base + "/ListAllOrders";
            public const string PayOrder = Base + "/PayOrder";
            public const string CancelOrder = Base + "/CancelOrder";
            public const string RateOrder = Base + "/RateOrder";
            public const string GetOrder = Base + "/GetOrder";
            public const string ListNotifications = Base + "/ListNotifications";
            public const string DeleteNotifications = Base + "/DeleteNotifications";
            public const string GetDiscountPercentage = Base + "/GetDiscountPercentage";
            public const string ConfirmOrderPayment = Base + "/ConfirmOrderPayment";
            public const string GetankAccounts = Base + "/GetankAccounts";
            public const string FillData = Base + "/FillData";
            public const string ChargeWallet = Base + "/ChargeWallet";
            public const string GetUserWallet = Base + "/GetUserWallet";

            public const String GetWorkers = Base + "/GetWorkers";
            public const String NewWorker = Base + "/NewWorker";
            public const String UpdateWorker = Base + "/UpdateWorker";
            public const String NewWorkerEvaluation = Base + "/NewWorkerEvaluation";
            public const String NewSallonEvaluation = Base + "/NewSallonEvaluation";
            public const String GetPointBalanceForUser = Base + "/GetPointBalanceForUser";
        }


        public static class Provider
        {
            public const string GetRegisterData = Base + "/GetRegisterData";
            public const string ListProviderMainServices = Base + "/ListProviderMainServices";
            public const string AddSubService = Base + "/AddSubService";
            public const string EditSubService = Base + "/EditSubService";
            public const string DeleteSubService = Base + "/DeleteSubService";
            public const string GetCurrentBoutiqueData = Base + "/GetCurrentBoutiqueData";
            public const string GetCurrentBoutiqueAppPrecent = Base + "/GetCurrentBoutiqueAppPrecent";
            public const string ListProviderOffers = Base + "/ListProviderOffers";
            public const string AddProviderOffer = Base + "/AddProviderOffer";
            public const string DeleteProviderOffer = Base + "/DeleteProviderOffer";
            public const string ListProviderOrders = Base + "/ListProviderOrders";
            public const string GetOrderDetails = Base + "/GetOrderDetails";
            public const string ChangeOrderStatus = Base + "/ChangeOrderStatus";
            public const string GetProviderData = Base + "/GetProviderData";
            public const string UpdateProviderData = Base + "/UpdateProviderData";
            public const string CountProviderOrders = Base + "/CountProviderOrders";
            public const string DeleteProviderNotification = Base + "/DeleteProviderNotification";
            public const string ProviderWallet = Base + "/ProviderWallet";
            public const string SettlementRequest = Base + "/SettlementRequest";
            public const string ToggleAvailability = Base + "/ToggleAvailability";
            public const string Reports = Base + "/Reports";
            public const string ListSubServices = Base + "/ListSubServices";
            public const string AddEmployeeToSalon = Base + "/AddEmployeeToSalon";
            public const string ListSalonEmployees = Base + "/ListSalonEmployees";
            public const string DeleteSalonEmployee = Base + "/DeleteSalonEmployee";
        }

        public static class App
        {
            public const string AboutApp = Base + "/AboutApp";
            public const string ContactUs = Base + "/ContactUs";
        }

        public static class NewChat
        {
            public const string ListChatUsers = Base + "/ListChatUsers";
            public const string ListMessagesUser = Base + "/ListMessagesUser";
            public const string UploadNewFile = Base + "/UploadNewFile";
        }


    }
}
