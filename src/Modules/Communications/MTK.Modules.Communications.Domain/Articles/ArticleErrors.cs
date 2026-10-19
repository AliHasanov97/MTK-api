using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Communications.Domain.Articles;

public static class ArticleErrors
{
    public static Error NotFound => new("Article.NotFound", "Məqalə tapılmadı.");
    public static Error CreateFailed => new("Article.CreateFailed", "Məqalə yaradıla bilmədi.");
    public static Error UpdateFailed => new("Article.UpdateFailed", "Məqalə yenilənə bilmədi.");
    public static Error DeleteFailed => new("Article.DeleteFailed", "Məqalə silinə bilmədi.");
    public static Error PublishFailed => new("Article.PublishFailed", "Məqalə yayımlana bilmədi.");
    public static Error AlreadyPublished => new("Article.AlreadyPublished", "Məqalə artıq yayımlanıb.");
}