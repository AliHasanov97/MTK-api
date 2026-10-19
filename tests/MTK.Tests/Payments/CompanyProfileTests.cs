using Moq;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Companies;
using MTK.Modules.Payments.Application.Companies.Commands.UpsertCompany;
using MTK.Modules.Payments.Domain.Companies;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Presentation.Controllers;
using MTK.Modules.Payments.Application.Companies.Queries.GetCompany;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class CompanyProfileTests
{
    [Fact]
    public void Create_StoresPublicContactInformationAndDefaultsEmail()
    {
        var company = Company.Create("MTK", null, "Baku", phone: "+994 12 000 00 00", workHours: "09:00–18:00");
        var response = CompanyResponse.FromDomain(company);

        Assert.Equal("info@mtk.az", response.Email);
        Assert.Equal("+994 12 000 00 00", response.Phone);
        Assert.Equal("Baku", response.Address);
        Assert.Equal("09:00–18:00", response.WorkHours);
    }

    [Fact]
    public async Task UpsertHandler_CreatesCompanyWithContactFields()
    {
        var repository = new Mock<ICompanyRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        Company? added = null;
        repository.Setup(x => x.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Company?)null);
        repository.Setup(x => x.Add(It.IsAny<Company>())).Callback<Company>(company => added = company);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpsertCompanyCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new UpsertCompanyCommand("MTK", "Director", "Baku",
            "info@mtk.az", "+994 12 000 00 00", "09:00–18:00"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(added);
        Assert.Equal("info@mtk.az", result.Value.Email);
        Assert.Equal("+994 12 000 00 00", result.Value.Phone);
        Assert.Equal("Baku", result.Value.Address);
        Assert.Equal("09:00–18:00", result.Value.WorkHours);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpsertHandler_UpdatesExistingCompanyContactFields()
    {
        var repository = new Mock<ICompanyRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var company = Company.Create("Old name", null, "Old address", "old@example.com");
        repository.Setup(x => x.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(company);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpsertCompanyCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new UpsertCompanyCommand("MTK", null, "Baku",
            "info@mtk.az", null, "09:00–18:00"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("MTK", company.Name);
        Assert.Equal("info@mtk.az", result.Value.Email);
        Assert.Null(result.Value.Phone);
        Assert.Equal("Baku", result.Value.Address);
        Assert.Equal("09:00–18:00", result.Value.WorkHours);
    }

    [Fact]
    public async Task PublicEndpoint_ReturnsContactFieldsForFrontend()
    {
        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<GetCompanyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new CompanyResponse(Guid.NewGuid(), "MTK", "Director", "Baku",
                "info@mtk.az", "+994 12 000 00 00", "09:00–18:00")));
        var controller = new CompaniesController(sender.Object);

        var action = await controller.GetPublic(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action);
        var data = ok.Value!.GetType().GetProperty("data")!.GetValue(ok.Value)!;
        Assert.Equal("info@mtk.az", data.GetType().GetProperty("Email")!.GetValue(data));
        Assert.Equal("+994 12 000 00 00", data.GetType().GetProperty("Phone")!.GetValue(data));
        Assert.Equal("Baku", data.GetType().GetProperty("Address")!.GetValue(data));
        Assert.Equal("09:00–18:00", data.GetType().GetProperty("WorkHours")!.GetValue(data));
    }
}
