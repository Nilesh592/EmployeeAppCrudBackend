using Azure;
using Employee_Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Employee_Crud.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [Route("GetAllEmployee")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            var response = new ServiceResponseDto<List<EmployeeModel>>
            {
                Data = new List<EmployeeModel>()
            };

            try
            {
                using (var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand("proc_EmployeeGet", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var employee = new EmployeeModel
                                {
                                    Id = reader.GetInt32("Id"),
                                    EmployeeName = reader.IsDBNull("EmployeeName") ? null : reader.GetString("EmployeeName"),
                                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    Salary = reader.GetDecimal("Salary"),
                                    DateOfBirth = reader.IsDBNull("DateOfBirth") ? null : reader.GetDateTime("DateOfBirth"),
                                    UploadPicture = reader.IsDBNull("UploadPicture") ? null : reader.GetString("UploadPicture"),
                                    PayrollType = reader.IsDBNull("PayrollType") ? null : reader.GetInt32("PayrollType")
                                };
                                response.Data.Add(employee);
                            }
                        }
                    }

                    response.Success = true;
                    response.Message = "Employees retrieved successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        [Route("PostEmployee")]
        [HttpPost]
        public async Task<IActionResult> PostEmployee(EmployeeModel obj)
        {
            var response = new ServiceResponseDto<EmployeeModel>();

            
            if (obj == null || !ModelState.IsValid)
            {
                response.Success = false;
                response.Message = "Invalid employee data.";
                return BadRequest(response);
            }

            try
            {
                using (var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand("proc_EmployeePost", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Explicitly define parameter types to avoid issues with AddWithValue
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = obj.Id;
                        cmd.Parameters.Add("@EmployeeName", SqlDbType.NVarChar, 100).Value = obj.EmployeeName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value = obj.Salary;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = obj.Email ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value = obj.DateOfBirth.HasValue ? obj.DateOfBirth : (object)DBNull.Value;
                        cmd.Parameters.Add("@UploadPicture", SqlDbType.NVarChar, 255).Value = obj.UploadPicture ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@PayrollType", SqlDbType.NVarChar, 50).Value = obj.PayrollType ?? (object)DBNull.Value;

                        
                        await cmd.ExecuteNonQueryAsync();

                        
                        response.Success = true;
                        response.Data = obj; 
                        response.Message = "Employee created successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response); 
            }

            return Ok(response);
        }

        [Route("UpdateEmployee")]
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(EmployeeModel obj)
        {
            var response = new ServiceResponseDto<EmployeeModel>();
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd;
                SqlDataAdapter da;
                DataTable dt = new DataTable();

                cmd = new SqlCommand("proc_EmployeeUpdate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", obj.Id);
                cmd.Parameters.AddWithValue("@EmployeeName", obj.EmployeeName);
                cmd.Parameters.AddWithValue("@Salary", obj.Salary);
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                cmd.Parameters.AddWithValue("@DateOfBirth", obj.DateOfBirth);
                cmd.Parameters.AddWithValue("@UploadPicture", obj.UploadPicture);
                cmd.Parameters.AddWithValue("@PayrollType", obj.PayrollType);
                da = new SqlDataAdapter(cmd);
                da.Update(dt);

                response.Success = true;
                response.Data = obj;
                response.Message = "Employee details updated successfully.";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        [Route("DeleteEmployee/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Employee(int id)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand("proc_EmployeeDelete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                
                return Ok();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("/UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            var response = new ServiceResponseDto<List<string>>();
            try
            {
                var UploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles");
                if (!Directory.Exists(UploadPath))
                {
                    Directory.CreateDirectory(UploadPath);
                }
                var uploadedFiles = new List<string>();
                foreach (var file in files)
                {
                    var filename = DateTime.Now.Ticks.ToString() + file.FileName;
                    var filePath = Path.Combine(UploadPath, filename);

                    await using var stram = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stram);
                    uploadedFiles.Add(filename);
                }

                response.Success = true;
                response.Message = "Files uploaded successfully.";
                response.Data = uploadedFiles;
                return Ok(response);

            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Message = $"File operation failed: {ex.Message}";
                return StatusCode(500, response);
            }
        }
    }
}

