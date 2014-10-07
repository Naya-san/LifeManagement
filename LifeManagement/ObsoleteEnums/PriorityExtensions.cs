namespace LifeManagement.ObsoleteEnums
{
    public static class PriorityExtensions
    {
        public static string ToLocalizedString(this Priority priority)
        {
            switch (priority)
            {
                case Priority.NoUrgentImportant :
                  return Resources.ResourceScr.NoUrgentImportant;
                case Priority.UrgentNoImportant :
                  return Resources.ResourceScr.UrgentNoImportant;
                case Priority.UrgentImportant :
                  return Resources.ResourceScr.UrgentImportant;
                default:
                  return Resources.ResourceScr.NoUrgentNoImportant;
            }
        }
    }
}