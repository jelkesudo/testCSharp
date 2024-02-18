using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using testCSharp.Services;

namespace testCSharp.Controllers
{
    public class PieChartController : Controller
    {
        private readonly ILogger<PieChartController> _logger;
        private readonly ApiService _apiService;
        public PieChartController(ILogger<PieChartController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {
            var employees = await _apiService.GetEmployees("/api/gettimeentries?code=", "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==");
            var employeesList = employees.ToList();

            Dictionary<string, long> newSortedEmployees = _apiService.SortWorkingHoursByEmployee(employeesList);

            Dictionary<string, decimal> newSortedEmployeesPercentages = getTotalWorktimePercentages(newSortedEmployees);

            var image = CreatePieChartImage(newSortedEmployeesPercentages);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "piechart.png");
            SaveBitmap(image, imagePath, SKEncodedImageFormat.Png);

            ViewBag.PieChartImagePath = "/images/piechart.png";

            return View();
        }

        private Dictionary<string, decimal> getTotalWorktimePercentages(Dictionary<string, long> data)
        {
            long sumTimeWorked = data.Values.Sum();
            Dictionary<string, decimal> temp = new Dictionary<string, decimal>();
            foreach (var kvp in data)
            {
                temp.Add(kvp.Key, Math.Round((decimal)kvp.Value * 100 / sumTimeWorked, 2));
            }
            return temp;
        }

        private void SaveBitmap(SKBitmap bitmap, string filePath, SKEncodedImageFormat format)
        {
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(format, 100))
            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }

        private SKBitmap CreatePieChartImage(Dictionary<string, decimal> data)
        {
            var bitmap = new SKBitmap(400, 300);

            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true,
                };

                SKRect bounds = new SKRect(0, 0, 400, 300);

                float total = (float)data.Values.Sum();
                float startAngle = 0;

                foreach (var kvp in data)
                {
                    float sweepAngle = (float)kvp.Value / total * 360;
                    paint.Color = GetRandomColor();

                    canvas.DrawArc(bounds, startAngle, sweepAngle, true, paint);

                    float labelAngle = startAngle + sweepAngle / 2;
                    float labelRadius = 130;

                    float labelX = 170 + labelRadius * (float)Math.Cos(DegreesToRadians(labelAngle));
                    float labelY = 160 + labelRadius * (float)Math.Sin(DegreesToRadians(labelAngle));

                    canvas.DrawText($"{kvp.Key}{kvp.Value}%", labelX, labelY, new SKPaint
                    {
                        TextSize = 9,
                        Color = SKColors.Black
                    });

                    startAngle += sweepAngle;
                }
            }

            return bitmap;
        }

        private float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180);
        }

        private SKColor GetRandomColor()
        {
            Random rand = new Random();
            return new SKColor((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
        }
    }
}
