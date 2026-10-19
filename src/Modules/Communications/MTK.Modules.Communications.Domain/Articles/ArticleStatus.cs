namespace MTK.Modules.Communications.Domain.Articles;

/// <summary>
/// Məqalə statusu
/// </summary>
public enum ArticleStatus
{
    /// <summary>
    /// Qaralama - hələ yayımlanmayıb
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Yayımlanıb - aktiv
    /// </summary>
    Published = 1,

    /// <summary>
    /// Arxivləşdirilib - gizli
    /// </summary>
    Archived = 2
}