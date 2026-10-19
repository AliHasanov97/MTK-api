using MTK.Modules.Buildings.Domain.FileAttachments;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class FileAttachmentTests
{
    [Fact]
    public void Create_WithBuildingReference_StoresFileMetadata()
    {
        var buildingId = Guid.NewGuid();

        var attachment = FileAttachment.Create(
            "plan.pdf", "buildings/plan.pdf", "application/pdf", 1024,
            buildingId, null, null, null);

        Assert.Equal("plan.pdf", attachment.FileName);
        Assert.Equal("buildings/plan.pdf", attachment.ObjectKey);
        Assert.Equal("application/pdf", attachment.ContentType);
        Assert.Equal(1024, attachment.SizeBytes);
        Assert.Equal(buildingId, attachment.BuildingId);
    }

    [Fact]
    public void Create_WithoutFileName_Throws()
    {
        Assert.Throws<ArgumentException>(() => FileAttachment.Create(
            " ", "key", "text/plain", 1, Guid.NewGuid(), null, null, null));
    }

    [Fact]
    public void Create_WithoutRelatedEntity_Throws()
    {
        Assert.Throws<ArgumentException>(() => FileAttachment.Create(
            "file.txt", "key", "text/plain", 1, null, null, null, null));
    }
}
