using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ReqqaDBUser");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    fullName = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    IsAvailable = table.Column<bool>(nullable: false),
                    isDeleted = table.Column<bool>(nullable: false),
                    code = table.Column<string>(nullable: true),
                    activeCode = table.Column<bool>(nullable: false),
                    img = table.Column<string>(nullable: true),
                    typeUser = table.Column<int>(nullable: false),
                    FK_BranchID = table.Column<int>(nullable: false),
                    showPassword = table.Column<string>(nullable: true),
                    lang = table.Column<string>(nullable: true),
                    registerDate = table.Column<DateTime>(nullable: false),
                    sendCodeDate = table.Column<DateTime>(nullable: false),
                    addressName = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    lng = table.Column<string>(nullable: true),
                    closeNotification = table.Column<bool>(nullable: false),
                    iDPhoto = table.Column<string>(nullable: true),
                    certificatePhoto = table.Column<string>(nullable: true),
                    ibanNumber = table.Column<string>(nullable: true),
                    wallet = table.Column<double>(nullable: false),
                    stableWallet = table.Column<double>(nullable: false),
                    TempPaymentId = table.Column<string>(nullable: true),
                    userWallet = table.Column<double>(nullable: false),
                    invitationCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bankNameAr = table.Column<string>(nullable: true),
                    bankNameEn = table.Column<string>(nullable: true),
                    bankAccountNumber = table.Column<string>(nullable: true),
                    OwnerNameAr = table.Column<string>(nullable: true),
                    FK_BranchID = table.Column<int>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    img = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    FK_BranchID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    FK_BranchID = table.Column<int>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContactUs",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    comment = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    FK_BranchID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Copons",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(nullable: true),
                    discPercentage = table.Column<double>(nullable: false),
                    FK_Branch = table.Column<int>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    keyMap = table.Column<string>(nullable: true),
                    aboutAppAr = table.Column<string>(nullable: true),
                    aboutAppEn = table.Column<string>(nullable: true),
                    condtionsAr = table.Column<string>(nullable: true),
                    condtionsEn = table.Column<string>(nullable: true),
                    paymentPolicyAr = table.Column<string>(nullable: true),
                    paymentPolicyEn = table.Column<string>(nullable: true),
                    appleStoreUrl = table.Column<string>(nullable: true),
                    googlePlayUrl = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    phone2 = table.Column<string>(nullable: true),
                    facebook = table.Column<string>(nullable: true),
                    twitter = table.Column<string>(nullable: true),
                    telegram = table.Column<string>(nullable: true),
                    instagram = table.Column<string>(nullable: true),
                    whatsApp = table.Column<string>(nullable: true),
                    snapChat = table.Column<string>(nullable: true),
                    youtube = table.Column<string>(nullable: true),
                    commercialRegister = table.Column<string>(nullable: true),
                    appPrecent = table.Column<double>(nullable: false),
                    appPrecentPercentage = table.Column<double>(nullable: false),
                    Deposit = table.Column<double>(nullable: false),
                    Tax = table.Column<double>(nullable: false),
                    TaxOfHome = table.Column<double>(nullable: false),
                    payText = table.Column<string>(nullable: true),
                    ExpireTime = table.Column<int>(nullable: false),
                    Screen1TitleAr = table.Column<string>(nullable: true),
                    Screen1TitleEn = table.Column<string>(nullable: true),
                    Screen2TitleAr = table.Column<string>(nullable: true),
                    Screen2TitleEn = table.Column<string>(nullable: true),
                    Screen3TitleAr = table.Column<string>(nullable: true),
                    Screen3TitleEn = table.Column<string>(nullable: true),
                    Screen1DescriptionAr = table.Column<string>(nullable: true),
                    Screen1DescriptionEn = table.Column<string>(nullable: true),
                    Screen2DescriptionAr = table.Column<string>(nullable: true),
                    Screen2DescriptionEn = table.Column<string>(nullable: true),
                    Screen3DescriptionAr = table.Column<string>(nullable: true),
                    Screen3DescriptionEn = table.Column<string>(nullable: true),
                    FK_BranchID = table.Column<int>(nullable: false),
                    invitationCodeBallance = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sliders",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    img = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    ProviderId = table.Column<int>(nullable: false),
                    FK_BranchID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceIds",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deviceID = table.Column<string>(nullable: true),
                    deviceType = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    FK_UserID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceIds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeviceIds_AspNetUsers_FK_UserID",
                        column: x => x.FK_UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialAccount",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FkProviderId = table.Column<string>(nullable: true),
                    PayOutPrice = table.Column<double>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialAccount_AspNetUsers_FkProviderId",
                        column: x => x.FkProviderId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoryNotify",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    FKUser = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryNotify", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryNotify_AspNetUsers_FKUser",
                        column: x => x.FKUser,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainServices",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    FK_CategoryID = table.Column<int>(nullable: true),
                    FK_BranchID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainServices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MainServices_Categories_FK_CategoryID",
                        column: x => x.FK_CategoryID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    FK_CityID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_FK_CityID",
                        column: x => x.FK_CityID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Cities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicesDelivery",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromInKM = table.Column<double>(nullable: false),
                    ToInKM = table.Column<double>(nullable: false),
                    DeliveryPrice = table.Column<double>(nullable: false),
                    ServiceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicesDelivery_MainServices_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "MainServices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProviderAditionalData",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    lng = table.Column<string>(nullable: true),
                    timeForm = table.Column<DateTime>(nullable: false),
                    timeTo = table.Column<DateTime>(nullable: false),
                    dayWorks = table.Column<string>(nullable: true),
                    salonType = table.Column<int>(nullable: false),
                    SalonUsersType = table.Column<int>(nullable: false),
                    descriptionAr = table.Column<string>(nullable: true),
                    descriptionEn = table.Column<string>(nullable: true),
                    bankAccount = table.Column<string>(nullable: true),
                    rate = table.Column<int>(nullable: false),
                    socialMediaProfile = table.Column<string>(nullable: true),
                    lastPayDate = table.Column<DateTime>(nullable: false),
                    paied = table.Column<double>(nullable: false),
                    IdentityImage = table.Column<string>(nullable: true),
                    commercialRegister = table.Column<string>(nullable: true),
                    CommercialRegisterImage = table.Column<string>(nullable: true),
                    HealthCardImage = table.Column<string>(nullable: true),
                    IbanNumber = table.Column<string>(nullable: true),
                    IbanImage = table.Column<string>(nullable: true),
                    FK_UserID = table.Column<string>(nullable: true),
                    FK_DistrictID = table.Column<int>(nullable: false),
                    FK_CategoryID = table.Column<int>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderAditionalData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderAditionalData_Categories_FK_CategoryID",
                        column: x => x.FK_CategoryID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderAditionalData_Districts_FK_DistrictID",
                        column: x => x.FK_DistrictID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Districts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderAditionalData_AspNetUsers_FK_UserID",
                        column: x => x.FK_UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    img = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false),
                    FK_ProviderAdditionalDataID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Offers_ProviderAditionalData_FK_ProviderAdditionalDataID",
                        column: x => x.FK_ProviderAdditionalDataID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "ProviderAditionalData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    typePay = table.Column<int>(nullable: false),
                    returnMoney = table.Column<bool>(nullable: false),
                    copon = table.Column<string>(nullable: true),
                    discountPercentage = table.Column<double>(nullable: false),
                    priceBeforeDisc = table.Column<double>(nullable: false),
                    valueOfDiscount = table.Column<double>(nullable: false),
                    valueOfTaxEleklil = table.Column<double>(nullable: false),
                    price = table.Column<double>(nullable: false),
                    paid = table.Column<bool>(nullable: false),
                    payOut = table.Column<bool>(nullable: false),
                    rate = table.Column<int>(nullable: false),
                    rateComment = table.Column<string>(nullable: true),
                    commentNote = table.Column<string>(nullable: true),
                    refusedReason = table.Column<string>(nullable: true),
                    pdf = table.Column<string>(nullable: true),
                    shippingPrice = table.Column<double>(nullable: false),
                    orderDate = table.Column<DateTime>(nullable: false),
                    address = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    lng = table.Column<string>(nullable: true),
                    Applicationpercentagepaid = table.Column<bool>(nullable: false),
                    Applicationpercentage = table.Column<double>(nullable: false),
                    Adminpercentage = table.Column<double>(nullable: false),
                    Providerpercentage = table.Column<double>(nullable: false),
                    ApplicationProviderpercentagepaid = table.Column<bool>(nullable: false),
                    ApplicationpercentageImg = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    AppCommission = table.Column<double>(nullable: false),
                    Deposit = table.Column<double>(nullable: false),
                    PaymentId = table.Column<string>(nullable: true),
                    FK_UserID = table.Column<string>(nullable: true),
                    FK_ProviderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_ProviderAditionalData_FK_ProviderID",
                        column: x => x.FK_ProviderID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "ProviderAditionalData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_FK_UserID",
                        column: x => x.FK_UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalonImages",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    img = table.Column<string>(nullable: true),
                    FK_ProviderAdditionalDataID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SalonImages_ProviderAditionalData_FK_ProviderAdditionalDataID",
                        column: x => x.FK_ProviderAdditionalDataID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "ProviderAditionalData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubServices",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: true),
                    nameEn = table.Column<string>(nullable: true),
                    duration = table.Column<double>(nullable: false),
                    price = table.Column<double>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    DescriptionAr = table.Column<string>(nullable: true),
                    DescriptionEn = table.Column<string>(nullable: true),
                    FK_MainServiceID = table.Column<int>(nullable: false),
                    FK_ProviderAdditionalDataID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubServices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubServices_MainServices_FK_MainServiceID",
                        column: x => x.FK_MainServiceID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "MainServices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubServices_ProviderAditionalData_FK_ProviderAdditionalDataID",
                        column: x => x.FK_ProviderAdditionalDataID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "ProviderAditionalData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_OrderId = table.Column<int>(nullable: false),
                    SenderId = table.Column<string>(nullable: true),
                    ReceiverId = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    DateSend = table.Column<DateTime>(nullable: false),
                    SenderSeen = table.Column<bool>(nullable: false),
                    ReceiverSeen = table.Column<bool>(nullable: false),
                    IsDeletedSender = table.Column<bool>(nullable: false),
                    IsDeletedReceiver = table.Column<bool>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Closed = table.Column<bool>(nullable: false),
                    TypeMessage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Orders_FK_OrderId",
                        column: x => x.FK_OrderId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    msgAr = table.Column<string>(nullable: true),
                    msgEn = table.Column<string>(nullable: true),
                    show = table.Column<bool>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    FK_UserID = table.Column<string>(nullable: true),
                    FK_OrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notifications_Orders_FK_OrderID",
                        column: x => x.FK_OrderID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_FK_UserID",
                        column: x => x.FK_UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderServices",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mainServiceID = table.Column<int>(nullable: false),
                    mainServiceNameAr = table.Column<string>(nullable: true),
                    mainServiceNameEn = table.Column<string>(nullable: true),
                    SubServiceID = table.Column<int>(nullable: false),
                    SubServicNameAr = table.Column<string>(nullable: true),
                    SubServicNameEn = table.Column<string>(nullable: true),
                    duration = table.Column<double>(nullable: false),
                    price = table.Column<double>(nullable: false),
                    deliveryPrice = table.Column<double>(nullable: false),
                    priceAtHome = table.Column<double>(nullable: false),
                    taxOfHome = table.Column<double>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    DescriptionAr = table.Column<string>(nullable: true),
                    DescriptionEn = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    address = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    lng = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeNameAr = table.Column<string>(nullable: true),
                    EmployeeNameEn = table.Column<string>(nullable: true),
                    EmployeeImg = table.Column<string>(nullable: true),
                    note = table.Column<string>(nullable: true),
                    FK_OrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderServices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderServices_Orders_FK_OrderID",
                        column: x => x.FK_OrderID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(nullable: true),
                    NameEn = table.Column<string>(nullable: true),
                    Img = table.Column<string>(nullable: true),
                    FK_ProviderAdditionalDataID = table.Column<int>(nullable: false),
                    FK_SubServiceID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Employees_ProviderAditionalData_FK_ProviderAdditionalDataID",
                        column: x => x.FK_ProviderAdditionalDataID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "ProviderAditionalData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Employees_SubServices_FK_SubServiceID",
                        column: x => x.FK_SubServiceID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "SubServices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(nullable: false),
                    address = table.Column<string>(nullable: true),
                    note = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    lng = table.Column<string>(nullable: true),
                    FK_SubServiceID = table.Column<int>(nullable: false),
                    FK_UserID = table.Column<string>(nullable: true),
                    Fk_EmployeeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Carts_SubServices_FK_SubServiceID",
                        column: x => x.FK_SubServiceID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "SubServices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_FK_UserID",
                        column: x => x.FK_UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Carts_Employees_Fk_EmployeeID",
                        column: x => x.Fk_EmployeeID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "ReqqaDBUser",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "ReqqaDBUser",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "ReqqaDBUser",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "ReqqaDBUser",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "ReqqaDBUser",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "ReqqaDBUser",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "ReqqaDBUser",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_FK_SubServiceID",
                schema: "ReqqaDBUser",
                table: "Carts",
                column: "FK_SubServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_FK_UserID",
                schema: "ReqqaDBUser",
                table: "Carts",
                column: "FK_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Fk_EmployeeID",
                schema: "ReqqaDBUser",
                table: "Carts",
                column: "Fk_EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceIds_FK_UserID",
                schema: "ReqqaDBUser",
                table: "DeviceIds",
                column: "FK_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_FK_CityID",
                schema: "ReqqaDBUser",
                table: "Districts",
                column: "FK_CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FK_ProviderAdditionalDataID",
                schema: "ReqqaDBUser",
                table: "Employees",
                column: "FK_ProviderAdditionalDataID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FK_SubServiceID",
                schema: "ReqqaDBUser",
                table: "Employees",
                column: "FK_SubServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccount_FkProviderId",
                schema: "ReqqaDBUser",
                table: "FinancialAccount",
                column: "FkProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryNotify_FKUser",
                schema: "ReqqaDBUser",
                table: "HistoryNotify",
                column: "FKUser");

            migrationBuilder.CreateIndex(
                name: "IX_MainServices_FK_CategoryID",
                schema: "ReqqaDBUser",
                table: "MainServices",
                column: "FK_CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FK_OrderId",
                schema: "ReqqaDBUser",
                table: "Messages",
                column: "FK_OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                schema: "ReqqaDBUser",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                schema: "ReqqaDBUser",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FK_OrderID",
                schema: "ReqqaDBUser",
                table: "Notifications",
                column: "FK_OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FK_UserID",
                schema: "ReqqaDBUser",
                table: "Notifications",
                column: "FK_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_FK_ProviderAdditionalDataID",
                schema: "ReqqaDBUser",
                table: "Offers",
                column: "FK_ProviderAdditionalDataID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FK_ProviderID",
                schema: "ReqqaDBUser",
                table: "Orders",
                column: "FK_ProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FK_UserID",
                schema: "ReqqaDBUser",
                table: "Orders",
                column: "FK_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderServices_FK_OrderID",
                schema: "ReqqaDBUser",
                table: "OrderServices",
                column: "FK_OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAditionalData_FK_CategoryID",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData",
                column: "FK_CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAditionalData_FK_DistrictID",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData",
                column: "FK_DistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAditionalData_FK_UserID",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData",
                column: "FK_UserID",
                unique: true,
                filter: "[FK_UserID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SalonImages_FK_ProviderAdditionalDataID",
                schema: "ReqqaDBUser",
                table: "SalonImages",
                column: "FK_ProviderAdditionalDataID");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesDelivery_ServiceId",
                schema: "ReqqaDBUser",
                table: "ServicesDelivery",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SubServices_FK_MainServiceID",
                schema: "ReqqaDBUser",
                table: "SubServices",
                column: "FK_MainServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_SubServices_FK_ProviderAdditionalDataID",
                schema: "ReqqaDBUser",
                table: "SubServices",
                column: "FK_ProviderAdditionalDataID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "BankAccounts",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Branches",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "ContactUs",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Copons",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "DeviceIds",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "FinancialAccount",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "HistoryNotify",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Offers",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "OrderServices",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "SalonImages",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "ServicesDelivery",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Sliders",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "SubServices",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "MainServices",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "ProviderAditionalData",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Districts",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "ReqqaDBUser");
        }
    }
}
