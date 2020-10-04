/*
  SimpleSelectionExport - A simple plugin for exporting an arbitrary
                            selection of KeePass entries

  Copyright (C) 2020 Mark Szymanski <markjszy@gmail.com>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using KeePass.Plugins;
using System.Windows.Forms;

using KeePassLib;
using KeePassLib.Utility;

using CsvHelper;

// The namespace name must be the same as the file name of the
// plugin without its extension.
// For example, if you compile a plugin 'SamplePlugin.dll',
// the namespace must be named 'SamplePlugin'.
namespace SimpleSelectionExport
{

    enum ExportFormats
    {
        Text,
        Csv
    }

    public sealed class SimpleSelectionExportExt : Plugin
    {
        private IPluginHost m_host = null;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            m_host = host;

            return true;
        }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (t != PluginMenuType.Entry) return null;

            ToolStripMenuItem tsmi = new ToolStripMenuItem("Simple Selection Export");

            ToolStripMenuItem tsmiExportToText = new ToolStripMenuItem();
            tsmiExportToText.Text = "Export selection to TXT";
            tsmiExportToText.Click += this.ExportToText;
            tsmi.DropDownItems.Add(tsmiExportToText);

            ToolStripMenuItem tsmiExportToCsv = new ToolStripMenuItem();
            tsmiExportToCsv.Text = "Export selection to CSV";
            tsmiExportToCsv.Click += this.ExportToCsv;
            tsmi.DropDownItems.Add(tsmiExportToCsv);

            // TODO: would prefer to disable the main group in the context menu instead of the subgroup items
            tsmi.DropDownOpening += delegate (object sender, EventArgs e)
            {
                bool hasSelection = m_host.MainWindow.GetSelectedEntriesCount() > 0;
                tsmiExportToText.Enabled = tsmiExportToCsv.Enabled = hasSelection;
            };

            return tsmi;
        }

        private void ExportToCsv(object sender, EventArgs e)
        {
            ExportItems(ExportFormats.Csv);
        }

        private void ExportToText(object sender, EventArgs e)
        {
            ExportItems(ExportFormats.Text);
        }

        private void ExportItems(ExportFormats fmt)
        {
            var selectedPwEntries = m_host.MainWindow.GetSelectedEntries();

            // Just double-check in case we end up in this condition somehow
            if (selectedPwEntries == null || selectedPwEntries.Length == 0)
            {
                MessageService.ShowInfo("No entries selected for export");
                return;
            }

            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (fmt == ExportFormats.Text)
            {
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            } else
            {
                saveFileDialog.Filter = "csv files(*.csv)| *.csv | All files(*.*) | *.*";
            }

            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        var path = ((FileStream)streamWriter.BaseStream).Name;
                        if (fmt == ExportFormats.Text)
                        {
                            foreach (PwEntry entry in selectedPwEntries)
                            {

                                var simplified = GetExportRecordFromEntry(entry);
                                var textPattern = $@"Title: {simplified.Title}
Username: {simplified.Username}
Password: {simplified.Password}
Url: {simplified.Url}
Notes: {simplified.Notes}
----------------------------------------------------------
";

                                streamWriter.WriteLine(textPattern);
                            }
                        } else
                        {   
                            // Assume CSV since we currently have just two options
                            using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                            {
                                var records = new List<BasicExportEntry>();
                                
                                foreach (PwEntry entry in selectedPwEntries)
                                {
                                    records.Add(GetExportRecordFromEntry(entry));
                                }

                                csvWriter.WriteRecords(records);
                                
                            }
                        }
                        MessageService.ShowInfo(String.Format("Export to {0} completed.", path));
                    }
                }
            }
        }

        private BasicExportEntry GetExportRecordFromEntry(PwEntry pwEntry)
        {
           
            var title = getRawValue(pwEntry, PwDefs.TitleField);
            var username = getRawValue(pwEntry, PwDefs.UserNameField);
            var password = getRawValue(pwEntry, PwDefs.PasswordField);
            var url = getRawValue(pwEntry, PwDefs.UrlField);
            var notes = getRawValue(pwEntry, PwDefs.NotesField);
            
            return new BasicExportEntry(title, username, password, url, notes);
        }

        private String getRawValue(PwEntry entry, string field)
        {
            var protectedString = entry.Strings.Get(field);
            if (protectedString == null)
            {
                return "";
            }
            return protectedString.ReadString();
        }
    }
}

