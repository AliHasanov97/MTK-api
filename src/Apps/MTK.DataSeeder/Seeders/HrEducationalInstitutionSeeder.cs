using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Hr.Application.EducationalInstitutions.CreateEducationalInstitution;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Təhsil ocaqlarını (EducationalInstitutions) seed edir — HrEmployeeSeeder-in təhsil tarixçəsi
/// qeydləri bunlara istinad edir.
/// </summary>
public sealed class HrEducationalInstitutionSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<HrEducationalInstitutionSeeder> _logger;
    private readonly IEducationalInstitutionRepository _repository;

    public static readonly List<Guid> InstitutionIds = new();

    private static readonly (string Name, EducationalInstitutionType Type)[] Institutions =
    {
        ("Bakı Dövlət Universiteti", EducationalInstitutionType.Ali),
        ("Azərbaycan Dövlət İqtisad Universiteti (UNEC)", EducationalInstitutionType.Ali),
        ("Azərbaycan Texniki Universiteti", EducationalInstitutionType.Ali),
        ("Azərbaycan Dillər Universiteti", EducationalInstitutionType.Ali),
        ("Xəzər Universiteti", EducationalInstitutionType.Ali),
        ("Bakı Dövlət Humanitar Kolleci", EducationalInstitutionType.Kollec),
        ("Azərbaycan Dövlət İqtisadiyyat Kolleci", EducationalInstitutionType.Kollec),
        ("Bakı Sənaye Kolleci", EducationalInstitutionType.Kollec),
    };

    public HrEducationalInstitutionSeeder(
        ISender sender,
        ILogger<HrEducationalInstitutionSeeder> logger,
        IEducationalInstitutionRepository repository)
    {
        _sender = sender;
        _logger = logger;
        _repository = repository;
    }

    public async Task SeedAsync()
    {
        var existing = await _repository.ListAsync();
        InstitutionIds.Clear();
        InstitutionIds.AddRange(existing.Select(i => i.Id));

        if (InstitutionIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} təhsil ocağı mövcuddur", InstitutionIds.Count);
            return;
        }

        foreach (var (name, type) in Institutions)
        {
            var result = await _sender.Send(new CreateEducationalInstitutionCommand { Name = name, Type = type });

            if (result.IsSuccess)
            {
                InstitutionIds.Add(result.Value.Id);
            }
            else
            {
                _logger.LogError("  ❌ Təhsil ocağı yaradıla bilmədi ({Name}): {Error}", name, result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} təhsil ocağı yaradıldı", InstitutionIds.Count);
    }
}
