namespace MTK.Common.Application.Authorization;

public static class Permissions
{
    // ========== IDENTITY MODULE ==========

    // User Management
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";
    public const string UsersView = "users.view";
    public const string UsersManage = "users.manage";

    // Role Management
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";
    public const string RolesView = "roles.view";
    public const string RolesManage = "roles.manage";

    // Group Management
    public const string GroupsCreate = "groups.create";
    public const string GroupsUpdate = "groups.update";
    public const string GroupsDelete = "groups.delete";
    public const string GroupsView = "groups.view";
    public const string GroupsManage = "groups.manage";

    // ========== BUILDINGS MODULE ==========

    // Building Management
    public const string BuildingsCreate = "buildings.create";
    public const string BuildingsUpdate = "buildings.update";
    public const string BuildingsDelete = "buildings.delete";
    public const string BuildingsView = "buildings.view";
    public const string BuildingsManage = "buildings.manage";

    // Apartment Management
    public const string ApartmentsCreate = "apartments.create";
    public const string ApartmentsUpdate = "apartments.update";
    public const string ApartmentsDelete = "apartments.delete";
    public const string ApartmentsView = "apartments.view";
    public const string ApartmentsAssign = "apartments.assign";
    public const string ApartmentsTransfer = "apartments.transfer";
    public const string ApartmentsManage = "apartments.manage";

    // Owner Management
    public const string OwnersCreate = "owners.create";
    public const string OwnersUpdate = "owners.update";
    public const string OwnersView = "owners.view";
    public const string OwnersManage = "owners.manage";

    // Garage Management
    public const string GaragesCreate = "garages.create";
    public const string GaragesUpdate = "garages.update";
    public const string GaragesView = "garages.view";
    public const string GaragesManage = "garages.manage";

    // ========== BILLING MODULE (Future) ==========
    public const string InvoicesCreate = "invoices.create";
    public const string InvoicesView = "invoices.view";
    public const string InvoicesApprove = "invoices.approve";
    public const string InvoicesManage = "invoices.manage";

    public const string PaymentsProcess = "payments.process";
    public const string PaymentsView = "payments.view";
    public const string PaymentsManage = "payments.manage";

    // ========== FINANCE MODULE (Future) ==========
    public const string FinanceView = "finance.view";
    public const string FinanceManage = "finance.manage";

    // ========== EXPENSES MODULE (Future) ==========
    public const string ExpensesCreate = "expenses.create";
    public const string ExpensesApprove = "expenses.approve";
    public const string ExpensesView = "expenses.view";
    public const string ExpensesManage = "expenses.manage";

    // ========== MAINTENANCE MODULE (Future) ==========
    public const string ComplaintsCreate = "complaints.create";
    public const string ComplaintsAssign = "complaints.assign";
    public const string ComplaintsView = "complaints.view";
    public const string ComplaintsManage = "complaints.manage";

    public const string WorkOrdersCreate = "workorders.create";
    public const string WorkOrdersAssign = "workorders.assign";
    public const string WorkOrdersView = "workorders.view";
    public const string WorkOrdersManage = "workorders.manage";

    // ========== VOTING MODULE (Future) ==========
    public const string VotingCreate = "voting.create";
    public const string VotingVote = "voting.vote";
    public const string VotingView = "voting.view";
    public const string VotingManage = "voting.manage";

    // ========== REPORTS & DASHBOARDS ==========
    public const string ReportsView = "reports.view";
    public const string ReportsGenerate = "reports.generate";
    public const string DashboardView = "dashboard.view";
}
