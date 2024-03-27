using Microsoft.AspNetCore.Mvc;

namespace SurveyApp.Controllers
{
    public class ImageData
    {
        public string? Image { get; set; }
    }

    public class ImageUploadController : Controller
    {
        [HttpPost]
        public IActionResult Index([FromBody] ImageData data)
        {
            if (data != null && !string.IsNullOrEmpty(data.Image))
            {
                try
                {
                    // Gelen veriyi base64'ten byte dizisine dönüştür
                    byte[] imageBytes = Convert.FromBase64String(data.Image);

                    // Dosya adı oluştur
                    string fileName = Guid.NewGuid().ToString() + ".jpg"; // veya istediğiniz uzantıyı kullanabilirsiniz

                    // Dosyayı wwwroot/img klasörüne kaydet
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

                    System.IO.File.WriteAllBytes(filePath, imageBytes);

                    // İşlemin başarılı olduğunu bildir
                    return Ok(fileName);
                }
                catch (Exception ex)
                {
                    // Hata durumunda istemciye uygun mesajı gönder
                    return BadRequest("Dosya kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                // Geçersiz istek durumunda uygun mesajı gönder
                return BadRequest("Gelen veri eksik veya hatalı.");
            }
        }
    }
}
