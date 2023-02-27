// DICOM DEMO

using Microsoft.AspNetCore.Identity;
using Microsoft.Health.Dicom.Client; // I am using the DICOM Client Package from Microsoft.Health.Dicom.Client
using Microsoft.Identity.Client; // I am using the Package Microsoft.Identity.Client from nuget.org
using System.Drawing;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using System;


namespace MyDICOM
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string webServerUrl = "https://jmnetohdswrkspc-jmnetohdsdicom.dicom.azurehealthcareapis.com";


            string[] StudyInstanceUID =  {
                                     "1.2.826.0.1.3680043.8.498.13230779778012324449356534479549187420",        // Blue Circle
                                     "1.2.826.0.1.3680043.8.498.13230779778012324449356534479549187420",        // Green Square
                                     "1.2.826.0.1.3680043.8.498.13230779778012324449356534479549187420"         // Red Triangle
                                     };

            string[] SeriesInstanceUID = {
                                    "1.2.826.0.1.3680043.8.498.77033797676425927098669402985243398207",         // Blue Circle
                                    "1.2.826.0.1.3680043.8.498.45787841905473114233124723359129632652",         // Green Square
                                    "1.2.826.0.1.3680043.8.498.45787841905473114233124723359129632652"          // Red Triangle
                                     };

            string[] SOPInstanceUID =  {
                                     "1.2.826.0.1.3680043.8.498.13273713909719068980354078852867170114",        // Blue Circle
                                     "1.2.826.0.1.3680043.8.498.12714725698140337137334606354172323212",        // Green Square
                                     "1.2.826.0.1.3680043.8.498.47359123102728459884412887463296905395"         // Red Triangle
                                     };


            bool cont = true;
            while (cont)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Azure DICOM Service Demo\n");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Store Blue Circle");
                Console.WriteLine("2 - Store Blue Green Square");
                Console.WriteLine("3 - Store Red Triangle");
                Console.WriteLine("4 - Retrieve all Instances within a study");
                Console.WriteLine("5 - Delete Study");
                Console.WriteLine("6 - Retrieve Specific Instance");
                Console.Write("\nSelect option:");

                ConsoleKeyInfo key = Console.ReadKey();

                try
                {
                    // Get Bearer Access Token
                    string bearertoken = GetAccessToken().Result;

                    // HttpClient with authorization Header
                    var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(webServerUrl);
                    IDicomWebClient client = new DicomWebClient(httpClient);
                    client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearertoken);

                    // Execute Requested DICOM Operation
                    switch (key.Key)
                    {
                        case ConsoleKey.D0:
                            cont = false;
                            continue;
                            break;

                        case ConsoleKey.D1:

                            {
                                Console.WriteLine("\nStore instance of blue-circle");

                                // Saving the Red Triangle
                                DicomFile dicomFile = await DicomFile.OpenAsync(@"C:\Users\jmneto\OneDrive\Projects\c#\DICOM\dcms\blue-circle.dcm");
                                DicomWebResponse response = await client.StoreAsync(new[] { dicomFile }, StudyInstanceUID[0]);

                                // Show the Stored Study 
                                Console.WriteLine($"ImageComments: {dicomFile.Dataset.GetString(DicomTag.ImageComments)} Study Date: {dicomFile.Dataset.GetDateTime(DicomTag.StudyDate, DicomTag.StudyTime)}");


                                Console.WriteLine("\n\ncurl --request POST \"{Service URL}/v{version}/studies/1.2.826.0.1.3680043.8.498.13230779778012324449356534479549187420\"\r\n--header \"Accept: application/dicom+json\"\r\n--header \"Content-Type: multipart/related; type=\\\"application/dicom\\\"\"\r\n--header \"Authorization: Bearer {token value}\"\r\n--form \"file1=@{path-to-dicoms}/blue-circle.dcm;type=application/dicom\"");


                                // Show Operation Status
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(response.StatusCode);
                            }

                            break;
                        case ConsoleKey.D2:

                            {
                                Console.WriteLine("\nStore instance of green-square");

                                // Saving the Red Triangle
                                DicomFile dicomFile = await DicomFile.OpenAsync(@"C:\Users\jmneto\OneDrive\Projects\c#\DICOM\dcms\green-square.dcm");
                                DicomWebResponse response = await client.StoreAsync(new[] { dicomFile }, StudyInstanceUID[1]);

                                // Show the Stored Study 
                                Console.WriteLine($"ImageComments: {dicomFile.Dataset.GetString(DicomTag.ImageComments)} Study Date: {dicomFile.Dataset.GetDateTime(DicomTag.StudyDate, DicomTag.StudyTime)}");

                                // Show Operation Status
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(response.StatusCode);

                            }

                            break;
                        case ConsoleKey.D3:

                            {
                                Console.WriteLine("\nStore instance of red-triangle");

                                // Saving the Red Triangle
                                DicomFile dicomFile = await DicomFile.OpenAsync(@"C:\Users\jmneto\OneDrive\Projects\c#\DICOM\dcms\red-triangle.dcm");
                                DicomWebResponse response = await client.StoreAsync(new[] { dicomFile }, StudyInstanceUID[2]);

                                // Show the Stored Study 
                                Console.WriteLine($"ImageComments: {dicomFile.Dataset.GetString(DicomTag.ImageComments)} Study Date: {dicomFile.Dataset.GetDateTime(DicomTag.StudyDate, DicomTag.StudyTime)}");

                                // Show Operation Status
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(response.StatusCode);

                            }

                            break;
                        case ConsoleKey.D4:
                            {
                                Console.WriteLine("\nRetrieve all instances within a study");

                                DicomWebAsyncEnumerableResponse<DicomFile> response = await client.RetrieveStudyAsync(StudyInstanceUID[0]);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(response.StatusCode);

                                await foreach (DicomFile dicomFile in response)
                                {
                                    string patientName = dicomFile.Dataset.GetString(DicomTag.PatientName);
                                    string studyId = dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID);
                                    string seriesNumber = dicomFile.Dataset.GetString(DicomTag.SeriesInstanceUID);
                                    string instanceNumber = dicomFile.Dataset.GetString(DicomTag.InstanceNumber);
                                    string imageComments = dicomFile.Dataset.GetString(DicomTag.ImageComments);
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine($"\nPatient Name: {patientName}   Study Id: {studyId}   Series: {seriesNumber}  Instance Number: {instanceNumber}  Image Comments: {imageComments}");

                                    dicomFile.Save($"C:\\Users\\jmneto\\OneDrive\\Projects\\c#\\DICOMViewer\\dcm\\{studyId}{seriesNumber}{instanceNumber}.dcm");
                                }
                            }

                            break;
                        case ConsoleKey.D5:
                            {
                                Console.WriteLine("\nDelete a study");

                                DicomWebResponse response = await client.DeleteStudyAsync(StudyInstanceUID[0]);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(response.StatusCode);
                            }
                            break;
                        case ConsoleKey.D6:
                            {
                                DicomWebResponse<DicomFile> response = await client.RetrieveInstanceAsync(StudyInstanceUID[2], SeriesInstanceUID[2], SOPInstanceUID[2]);

                                DicomFile dicomFile = response.GetValueAsync().Result;

                                string patientName = dicomFile.Dataset.GetString(DicomTag.PatientName);
                                string studyId = dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID);
                                string seriesNumber = dicomFile.Dataset.GetString(DicomTag.SeriesInstanceUID);
                                string instanceNumber = dicomFile.Dataset.GetString(DicomTag.InstanceNumber);
                                string imageComments = dicomFile.Dataset.GetString(DicomTag.ImageComments);

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"Patient Name: {patientName}   Study Id: {studyId}   Series: {seriesNumber}  Instance Number: {instanceNumber}  Image Comments: {imageComments}");

                            }
                            break;
                    }
                }
                catch (Microsoft.Health.Dicom.Client.DicomWebException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nResult: {0}", ex.StatusCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("\n\n\nPress any key to continue");
                ConsoleKeyInfo cki = Console.ReadKey(true);
            }
        }

        static async Task<string> GetAccessToken()
        {
            // OAuth necessary Information
            string tenantId = "Your Tenant ID"; // tenant-id
            string clientId = "Your Client ID"; // client-id 
            string clientSecret = "Your Client Secret"; // client-secret
            string resource = "https://dicom.healthcareapis.azure.com";

            // Construct the authority and token endpoints
            string authority = $"https://login.microsoftonline.com/{tenantId}";

            // Create a confidential client application object with your app id and secret
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            // Set the Scope to our Dicom Server
            var scopes = new[] { $"{resource}/.default" };
            
            // Request the Bearer Token
            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            // Return The Access Tokem
            return authResult.AccessToken;
        }
    }
}