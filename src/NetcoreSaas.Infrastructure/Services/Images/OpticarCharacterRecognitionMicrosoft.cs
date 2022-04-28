using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NetcoreSaas.Application.Services.Images;
using NetcoreSaas.Domain.Settings;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;

namespace NetcoreSaas.Infrastructure.Services.Images
{
    public class OpticalCharacterRecognitionMicrosoft : IOpticalCharacterRecognitionService
    {
        private readonly OpticalCharacterRecognitionSettings _ocrSettings;

        public OpticalCharacterRecognitionMicrosoft(IOptions<OpticalCharacterRecognitionSettings> ocrSettings)
        {
            _ocrSettings = ocrSettings.Value;
        }

        public async Task<IEnumerable<string>> ReadLinesFromImage(FileStream stream)
        {
            var computerClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_ocrSettings.Key)) { Endpoint = _ocrSettings.Url };
            return await ReadImage(computerClient, stream);
        }

        private static async Task<List<string>> ReadImage(ComputerVisionClient client, FileStream stream)
        {
            try
            {
                client.HttpClient.Timeout = TimeSpan.FromSeconds(20);
                var textHeaders = await client.ReadInStreamAsync(stream, language: "es");
                // After the request, get the operation location (operation ID)
                var operationLocation = textHeaders.OperationLocation;

                // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
                const int numberOfCharsInOperationId = 36;
                var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract the text
                ReadOperationResult results;
                do
                {
                    results = await client.GetReadResultAsync(Guid.Parse(operationId));
                } while ((results.Status == OperationStatusCodes.Running ||
                          results.Status == OperationStatusCodes.NotStarted));

                // Display the found text.
                var textUrlFileResults = results.AnalyzeResult.ReadResults;
                var lines = new List<string>();
                var pageNumber = 1;
                foreach (var page in textUrlFileResults)
                {
                    var lineNumber = 1;
                    foreach (var line in page.Lines)
                    {
                        Console.WriteLine($"Page [{pageNumber}] Line [{lineNumber}]: " + line.Text);
                        lines.Add(line.Text);
                        lineNumber++;
                    }

                    pageNumber++;
                }
                return lines;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error escaneando: " + ex);
                return new List<string>();
            }
        }
    }
}