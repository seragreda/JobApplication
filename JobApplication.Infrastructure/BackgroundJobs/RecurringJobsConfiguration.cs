using Hangfire;

namespace JobApplication.Infrastructure.BackgroundJobs;

public static class RecurringJobsConfiguration
{
    public static void RegisterRecurringJobs()
    {
        // 1) إغلاق الوظائف المفتوحة اللي فات عليها 30 يوم — كل يوم الساعة 12 بالليل
        RecurringJob.AddOrUpdate<CloseStaleJobsJob>(
            "auto-close-stale-jobs",
            job => job.ExecuteAsync(),
            "0 0 * * *");

        // 2) تنظيف التقديمات الملغاة القديمة (60 يوم) — كل يوم أحد الساعة 3 صباحًا
        RecurringJob.AddOrUpdate<CleanupCancelledApplicationsJob>(
            "cleanup-cancelled-applications",
            job => job.ExecuteAsync(),
            "0 3 * * 0");

        // 3) تقرير يومي — كل يوم الساعة 9 صباحًا
        RecurringJob.AddOrUpdate<DailyJobsReportJob>(
            "daily-jobs-report",
            job => job.ExecuteAsync(),
            "0 9 * * *");
    }
}