using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IOTDataProcessor.Controllers
{
    [Route("devices")]
    [ApiController]    
    public class LightSensorController : ControllerBase
    {
        private readonly ILightSensorTelemetry _telemetryService;
        private readonly ILightSensor _lightSensorService;
        private readonly ISensorLogger _sensorLogger;


        public LightSensorController(ILightSensorTelemetry telemetryService, ILightSensor lightSensorService, ISensorLogger sensorLogger)
        {
            _telemetryService = telemetryService;
            _lightSensorService = lightSensorService;
            _sensorLogger = sensorLogger;
        }

        // POST /devices/{deviceId}/telemetry
        [HttpPost("{deviceId}/telemetry")]
        public async Task<IActionResult> Create([FromRoute, BindRequired] int deviceId, [FromBody] List<LightSensorTelemetryModel> telemetryData)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                int sensorId = await _lightSensorService.CreateOrGetSensorAsync(deviceId);

                telemetryData.ForEach(telemetry =>
                {
                    telemetry.LightSensorId = sensorId;
                });

                await _telemetryService.AddAsync(telemetryData);

                return Ok("Telemetry added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error. {ex.Message}");
            }
        }

        // GET /devices/{deviceId}/statistics
        [HttpGet("{deviceId}/statistics")]
        public async Task<IActionResult> GetStatistics([FromRoute, BindRequired] int deviceId)
        {
            var result = await _lightSensorService.GetFilteredMaximum(deviceId);
            return result == null ? NotFound() : Ok(result);
        }

        //POST /devices/{deviceId}/log
        [HttpPost("{deviceId}/log")]
        [Authorize]
        public async Task<IActionResult> CreateLog([FromRoute, BindRequired] int deviceId, [FromBody] string message)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                await _lightSensorService.CreateOrGetSensorAsync(deviceId);
                var logModel = new SensorLogModel() { DeviceId = deviceId, Message = message };

                await _sensorLogger.AddLogAsync(logModel);

                return Ok("Log added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error. {ex.Message}");
            }
        }
    }
}
