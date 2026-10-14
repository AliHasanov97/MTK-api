using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payroll_tax_brackets",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Party = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LowerBound = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpperBound = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_tax_brackets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payroll_runs",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CalculatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_runs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payslips",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GrossSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployeeDsmf = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployeeUnemployment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployeeMedicalInsurance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxableIncome = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IncomeTax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployerDsmf = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployerUnemployment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployerMedicalInsurance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEmployerCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payslips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payslips_payroll_runs_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalSchema: "payments",
                        principalTable: "payroll_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_runs_Year_Month",
                schema: "payments",
                table: "payroll_runs",
                columns: new[] { "Year", "Month" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_tax_brackets_EffectiveTo",
                schema: "payments",
                table: "payroll_tax_brackets",
                column: "EffectiveTo");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_tax_brackets_RateType_Party_EffectiveFrom",
                schema: "payments",
                table: "payroll_tax_brackets",
                columns: new[] { "RateType", "Party", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_payslips_EmployeeId",
                schema: "payments",
                table: "payslips",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_payslips_PayrollRunId",
                schema: "payments",
                table: "payslips",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_payslips_PayrollRunId_EmployeeId",
                schema: "payments",
                table: "payslips",
                columns: new[] { "PayrollRunId", "EmployeeId" },
                unique: true);

            // ---------------------------------------------------------------------
            // PLACEHOLDER dərəcələr — RƏSMİ MƏNBƏDƏN (Vergilər Nazirliyi / DSMF /
            // İcbari Tibbi Sığorta Fondu) TƏSDİQLƏNMƏYİB. Production-a buraxmazdan
            // əvvəl mütləq yoxlanılıb, lazım olsa dəyişdirilməlidir.
            // EffectiveFrom qəsdən uzaq keçmişə qoyulub ki, test zamanı istənilən
            // dövr üçün bir dərəcə tapılsın; real qanun dəyişiklikləri gələndə yeni
            // EffectiveFrom-lu sətir əlavə edilməli, köhnəsi Deactivate (EffectiveTo)
            // ilə bağlanmalıdır — kod dəyişmədən.
            // ---------------------------------------------------------------------
            var effectiveFrom = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var seededAt = new DateTimeOffset(2026, 10, 14, 0, 0, 0, TimeSpan.Zero);

            migrationBuilder.InsertData(
                schema: "payments",
                table: "payroll_tax_brackets",
                columns: new[] { "Id", "RateType", "Party", "LowerBound", "UpperBound", "Rate", "EffectiveFrom", "Description", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("41a8852c-012f-40c5-93cd-23473aaad25d"), "Dsmf", "Employee", 0m, 200m, 0.03m, effectiveFrom, "PLACEHOLDER — DSMF işçi payı, 200 AZN-ə qədər", seededAt },
                    { new Guid("bf8c68bb-94ed-4240-96c1-7a5effd068a7"), "Dsmf", "Employee", 200m, null, 0.10m, effectiveFrom, "PLACEHOLDER — DSMF işçi payı, 200 AZN-dən yuxarı", seededAt },

                    { new Guid("cbb4d9b8-6a7d-4003-ac97-140f508adc75"), "Dsmf", "Employer", 0m, 200m, 0.22m, effectiveFrom, "PLACEHOLDER — DSMF işəgötürən payı, 0-200 AZN", seededAt },
                    { new Guid("401dd9b9-3c02-405c-9ace-eead9454310c"), "Dsmf", "Employer", 200m, 8000m, 0.15m, effectiveFrom, "PLACEHOLDER — DSMF işəgötürən payı, 200-8000 AZN", seededAt },
                    { new Guid("40f535a8-e9ce-49f7-84b2-20810c6c4982"), "Dsmf", "Employer", 8000m, null, 0.11m, effectiveFrom, "PLACEHOLDER — DSMF işəgötürən payı, 8000 AZN-dən yuxarı", seededAt },

                    { new Guid("bd7fe3a4-d01b-49d1-be84-e91d0807a64f"), "Unemployment", "Employee", 0m, null, 0.005m, effectiveFrom, "PLACEHOLDER — İşsizlikdən sığorta, işçi payı (sabit 0.5%)", seededAt },
                    { new Guid("f767b5f1-865c-4df7-ae45-a5d6705416f2"), "Unemployment", "Employer", 0m, null, 0.005m, effectiveFrom, "PLACEHOLDER — İşsizlikdən sığorta, işəgötürən payı (sabit 0.5%)", seededAt },

                    { new Guid("aff02d22-2e3b-4584-a8ef-a7592a4ca05a"), "MedicalInsurance", "Employee", 0m, 8000m, 0.02m, effectiveFrom, "PLACEHOLDER — İcbari tibbi sığorta, işçi payı, 0-8000 AZN", seededAt },
                    { new Guid("07d472c1-4e4b-4d6a-83fc-d9989108ba16"), "MedicalInsurance", "Employee", 8000m, null, 0.005m, effectiveFrom, "PLACEHOLDER — İcbari tibbi sığorta, işçi payı, 8000 AZN-dən yuxarı", seededAt },
                    { new Guid("29aa3572-e890-4789-89f5-91288f7b077c"), "MedicalInsurance", "Employer", 0m, 8000m, 0.02m, effectiveFrom, "PLACEHOLDER — İcbari tibbi sığorta, işəgötürən payı, 0-8000 AZN", seededAt },
                    { new Guid("f6a7c1be-dfd9-4995-ad6c-85298c384fab"), "MedicalInsurance", "Employer", 8000m, null, 0.005m, effectiveFrom, "PLACEHOLDER — İcbari tibbi sığorta, işəgötürən payı, 8000 AZN-dən yuxarı", seededAt },

                    { new Guid("434ebbc4-0a60-4a46-9f64-f9a487443234"), "IncomeTax", "Employee", 0m, 8000m, 0.00m, effectiveFrom, "PLACEHOLDER — Gəlir vergisi, 0-8000 AZN (qeyri-neft sektoru güzəşti — tarix məhdudiyyətini yoxla)", seededAt },
                    { new Guid("8c71f822-d3e2-4142-81cf-5ea42e3d994f"), "IncomeTax", "Employee", 8000m, null, 0.14m, effectiveFrom, "PLACEHOLDER — Gəlir vergisi, 8000 AZN-dən yuxarı", seededAt }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payslips",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payroll_runs",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payroll_tax_brackets",
                schema: "payments");
        }
    }
}
