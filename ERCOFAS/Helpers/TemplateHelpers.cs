namespace NetStarter.Helpers
{
    /// <summary>
    /// The helper class for the template.
    /// </summary>
    public static class TemplateHelpers
    {
        public static string GetLabelCss(string status)
        {
            if (status == "Pending" || status == "Waiting for SOA Generation" || status == "Updated")
                return "label label-warning";
            else if (status == "Rejected") 
                return "label label-danger";
            else if (status == "Approved")
               return "label label-success";
            else
                return "label label-default";
        }
    }
}