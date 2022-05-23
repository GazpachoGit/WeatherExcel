using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using weatherApp.Controllers.Models;
using weatherApp.Controllers.Queries;
using weatherApp.Models;

namespace weatherApp.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly WeatherContext db;

        public WeatherController(WeatherContext db)
        {
            this.db = db;
        }

        //[HttpGet("")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        //[HttpGet]
        //public async Task<IEnumerable<WeatherInfo>> Get()
        //{            
        //    return await db.WeatherInfo.ToListAsync();
        //}
        [HttpGet]
        [Route("layout")]
        public async Task<LayoutResouce> Layout()
        {
            var res = await db.WeatherInfo.Select(r => new { r.Month, r.Year }).Distinct().ToListAsync();
            return new LayoutResouce()
            {
                Months = res.Select(r => r.Month).Distinct().ToList(),
                Years = res.Select(r => r.Year).Distinct().ToList(),
            };
        }
        [HttpGet]
        public async Task<WeatherResource> Get([FromQuery] WeatherQuery weatherQuery)
        {
            var query = db.WeatherInfo.AsQueryable();           
            if(weatherQuery.Month != null)
            {
                query = query.Where(r => r.Month == weatherQuery.Month);
            }
            if(weatherQuery.Year != null)
            {
                query = query.Where(r => r.Year == weatherQuery.Year);
            }
            var count = await query.CountAsync();
            if (weatherQuery.Page <= 0) weatherQuery.Page = 1;
            query = query.Skip(25 * (weatherQuery.Page - 1)).Take(25);
            query = query.OrderBy(r => r.Date).ThenBy(r => r.Time);
            var list = await query.ToListAsync();
            return new WeatherResource()
            {
                Result = list,
                TotalFound = count
            };
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormFileCollection files)
        {
            var errors = new List<string>();
            try
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var sheetErrors = await ReadFile(formFile);
                        errors.AddRange(sheetErrors);
                    }
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if (errors.Count > 0) 
            {
                var error = new ErrorDto() { Message = String.Join(", ", errors.ToArray()) } ;
                return BadRequest(error.ToString());
            } 
            return Ok();
        }
        private async Task<List<string>> ReadFile(IFormFile formFile)
        {
            var errorList = new List<string>();
            var ext = Path.GetExtension(formFile.FileName);
            if (ext == ".xls" || ext == ".xlsx")
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    stream.Position = 0;
                    var wb = new XSSFWorkbook(stream);
                    for (var i = 0; i < wb.NumberOfSheets; i++)
                    {
                        var sheet = wb.GetSheetAt(i);
                        try
                        {
                            if (sheet.LastRowNum > 4)
                            {
                                await ReadSheet(sheet);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorList.Add(sheet.SheetName);
                            continue;
                        }
                    }
                }
            }
            else
            {
                errorList.Add(formFile.FileName);
            }
            return errorList;

        }
        private async Task ReadSheet(ISheet sheet)
        {
            var sheetName = sheet.SheetName.Split(' ');
            var month = sheetName[0];
            var year = sheetName[1];
            var recordsList = new List<WeatherInfo>();
            for (int rowIndex = 4; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    recordsList.Add(new WeatherInfo()
                    {
                        Year = year,
                        Month = month,
                        Date = GetCellValue(row.GetCell(0)),
                        Time = GetCellValue(row.GetCell(1)),
                        Temperature = GetCellValue(row.GetCell(2)),
                        Humidity = GetCellValue(row.GetCell(3)),
                        DewPoint = GetCellValue(row.GetCell(4)),
                        Pressure = GetCellValue(row.GetCell(5)),
                        WindDirection = GetCellValue(row.GetCell(6)),
                        WindSpeed = GetCellValue(row.GetCell(7)),
                        Cloudy = GetCellValue(row.GetCell(8)),
                        CloudyLowBorder = GetCellValue(row.GetCell(9)),
                        Visibility = GetCellValue(row.GetCell(10)),
                        WeatherPhenomenon = GetCellValue(row.GetCell(11)),
                    });
                }               
            }
            await db.WeatherInfo.AddRangeAsync(recordsList);
            await db.SaveChangesAsync();
        }
        private string GetCellValue(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Numeric)
            {
                return cell.NumericCellValue.ToString();
            }
            var output = cell.StringCellValue;
            return (!String.IsNullOrWhiteSpace(output)) ? output : null;
        }
    }
}
