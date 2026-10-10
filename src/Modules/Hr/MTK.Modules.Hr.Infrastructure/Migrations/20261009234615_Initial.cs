using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.CreateSequence<int>(
                name: "application_number_seq",
                schema: "hr");

            migrationBuilder.CreateSequence<int>(
                name: "order_number_seq",
                schema: "hr");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "jsonb", nullable: true),
                    NewValues = table.Column<string>(type: "jsonb", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "EntityType", "Action" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "educational_institutions",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Name\",''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_educational_institutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "holiday_templates",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    DaysCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Name\",''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_message_consumers",
                schema: "hr",
                columns: table => new
                {
                    InboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_message_consumers", x => new { x.InboxMessageId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "jsonb", maxLength: 2000, nullable: false),
                    OccurredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Name\",''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "labor_code_cases",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Code\",'') || ' ' || coalesce(\"Name\",''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_labor_code_cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_labor_code_cases_labor_code_cases_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "hr",
                        principalTable: "labor_code_cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message_consumers",
                schema: "hr",
                columns: table => new
                {
                    OutboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_message_consumers", x => new { x.OutboxMessageId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "jsonb", maxLength: 2000, nullable: false),
                    OccurredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    TriggeredByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IdentityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "calendar_days",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HolidayTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DayType = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ApplicableWorkingDays = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Name\",''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_calendar_days_holiday_templates_HolidayTemplateId",
                        column: x => x.HolidayTemplateId,
                        principalSchema: "hr",
                        principalTable: "holiday_templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationNumber = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('hr.application_number_seq')"),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "PendingApproval"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"ApplicationNumber\"::text, ''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_applications_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "hr",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('hr.order_number_seq')"),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"OrderNumber\"::text, ''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "hr",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FathersName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telephone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HomeTelephoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_job_applications_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_job_applications_jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_on_non_workday_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_on_non_workday_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_on_non_workday_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employment_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LaborCodeCaseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_orders_job_applications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "hr",
                        principalTable: "job_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_orders_labor_code_cases_LaborCodeCaseId",
                        column: x => x.LaborCodeCaseId,
                        principalSchema: "hr",
                        principalTable: "labor_code_cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisterNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FathersName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    BirthDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FinCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IdCardNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContractNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SalaryBankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EmployeeBankAccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MaritalStatus = table.Column<int>(type: "integer", nullable: true),
                    NumberOfChildren = table.Column<int>(type: "integer", nullable: true),
                    MilitaryService = table.Column<int>(type: "integer", nullable: true),
                    Veteran = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Disability = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Education = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HomePhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RegisteredAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CurrentAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WorkingDays = table.Column<int>(type: "integer", nullable: true),
                    VacationDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ChildrenUnder14Count = table.Column<int>(type: "integer", nullable: true),
                    IsKarabakhWorker = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsSingleParent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasDisabledChild = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    StartWorkDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    EmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "FullTime"),
                    EmploymentOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Name\",'') || ' ' || coalesce(\"Surname\",'') || ' ' || coalesce(\"FathersName\",''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employees_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "hr",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employees_employment_orders_EmploymentOrderId",
                        column: x => x.EmploymentOrderId,
                        principalSchema: "hr",
                        principalTable: "employment_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employees_jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "application_for_change_of_position",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentJobId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewJobId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_for_change_of_position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_application_for_change_of_position_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_for_change_of_position_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_application_for_change_of_position_jobs_CurrentJobId",
                        column: x => x.CurrentJobId,
                        principalSchema: "hr",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_application_for_change_of_position_jobs_NewJobId",
                        column: x => x.NewJobId,
                        principalSchema: "hr",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bonus_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderExecutionSupervisorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BonusQuantity = table.Column<int>(type: "integer", nullable: false),
                    SalaryMonth = table.Column<int>(type: "integer", nullable: false),
                    SalaryYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bonus_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bonus_orders_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bonus_orders_employees_OrderExecutionSupervisorId",
                        column: x => x.OrderExecutionSupervisorId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bonus_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "edu_leave_app",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_edu_leave_app", x => x.Id);
                    table.ForeignKey(
                        name: "FK_edu_leave_app_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_edu_leave_app_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employee_education_histories",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationalInstitutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLevel = table.Column<int>(type: "integer", nullable: false),
                    Faculty = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Specialty = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DiplomaNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegisterNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_education_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_education_histories_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "hr",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_education_histories_educational_institutions_Educa~",
                        column: x => x.EducationalInstitutionId,
                        principalSchema: "hr",
                        principalTable: "educational_institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_education_histories_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_work_histories",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Position = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_work_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_work_histories_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "hr",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_work_histories_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_work_schedules",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    Monday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    Tuesday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    Wednesday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    Thursday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    Friday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    Saturday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    Sunday = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_work_schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_work_schedules_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employment_status_change_applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentEmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NewEmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderExecutionSupervisorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_status_change_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_status_change_applications_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employment_status_change_applications_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_status_change_applications_employees_OrderExecut~",
                        column: x => x.OrderExecutionSupervisorId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notice_of_change_in_working_conditions",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Index\"::text, ''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notice_of_change_in_working_conditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notice_of_change_in_working_conditions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "hr",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notice_of_change_in_working_conditions_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "salary_deductions",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    District = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    JudgementNo = table.Column<int>(type: "integer", nullable: false),
                    JudgementDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PercentageSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    StateFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Creditor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Debt = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_deductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salary_deductions_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_salary_deductions_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "unexcused_absences",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unexcused_absences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_unexcused_absences_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unexcused_absences_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "unpaid_leave_applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unpaid_leave_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_unpaid_leave_applications_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_unpaid_leave_applications_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacation_applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TotalRequestedDays = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_applications_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacation_applications_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacation_compensation_applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedDays = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_compensation_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_compensation_applications_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacation_compensation_applications_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacation_plans",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedDays = table.Column<int>(type: "integer", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReturnToWorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CalendarYear = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_plans_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacation_return_applications",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturnDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_return_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_return_applications_applications_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacation_return_applications_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warnings",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderExecutionSupervisorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DisciplinaryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Warning")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_warnings_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warnings_employees_OrderExecutionSupervisorId",
                        column: x => x.OrderExecutionSupervisorId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warnings_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_for_change_of_position",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationForChangeOfPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_for_change_of_position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_for_change_of_position_application_for_change_of_posi~",
                        column: x => x.ApplicationForChangeOfPositionId,
                        principalSchema: "hr",
                        principalTable: "application_for_change_of_position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_for_change_of_position_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_for_change_of_position_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "edu_leave_order",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLeaveApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReturnToWorkDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_edu_leave_order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_edu_leave_order_edu_leave_app_EducationLeaveApplicationId",
                        column: x => x.EducationLeaveApplicationId,
                        principalSchema: "hr",
                        principalTable: "edu_leave_app",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_edu_leave_order_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_edu_leave_order_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employment_status_change_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmploymentStatusChangeApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentEmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NewEmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderExecutionSupervisorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_status_change_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_status_change_orders_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_status_change_orders_employees_OrderExecutionSup~",
                        column: x => x.OrderExecutionSupervisorId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_status_change_orders_employment_status_change_ap~",
                        column: x => x.EmploymentStatusChangeApplicationId,
                        principalSchema: "hr",
                        principalTable: "employment_status_change_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employment_status_change_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "unpaid_leave_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UnpaidLeaveApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReturnToWorkDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unpaid_leave_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_unpaid_leave_orders_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unpaid_leave_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_unpaid_leave_orders_unpaid_leave_applications_UnpaidLeaveAp~",
                        column: x => x.UnpaidLeaveApplicationId,
                        principalSchema: "hr",
                        principalTable: "unpaid_leave_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacation_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VacationApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    VacationDays = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReturnToWorkDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_orders_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vacation_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacation_orders_vacation_applications_VacationApplicationId",
                        column: x => x.VacationApplicationId,
                        principalSchema: "hr",
                        principalTable: "vacation_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "compensation_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompensationApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompensatedDays = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compensation_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_compensation_orders_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_compensation_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_compensation_orders_vacation_compensation_applications_Comp~",
                        column: x => x.CompensationApplicationId,
                        principalSchema: "hr",
                        principalTable: "vacation_compensation_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacation_return_orders",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VacationReturnApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturnDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_return_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_return_orders_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vacation_return_orders_orders_Id",
                        column: x => x.Id,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vacation_return_orders_vacation_return_applications_Vacatio~",
                        column: x => x.VacationReturnApplicationId,
                        principalSchema: "hr",
                        principalTable: "vacation_return_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "file_attachments",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmploymentOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationForChangeOfPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderForChangeOfPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnexcusedAbsenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    NoticeOfChangeInWorkingConditionsId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarningId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompensationOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    VacationCompensationApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnpaidLeaveApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnpaidLeaveOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducationLeaveApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducationLeaveOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    VacationApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    VacationOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    BonusOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SalaryDeductionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmploymentStatusChangeApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmploymentStatusChangeOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkOnNonWorkdayOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    VacationReturnApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    VacationReturnOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    DocumentType = table.Column<int>(type: "integer", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', \"FileName\")", stored: true),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_attachments_application_for_change_of_position_Applica~",
                        column: x => x.ApplicationForChangeOfPositionId,
                        principalSchema: "hr",
                        principalTable: "application_for_change_of_position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_bonus_orders_BonusOrderId",
                        column: x => x.BonusOrderId,
                        principalSchema: "hr",
                        principalTable: "bonus_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_compensation_orders_CompensationOrderId",
                        column: x => x.CompensationOrderId,
                        principalSchema: "hr",
                        principalTable: "compensation_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_edu_leave_app_EducationLeaveApplicationId",
                        column: x => x.EducationLeaveApplicationId,
                        principalSchema: "hr",
                        principalTable: "edu_leave_app",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_edu_leave_order_EducationLeaveOrderId",
                        column: x => x.EducationLeaveOrderId,
                        principalSchema: "hr",
                        principalTable: "edu_leave_order",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_employment_orders_EmploymentOrderId",
                        column: x => x.EmploymentOrderId,
                        principalSchema: "hr",
                        principalTable: "employment_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_employment_status_change_applications_Empl~",
                        column: x => x.EmploymentStatusChangeApplicationId,
                        principalSchema: "hr",
                        principalTable: "employment_status_change_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_employment_status_change_orders_Employment~",
                        column: x => x.EmploymentStatusChangeOrderId,
                        principalSchema: "hr",
                        principalTable: "employment_status_change_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_job_applications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "hr",
                        principalTable: "job_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_notice_of_change_in_working_conditions_Not~",
                        column: x => x.NoticeOfChangeInWorkingConditionsId,
                        principalSchema: "hr",
                        principalTable: "notice_of_change_in_working_conditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_order_for_change_of_position_OrderForChang~",
                        column: x => x.OrderForChangeOfPositionId,
                        principalSchema: "hr",
                        principalTable: "order_for_change_of_position",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "hr",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_salary_deductions_SalaryDeductionId",
                        column: x => x.SalaryDeductionId,
                        principalSchema: "hr",
                        principalTable: "salary_deductions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_unexcused_absences_UnexcusedAbsenceId",
                        column: x => x.UnexcusedAbsenceId,
                        principalSchema: "hr",
                        principalTable: "unexcused_absences",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_unpaid_leave_applications_UnpaidLeaveAppli~",
                        column: x => x.UnpaidLeaveApplicationId,
                        principalSchema: "hr",
                        principalTable: "unpaid_leave_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_unpaid_leave_orders_UnpaidLeaveOrderId",
                        column: x => x.UnpaidLeaveOrderId,
                        principalSchema: "hr",
                        principalTable: "unpaid_leave_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_vacation_applications_VacationApplicationId",
                        column: x => x.VacationApplicationId,
                        principalSchema: "hr",
                        principalTable: "vacation_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_vacation_compensation_applications_Vacatio~",
                        column: x => x.VacationCompensationApplicationId,
                        principalSchema: "hr",
                        principalTable: "vacation_compensation_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_vacation_orders_VacationOrderId",
                        column: x => x.VacationOrderId,
                        principalSchema: "hr",
                        principalTable: "vacation_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_vacation_return_applications_VacationRetur~",
                        column: x => x.VacationReturnApplicationId,
                        principalSchema: "hr",
                        principalTable: "vacation_return_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_attachments_vacation_return_orders_VacationReturnOrder~",
                        column: x => x.VacationReturnOrderId,
                        principalSchema: "hr",
                        principalTable: "vacation_return_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_warnings_WarningId",
                        column: x => x.WarningId,
                        principalSchema: "hr",
                        principalTable: "warnings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_file_attachments_work_on_non_workday_orders_WorkOnNonWorkda~",
                        column: x => x.WorkOnNonWorkdayOrderId,
                        principalSchema: "hr",
                        principalTable: "work_on_non_workday_orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_for_change_of_position_CurrentJobId",
                schema: "hr",
                table: "application_for_change_of_position",
                column: "CurrentJobId");

            migrationBuilder.CreateIndex(
                name: "IX_application_for_change_of_position_EmployeeId",
                schema: "hr",
                table: "application_for_change_of_position",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_application_for_change_of_position_NewJobId",
                schema: "hr",
                table: "application_for_change_of_position",
                column: "NewJobId");

            migrationBuilder.CreateIndex(
                name: "IX_applications_ApplicationNumber",
                schema: "hr",
                table: "applications",
                column: "ApplicationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_CreatedAt",
                schema: "hr",
                table: "applications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_applications_CreatedById",
                schema: "hr",
                table: "applications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_applications_SearchVector",
                schema: "hr",
                table: "applications",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_applications_Status",
                schema: "hr",
                table: "applications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                schema: "hr",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                schema: "hr",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SearchVector",
                schema: "hr",
                table: "AuditLogs",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                schema: "hr",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                schema: "hr",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_bonus_orders_EmployeeId",
                schema: "hr",
                table: "bonus_orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_bonus_orders_OrderExecutionSupervisorId",
                schema: "hr",
                table: "bonus_orders",
                column: "OrderExecutionSupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_Date",
                schema: "hr",
                table: "calendar_days",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_Date_ApplicableWorkingDays",
                schema: "hr",
                table: "calendar_days",
                columns: new[] { "Date", "ApplicableWorkingDays" });

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_DayType",
                schema: "hr",
                table: "calendar_days",
                column: "DayType");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_DeletedAt",
                schema: "hr",
                table: "calendar_days",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_HolidayTemplateId",
                schema: "hr",
                table: "calendar_days",
                column: "HolidayTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_SearchVector",
                schema: "hr",
                table: "calendar_days",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_Year",
                schema: "hr",
                table: "calendar_days",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_Year_Date",
                schema: "hr",
                table: "calendar_days",
                columns: new[] { "Year", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_compensation_orders_CompensationApplicationId",
                schema: "hr",
                table: "compensation_orders",
                column: "CompensationApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_compensation_orders_EmployeeId",
                schema: "hr",
                table: "compensation_orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_app_EmployeeId",
                schema: "hr",
                table: "edu_leave_app",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_app_EndDate",
                schema: "hr",
                table: "edu_leave_app",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_app_StartDate",
                schema: "hr",
                table: "edu_leave_app",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_order_EducationLeaveApplicationId",
                schema: "hr",
                table: "edu_leave_order",
                column: "EducationLeaveApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_order_EmployeeId",
                schema: "hr",
                table: "edu_leave_order",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_order_EndDate",
                schema: "hr",
                table: "edu_leave_order",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_order_ReturnToWorkDate",
                schema: "hr",
                table: "edu_leave_order",
                column: "ReturnToWorkDate");

            migrationBuilder.CreateIndex(
                name: "IX_edu_leave_order_StartDate",
                schema: "hr",
                table: "edu_leave_order",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_educational_institutions_Name",
                schema: "hr",
                table: "educational_institutions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_educational_institutions_SearchVector",
                schema: "hr",
                table: "educational_institutions",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_employee_education_histories_CreatedById",
                schema: "hr",
                table: "employee_education_histories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_employee_education_histories_EducationalInstitutionId",
                schema: "hr",
                table: "employee_education_histories",
                column: "EducationalInstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_education_histories_EmployeeId",
                schema: "hr",
                table: "employee_education_histories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_education_histories_EndDate",
                schema: "hr",
                table: "employee_education_histories",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_histories_CreatedAt",
                schema: "hr",
                table: "employee_work_histories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_histories_CreatedById",
                schema: "hr",
                table: "employee_work_histories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_histories_EmployeeId",
                schema: "hr",
                table: "employee_work_histories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_schedules_EmployeeId_EffectiveFrom",
                schema: "hr",
                table: "employee_work_schedules",
                columns: new[] { "EmployeeId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_CreatedAt",
                schema: "hr",
                table: "employees",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_employees_CreatedById",
                schema: "hr",
                table: "employees",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_employees_EmploymentOrderId",
                schema: "hr",
                table: "employees",
                column: "EmploymentOrderId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL AND \"EmploymentOrderId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_employees_JobId",
                schema: "hr",
                table: "employees",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_employees_SearchVector",
                schema: "hr",
                table: "employees",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_employment_orders_JobApplicationId",
                schema: "hr",
                table: "employment_orders",
                column: "JobApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employment_orders_LaborCodeCaseId",
                schema: "hr",
                table: "employment_orders",
                column: "LaborCodeCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_status_change_applications_EmployeeId",
                schema: "hr",
                table: "employment_status_change_applications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_status_change_applications_OrderExecutionSupervi~",
                schema: "hr",
                table: "employment_status_change_applications",
                column: "OrderExecutionSupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_status_change_orders_EmployeeId",
                schema: "hr",
                table: "employment_status_change_orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_employment_status_change_orders_EmploymentStatusChangeAppli~",
                schema: "hr",
                table: "employment_status_change_orders",
                column: "EmploymentStatusChangeApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employment_status_change_orders_OrderExecutionSupervisorId",
                schema: "hr",
                table: "employment_status_change_orders",
                column: "OrderExecutionSupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_ApplicationForChangeOfPositionId",
                schema: "hr",
                table: "file_attachments",
                column: "ApplicationForChangeOfPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_BonusOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "BonusOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_CompensationOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "CompensationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_CreatedAt",
                schema: "hr",
                table: "file_attachments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_EducationLeaveApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "EducationLeaveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_EducationLeaveOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "EducationLeaveOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_EmployeeId",
                schema: "hr",
                table: "file_attachments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_EmploymentOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "EmploymentOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_EmploymentStatusChangeApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "EmploymentStatusChangeApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_EmploymentStatusChangeOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "EmploymentStatusChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_JobApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_NoticeOfChangeInWorkingConditionsId",
                schema: "hr",
                table: "file_attachments",
                column: "NoticeOfChangeInWorkingConditionsId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_OrderForChangeOfPositionId",
                schema: "hr",
                table: "file_attachments",
                column: "OrderForChangeOfPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_OrderId",
                schema: "hr",
                table: "file_attachments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_SalaryDeductionId",
                schema: "hr",
                table: "file_attachments",
                column: "SalaryDeductionId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_SearchVector",
                schema: "hr",
                table: "file_attachments",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_UnexcusedAbsenceId",
                schema: "hr",
                table: "file_attachments",
                column: "UnexcusedAbsenceId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_UnpaidLeaveApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "UnpaidLeaveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_UnpaidLeaveOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "UnpaidLeaveOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_VacationApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "VacationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_VacationCompensationApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "VacationCompensationApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_VacationOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "VacationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_VacationReturnApplicationId",
                schema: "hr",
                table: "file_attachments",
                column: "VacationReturnApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_VacationReturnOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "VacationReturnOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_WarningId",
                schema: "hr",
                table: "file_attachments",
                column: "WarningId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_WorkOnNonWorkdayOrderId",
                schema: "hr",
                table: "file_attachments",
                column: "WorkOnNonWorkdayOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_Day_Month",
                schema: "hr",
                table: "holiday_templates",
                columns: new[] { "Day", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_DeletedAt",
                schema: "hr",
                table: "holiday_templates",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_Name",
                schema: "hr",
                table: "holiday_templates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_SearchVector",
                schema: "hr",
                table: "holiday_templates",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_JobId",
                schema: "hr",
                table: "job_applications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_Name",
                schema: "hr",
                table: "jobs",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_SearchVector",
                schema: "hr",
                table: "jobs",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_labor_code_cases_Code",
                schema: "hr",
                table: "labor_code_cases",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_labor_code_cases_IsActive",
                schema: "hr",
                table: "labor_code_cases",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_labor_code_cases_ParentId",
                schema: "hr",
                table: "labor_code_cases",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_labor_code_cases_SearchVector",
                schema: "hr",
                table: "labor_code_cases",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_notice_of_change_in_working_conditions_CreatedById",
                schema: "hr",
                table: "notice_of_change_in_working_conditions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_notice_of_change_in_working_conditions_EmployeeId",
                schema: "hr",
                table: "notice_of_change_in_working_conditions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_notice_of_change_in_working_conditions_Index",
                schema: "hr",
                table: "notice_of_change_in_working_conditions",
                column: "Index",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_notice_of_change_in_working_conditions_SearchVector",
                schema: "hr",
                table: "notice_of_change_in_working_conditions",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_order_for_change_of_position_ApplicationForChangeOfPosition~",
                schema: "hr",
                table: "order_for_change_of_position",
                column: "ApplicationForChangeOfPositionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_for_change_of_position_EmployeeId",
                schema: "hr",
                table: "order_for_change_of_position",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CreatedAt",
                schema: "hr",
                table: "orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CreatedById",
                schema: "hr",
                table: "orders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderNumber",
                schema: "hr",
                table: "orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_SearchVector",
                schema: "hr",
                table: "orders",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_salary_deductions_EmployeeId",
                schema: "hr",
                table: "salary_deductions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_unexcused_absences_EmployeeId",
                schema: "hr",
                table: "unexcused_absences",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_applications_EmployeeId",
                schema: "hr",
                table: "unpaid_leave_applications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_applications_EndDate",
                schema: "hr",
                table: "unpaid_leave_applications",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_applications_StartDate",
                schema: "hr",
                table: "unpaid_leave_applications",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_orders_EmployeeId",
                schema: "hr",
                table: "unpaid_leave_orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_orders_EndDate",
                schema: "hr",
                table: "unpaid_leave_orders",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_orders_ReturnToWorkDate",
                schema: "hr",
                table: "unpaid_leave_orders",
                column: "ReturnToWorkDate");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_orders_StartDate",
                schema: "hr",
                table: "unpaid_leave_orders",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_unpaid_leave_orders_UnpaidLeaveApplicationId",
                schema: "hr",
                table: "unpaid_leave_orders",
                column: "UnpaidLeaveApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "hr",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                schema: "hr",
                table: "Users",
                column: "IdentityId",
                filter: "\"IdentityId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SearchVector",
                schema: "hr",
                table: "Users",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_applications_EmployeeId",
                schema: "hr",
                table: "vacation_applications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_applications_EndDate",
                schema: "hr",
                table: "vacation_applications",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_applications_StartDate",
                schema: "hr",
                table: "vacation_applications",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_compensation_applications_EmployeeId",
                schema: "hr",
                table: "vacation_compensation_applications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_orders_EmployeeId",
                schema: "hr",
                table: "vacation_orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_orders_EndDate",
                schema: "hr",
                table: "vacation_orders",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_orders_ReturnToWorkDate",
                schema: "hr",
                table: "vacation_orders",
                column: "ReturnToWorkDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_orders_StartDate",
                schema: "hr",
                table: "vacation_orders",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_orders_VacationApplicationId",
                schema: "hr",
                table: "vacation_orders",
                column: "VacationApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_CalendarYear",
                schema: "hr",
                table: "vacation_plans",
                column: "CalendarYear");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_EmployeeId",
                schema: "hr",
                table: "vacation_plans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_EmployeeId_CalendarYear",
                schema: "hr",
                table: "vacation_plans",
                columns: new[] { "EmployeeId", "CalendarYear" });

            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_StartDate",
                schema: "hr",
                table: "vacation_plans",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_return_applications_EmployeeId",
                schema: "hr",
                table: "vacation_return_applications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_return_applications_ReturnDate",
                schema: "hr",
                table: "vacation_return_applications",
                column: "ReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_return_orders_EmployeeId",
                schema: "hr",
                table: "vacation_return_orders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_return_orders_ReturnDate",
                schema: "hr",
                table: "vacation_return_orders",
                column: "ReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_return_orders_VacationReturnApplicationId",
                schema: "hr",
                table: "vacation_return_orders",
                column: "VacationReturnApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warnings_EmployeeId",
                schema: "hr",
                table: "warnings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_warnings_OrderExecutionSupervisorId",
                schema: "hr",
                table: "warnings",
                column: "OrderExecutionSupervisorId");

            // Shadow User snapshot-u Identity event-ləri ilə sync olunur; modul gəlməzdən əvvəl yaradılmış
            // istifadəçilər üçün event yoxdur — CreatedBy/Employee.UserId FK-ları pozulmasın deyə bir dəfəlik köçürülür.
            migrationBuilder.Sql(
                """
                INSERT INTO hr."Users" ("Id", "FirstName", "LastName", "Email", "PhoneNumber", "IdentityId", "Status", "CreatedAt", "UpdatedAt", "DeletedAt")
                SELECT "Id", "FirstName", "LastName", "Email", "PhoneNumber", "IdentityId", "Status", "CreatedAt", "UpdatedAt", "DeletedAt"
                FROM identity."Users"
                ON CONFLICT ("Id") DO NOTHING;
                """);

            // Əmək Məcəlləsi, maddə 47 (müddətli əmək müqaviləsi halları) — işə qəbul əmrinin LaborCodeCase-i bunlardan seçilir.
            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    parent_id UUID := gen_random_uuid();
                BEGIN
                    -- Maddə 47 (parent)
                    INSERT INTO hr.labor_code_cases ("Id", "Code", "Name", "ParentId", "IsActive", "CreatedAt")
                    VALUES (
                        parent_id,
                        '47',
                        'Müddətli əmək müqaviləsi bağlanan hallar',
                        NULL,
                        TRUE,
                        NOW()
                    );

                    -- Alt bəndlər (children)
                    INSERT INTO hr.labor_code_cases ("Id", "Code", "Name", "ParentId", "IsActive", "CreatedAt")
                    VALUES
                    (
                        gen_random_uuid(),
                        'a',
                        'İşçinin əmək qabiliyyətini müvəqqəti itirməsi, ezamiyyətdə, məzuniyyətdə olması, habelə iş yeri və vəzifəsi saxlanılmaqla qanunvericilikdə nəzərdə tutulmuş digər hallarda müəyyən səbəbdən müvəqqəti olaraq işə çıxmaması ilə əlaqədar onun əmək funksiyasının başqa şəxs tərəfindən icrasının zəruriyyəti olduqda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'b',
                        'Təbii və iqlim şəraitinə və ya işin xüsusiyyətinə görə il boyu görülə bilməyən mövsümü işlərin yerinə yetirilməsi zamanı',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'c',
                        'İşin həcminin və davamiyyətinin qısamüddətli olduğu təmir-tikinti, quraşdırma, yeni texnologiyanın tətbiqi və mənimsənilməsi, təcrübə-sınaq işlərinin aparılması və bu qəbildən olan digər işlərin görüldüyü hallarda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'ç',
                        'Müvafiq vəzifə (peşə) üzrə əmək funksiyasının mürəkkəbliyi, məsuliyyətliliyi baxımından işçinin əmək və peşə vərdişlərinin mənimsənilməsi, yüksək peşəkarlıq səviyyəsinin əldə edilməsi tələb olunan (stajkeçmə, rezidentura dövrləri) hallarda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'd',
                        'İşçinin şəxsi, ailə-məişət vəziyyəti ilə bağlı olan, o cümlədən işləməklə yanaşı təhsil aldığı, müəyyən səbəbdən müvafiq yaşayış məntəqəsində müvəqqəti yaşadığı, pensiya yaşına çatdıqda işləmək istəyi olduğu hallarda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'e',
                        'Müvafiq icra hakimiyyəti orqanının müəyyən etdiyi orqanın (qurumun) göndərişi ilə haqqı ödənilən ictimai işlər görülərkən',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'ə',
                        'Bu Məcəllənin 6-cı maddəsinin "c" bəndində göstərilən orqanlar istisna olmaqla seçkili orqanlarda (təşkilatlarda, birliklərdə) seçkili vəzifələrə seçilərkən',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'f',
                        'Tərəflərin hüquq bərabərliyi prinsipinə əməl edilməklə onların qarşılıqlı razılığı ilə',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'g',
                        'Bu Məcəllənin 46-cı maddəsinin ikinci hissəsində nəzərdə tutulmuş qaydada işçilərlə briqada, işçi qrupu halında kollektiv əmək müqaviləsi bağlandıqda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'ğ',
                        'Təhsilalanların istehsalat təlimi və təcrübəsi keçdiyi hallarda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'h',
                        'Əcnəbilər və vətəndaşlığı olmayan şəxslər iş icazəsi əsasında Azərbaycan Respublikasının ərazisində haqqı ödənilən əmək fəaliyyətinə cəlb olunduqda',
                        parent_id,
                        TRUE,
                        NOW()
                    ),
                    (
                        gen_random_uuid(),
                        'x',
                        'Qanunvericilikdə nəzərdə tutulmuş digər hallarda',
                        parent_id,
                        TRUE,
                        NOW()
                    );
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "calendar_days",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employee_education_histories",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employee_work_histories",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employee_work_schedules",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "file_attachments",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "inbox_message_consumers",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "outbox_message_consumers",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "vacation_plans",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "holiday_templates",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "educational_institutions",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "bonus_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "compensation_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "edu_leave_order",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employment_status_change_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "notice_of_change_in_working_conditions",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "order_for_change_of_position",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "salary_deductions",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "unexcused_absences",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "unpaid_leave_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "vacation_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "vacation_return_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "warnings",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "work_on_non_workday_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "vacation_compensation_applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "edu_leave_app",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employment_status_change_applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "application_for_change_of_position",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "unpaid_leave_applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "vacation_applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "vacation_return_applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employees",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employment_orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "job_applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "labor_code_cases",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "applications",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "jobs",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "hr");

            migrationBuilder.DropSequence(
                name: "application_number_seq",
                schema: "hr");

            migrationBuilder.DropSequence(
                name: "order_number_seq",
                schema: "hr");
        }
    }
}
