using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;
using System.Security.Cryptography;
using OpenPassVault.Shared.Crypto.Interfaces;
using SkiaSharp;

namespace OpenPassVault.API.Services;

public class CaptchaService(IHmacService hmacService) : ICaptchaService
{
    private const int ImageWidth = 200;
    private const int ImageHeight = 80;
    private const int CaptchaLength = 9;
    
    private static readonly char[] Characters = "abcdefghijklmnopqrstuvwxyz1234567890-".ToCharArray();
    
    public Task<bool> VerifyCaptcha(string captchaResponse, string captchaHmac)
    {
        var captchaResponseBytes = System.Text.Encoding.UTF8.GetBytes(captchaResponse);
        
        return hmacService.VerifyHmac(captchaHmac, captchaResponseBytes);
    }

    public async Task<NewCaptchaResponse> GenerateCaptcha()
    {
        var maxIndex = Characters.Length - 1;
        var captchaCodeChars = Enumerable.Range(0, CaptchaLength)
            .Select(_ => Characters[RandomNumberGenerator.GetInt32(maxIndex)])
            .ToArray();
        
        var captchaCode = string.Join("", captchaCodeChars);

        var image = await CreateImage(string.Join("", captchaCode));
        var captchaHmac = await hmacService.CreateHmac(System.Text.Encoding.UTF8.GetBytes(captchaCode));
        
        return new NewCaptchaResponse { CaptchaImageBase64 = image, CaptchaHmac = captchaHmac};
    }
    
    private Task<string> CreateImage(string captchaCode)
    {
        var imageInfo = new  SKImageInfo(width: ImageWidth, height: ImageHeight, SKColorType.RgbaF32);
        using var bitmap = new SKBitmap(imageInfo);
        using var bitmapCanvas = new SKCanvas(bitmap);
        //bitmapCanvas.Clear();

        const int xCoordinate = 20;
        const int yCoordinate = ImageHeight / 2 + 16;
        bitmapCanvas.DrawText(
            captchaCode,
            new SKPoint(xCoordinate, yCoordinate), // Adjust position as needed
            new SKFont(SKTypeface.FromFamilyName("Arial"), 24),
            new SKPaint
            {
                Color = SKColors.Gray,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            }
        );

        using var bitmapPng = bitmap.Encode(SKEncodedImageFormat.Png, 65);
        return Task.FromResult(Convert.ToBase64String(bitmapPng.ToArray()));
    }
}