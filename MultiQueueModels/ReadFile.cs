using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
namespace MultiQueueModels
{
    public class ReadFiles
    {
        public List<int> ConfigValues { get; private set; }
        public List<(int, double)> InterarrivalDistribution { get; private set; }
        public List<(int, double)> ServiceDistribution { get; private set; }

        private bool isServiceSection = false; // To track if we are in the service section

        public ReadFiles()
        {
            ConfigValues = new List<int>();
            InterarrivalDistribution = new List<(int, double)>();
            ServiceDistribution = new List<(int, double)>();
        }

        public ReadFiles ParseFile(string filePath)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                Body body = wordDoc.MainDocumentPart?.Document.Body;

                foreach (var paragraph in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                {
                    string paragraphText = paragraph.InnerText.Trim();

                    if (string.IsNullOrWhiteSpace(paragraphText))
                        continue;

                    if (!IsNumeric(paragraphText))
                        continue;

                    if (paragraphText.Length == 2 && int.Parse(paragraphText) == -1)
                    {
                        isServiceSection = true; // Switch to service distribution section
                    }

                    ProcessParagraph(paragraphText);
                }
            }

            return new ReadFiles
            {
                ConfigValues = ConfigValues,
                InterarrivalDistribution = InterarrivalDistribution,
                ServiceDistribution = ServiceDistribution
            };
        }

        private void ProcessParagraph(string paragraphText)
        {
            if (!paragraphText.Contains(',') && !isServiceSection)
            {
                ParseConfigValues(paragraphText);
            }
            else if (paragraphText.Contains(",") && !isServiceSection)
            {
                ParseInterarrivalDistribution(paragraphText);
            }
            else if (paragraphText.Contains(",") && isServiceSection)
            {
                ParseServiceDistribution(paragraphText);
            }
        }

        private void ParseConfigValues(string paragraphText)
        {
            ConfigValues.Add(int.Parse(paragraphText));
        }

        private void ParseInterarrivalDistribution(string paragraphText)
        {
            var values = paragraphText.Split(',');
            int value = int.Parse(values[0]);
            double probability = double.Parse(values[1]);
            InterarrivalDistribution.Add((value, probability));
        }

        private void ParseServiceDistribution(string paragraphText)
        {
            var values = paragraphText.Split(',');
            int value = int.Parse(values[0]);
            double probability = double.Parse(values[1]);

            ServiceDistribution.Add((value, probability));
        }

        public static bool IsNumeric(string input)
        {
            var parts = input.Split(',');

            foreach (var part in parts)
            {
                if (!double.TryParse(part.Trim(), out _))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
