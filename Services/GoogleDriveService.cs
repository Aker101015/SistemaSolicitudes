using Microsoft.AspNetCore.Http;
using System.IO;

namespace SistemaSolicitudes.Services
{
    public class GoogleDriveService
    {
        //private readonly DriveService _driveService;
        //private readonly string _carpetaSolicitudesId;
        //private readonly string _carpetaRespuestasId;

        //public GoogleDriveService(IConfiguration configuration)
        //{
        //    var credPath = configuration["GoogleDrive:CredPath"];
        //    _carpetaSolicitudesId = configuration["GoogleDrive:CarpetaSolicitudesId"] ?? throw new InvalidOperationException("CarpetaSolicitudesId no está configurada.");
        //    _carpetaRespuestasId = configuration["GoogleDrive:CarpetaRespuestasId"] ?? throw new InvalidOperationException("CarpetaRespuestasId no está configurada.");

        //    var credential = GoogleCredential.FromFile(credPath).CreateScoped(DriveService.Scope.Drive);
        //    _driveService = new DriveService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "Sistema de Solicitudes"
        //    });
        //}

        //public async Task<string> SubirArchivoAsync(IFormFile archivo, string nombreArchivo, bool esSolicitud = true)
        //{
        //    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        //    {
        //        Name = nombreArchivo,
        //        Parents = new[] { esSolicitud ? _carpetaSolicitudesId : _carpetaRespuestasId },
        //        MimeType = "application/pdf"
        //    };

        //    await using var memoryStream = new MemoryStream();
        //    await archivo.CopyToAsync(memoryStream);
        //    memoryStream.Position = 0;

        //    var request = _driveService.Files.Create(fileMetadata, memoryStream, "application/pdf");
        //    request.Fields = "webViewLink";

        //    // Define uploadProgress correctamente
        //    var uploadProgress = await request.UploadAsync();

        //    if (uploadProgress.Status == Google.Apis.Upload.UploadStatus.Completed)
        //    {
        //        var driveFile = request.ResponseBody;
        //        await _driveService.Permissions.Create(
        //            new Google.Apis.Drive.v3.Data.Permission
        //            {
        //                Role = "reader",
        //                Type = "anyone"
        //            },
        //            driveFile.Id
        //        ).ExecuteAsync();

        //        return driveFile.WebViewLink;
        //    }
        //    else
        //    {
        //        //Muestra el estado real del error
        //        var mensajeError = $"Subida fallida. Estado: {uploadProgress.Status}";
        //        if (uploadProgress.Exception != null)
        //            mensajeError += $", Excepción: {uploadProgress.Exception?.Message}";

        //        Console.WriteLine($"Error en GoogleDriveService: {mensajeError}");
        //        throw new InvalidOperationException(mensajeError);
        //    }
        //}

        // En GoogleDriveService.cs, cambia el método SubirArchivoAsync para guardar localmente
        public async Task<string> SubirArchivoAsync(IFormFile archivo, string nombreArchivo, bool esSolicitud = true)
        {
            var rutaCarpeta = esSolicitud ? "wwwroot/uploads/solicitudes" : "wwwroot/uploads/respuestas";
            Directory.CreateDirectory(rutaCarpeta);

            var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/uploads/{(esSolicitud ? "solicitudes" : "respuestas")}/{nombreArchivo}";
        }


    }
}