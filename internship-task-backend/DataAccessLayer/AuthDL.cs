using internship_task_backend.Model;
using MySql.Data.MySqlClient;
using System.Data.Common;
using static internship_task_backend.Model.LogIn;
using static internship_task_backend.Model.SignUp;
using static internship_task_backend.Model.AddLesson;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using static internship_task_backend.Model.GetLesson;
using System.Security.Cryptography;

namespace internship_task_backend.DataAccessLayer
{
    public class AuthDL : IAuthDL
    {
        public readonly IConfiguration _configuration;
        public readonly MySqlConnection _mySqlConnection;
        public AuthDL(IConfiguration configuration) {

            _configuration = configuration;
            _mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:MySqlDBConnection"]);
        }

        public async Task<AddLesson.AddLessonResponse> AddLesson(AddLesson.AddLessonRequest request)
        {
            AddLessonResponse res = new AddLessonResponse();
            res.success = true;
            res.message = " Course Lesson added successfully.";
            try
            {
                _mySqlConnection.Open();
                string query = @"insert into task_schema.lesson (id, title, description, credit_hour) values(@id, @title, @description, @credit_hour)";
                using (MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", request.Id);
                    cmd.Parameters.AddWithValue("@title", request.Title);
                    cmd.Parameters.AddWithValue("@description", request.Description);
                    cmd.Parameters.AddWithValue("@credit_hour", request.credit_hour);
                    cmd.CommandTimeout = 180;

                    int status = await cmd.ExecuteNonQueryAsync();

                    if (status <= 0)
                    {
                        res.success = false;
                        res.message = "Something went wrong";
                    }

                }
            }
            catch(Exception ex)
            {
                res.success = false;
                res.message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }
            return res;
        }

        public async Task<LogIn.LogInResponse> LogIn(LogIn.LogInRequest request)
        {
            
            LogInResponse res = new LogInResponse();
            res.success = true;
            res.message = " User logged in";
            try
            {
                
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string query = @"select * from task_schema.user_details where email = @email ";

                using (MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 180;
                    cmd.Parameters.AddWithValue("@email", request.email);
                    cmd.Parameters.AddWithValue("@password", request.password);
                   
                    using (MySqlDataReader dataReader = (MySqlDataReader)await cmd.ExecuteReaderAsync())

                    {                        
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                               string pass = dataReader.GetString(1);
                               //if(BCrypt.Net.BCrypt.Verify(request.password,pass ))
                               // {
                                    res.success = true;
                                    res.message = "User login successfull";
                                    res.loginInfo = new UserLoginInfo();

                                    res.loginInfo.email = dataReader.GetString(0);
                                    res.loginInfo.password = dataReader.GetString(1);

                                    res.token = GenerateJwt(res.loginInfo.email);
                                //}
                              /* else
                                {
                                    res.success = false;
                                    res.message = "password wrong";
                                }*/
                               

                               
                            }
                            
                        }
                        else
                        {
                            res.success = false;
                            res.message = "User doesnt exist";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return res;

            
        }

        public async Task<SignUp.SignUpResponse> SignUp(SignUpRequest request)
        {
            SignUpResponse res = new SignUpResponse();
            res.success = true;
            res.message = "User Registration Succesfull";
            try
            {
                if (!request.password.Equals(request.confirm_password))
                   {
                    res.success = false;
                    res.message = "Password and Confirm Password don't match";
                    return res;
                }
                if(_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string query = @"INSERT INTO task_schema.user_details(email, password) VALUES(@email, @password)";

                using (MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@email", request.email);
                    string hashPassword = BCrypt.Net.BCrypt.HashPassword(request.password);
                    cmd.Parameters.AddWithValue("@password",hashPassword);
                    cmd.CommandTimeout = 180;

                    int status = await cmd.ExecuteNonQueryAsync();

                    if(status <= 0)
                    {
                        res.success = false;
                        res.message = "Something went wrong";
                    }
                    
                }
            }catch(Exception ex)
            {
                res.success = false;
                res.message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return res;
        }

       public string GenerateJwt(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretkey;mysecretkey;mysecretkey;"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("Date", DateTime.Now.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audiance"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            string Data = new JwtSecurityTokenHandler().WriteToken(token);
            return Data;
        }

        async Task<GetLessonResponse> IAuthDL.GetLesson()
        {
            GetLessonResponse res = new GetLessonResponse();
            res.success = true;
            res.message = "Successful";
            try
            {
                _mySqlConnection.Open();
                string query = @"select * from task_schema.lesson";
                using (MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 180;


                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                res.id = reader.GetInt32(0);
                                res.title = reader.GetString(1);
                                res.description = reader.GetString(2);
                                res.credit_hour = reader.GetInt32(3);
                            }
                        }
                    }
                    
                        }
            }catch(Exception ex)
            {
                res.success = false;
                res.message = ex.Message;
            }
            return res;
        }
    }
}
