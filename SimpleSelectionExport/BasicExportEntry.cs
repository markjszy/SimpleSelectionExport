using System;

namespace SimpleSelectionExport
{
    class BasicExportEntry
    {
        public BasicExportEntry(String title, String username, String password, String url, String notes)
        {
            this.Title = title;
            this.Username = username;
            this.Password = password;
            this.Url = url;
            this.Notes = notes;
        }

        public String Title { get; }

        public String Username { get; }

        public String Password { get; }

        public String Url { get; }

        public String Notes { get; }
    }
}
