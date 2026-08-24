using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

[ApiController]
[Route("api/[controller]")]
public class QrCodeController : ControllerBase
{

    [HttpGet("generate/{text}")]
    public IActionResult GenerateQrCode(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return BadRequest(new { message = "Text parameter cannot be empty" });
        }

        try
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    return File(qrCodeImage, "image/png", "qrcode.png");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error generating QR code", error = ex.Message });
        }
    }
}


