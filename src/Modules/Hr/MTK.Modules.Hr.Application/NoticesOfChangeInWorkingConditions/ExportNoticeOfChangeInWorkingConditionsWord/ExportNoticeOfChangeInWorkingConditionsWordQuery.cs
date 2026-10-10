using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.ExportNoticeOfChangeInWorkingConditionsWord;

public sealed record ExportNoticeOfChangeInWorkingConditionsWordQuery(Guid NoticeId)
    : IQuery<ExportNoticeOfChangeInWorkingConditionsWordResponse>;
