using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRUDService.Helpers;
using CRUDService.Models;
using DTOModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;


namespace CRUDService.Services
{
    public interface IDataService
    {
        Task<UsersDTO> RegisterUserAsync(UsersDTO userdto, string password);
        Task<UsersDTO> GetUserAsync(UsersDTO userdto);
        Task<UsersDTO> GetUserAsync(Guid userid);
        Task<UsersDTO> AuthenticateUser(UsersDTO userdto, string password);
        IEnumerable<UsersDTO> GetUsers();
        Task<IEnumerable<ConversationsDTO>> GetConversations(string username);
        IEnumerable<MessagesDTO> GetConversationsMessages(string conversationid, string userid);
        ConversationsDTO SaveNewConversation(ConversationsDTO conversation);
        bool SaveMessage(MessagesDTO message);
    }

    public class DataService : IDataService
    {
        private readonly ChatapplicationContext _context;
        private readonly IMapper _mapper;
        private readonly string _connectionstring;
        public DataService(ChatapplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _connectionstring = _context.Database.GetDbConnection().ConnectionString;
        }

        public async Task<UsersDTO> RegisterUserAsync(UsersDTO userdto, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == userdto.Username))
                throw new AppException("Username \"" + userdto.Username + "\" is already taken");
            var user = _mapper.Map<Users>(userdto);
            user.UserId = Guid.NewGuid();

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var userreturndto = _mapper.Map<UsersDTO>(user);
            return userreturndto;
        }

        public async Task<UsersDTO> GetUserAsync(UsersDTO userdto)
        {
            try
            {
                var user = _mapper.Map<Users>(userdto);

                return  _mapper.Map<UsersDTO>(await _context.Users.FirstOrDefaultAsync(u => u == user));

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UsersDTO> GetUserAsync(Guid userid)
        {
            try
            {
                return _mapper.Map<UsersDTO>(await _context.Users.FirstOrDefaultAsync(u => u.UserId == userid));

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UsersDTO> AuthenticateUser(UsersDTO userdto, string password)
        {
            if (string.IsNullOrEmpty(userdto.Username) || string.IsNullOrEmpty(password))
                return null;

            var authuser = await _context.Users.SingleOrDefaultAsync(x => x.Username == userdto.Username);

            // check if username exists
            if (authuser == null)
                return null;

            //// check if password is correct
            if (!VerifyPasswordHash(password, authuser.PasswordHash, authuser.PasswordSalt))
                return null;
            // authentication successful
            var usereturndto = _mapper.Map<UsersDTO>(authuser);
            return usereturndto;
        }

        public IEnumerable<UsersDTO> GetUsers()
        {
            try
            {
                var users = _context.Users.ToList();
                var usersdto = _mapper.Map<IEnumerable<UsersDTO>>(users);
                return usersdto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<ConversationsDTO>> GetConversations(string userid)
        {
            try
            {
                var p = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userid)
                };
                var datatable = GetDataTable("Sel_Conversations", p);
                var conversations = new List<ConversationsDTO>();
                foreach(DataRow row in datatable.Rows)
                {
                    var conversation = new ConversationsDTO();
                    conversation.ConversationId = Guid.Parse(row["ConversationId"].ToString());
                    conversation.UsersConversations = new List<UsersConversationsDTO>{
                        new UsersConversationsDTO{
                            ConversationId = Guid.Parse(row["ConversationId"].ToString()),
                            UserId = Guid.Parse(row["UserId"].ToString()),
                            User = new UsersDTO
                            {
                                UserId = Guid.Parse(row["UserId"].ToString()),
                                FirstName = row["FirstName"].ToString(),
                                LastName = row["LastName"].ToString()
                            }
                        }
                    };
                    conversations.Add(conversation);
                }
                return conversations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<MessagesDTO> GetConversationsMessages(string conversationid, string userid)
        {
            try
            {
                var p = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userid),
                    new SqlParameter("@ConversationId", conversationid)
                };
                var datatable = GetDataTable("Sel_ConversationMessages", p);
                var messages = new List<MessagesDTO>();
                foreach (DataRow row in datatable.Rows)
                {
                    var message = new MessagesDTO();
                    message.MessageId = Guid.Parse(row["MessageId"].ToString());
                    message.Message = row["Message"].ToString();
                    message.ConversationId = Guid.Parse(row["ConversationId"].ToString());
                    message.TimeStamp = DateTime.Parse(row["TimeStamp"].ToString());
                    message.UserId = Guid.TryParse(row["UserId"].ToString(), out var guid) ? (Guid?)guid : null;
                    messages.Add(message);
                }
                return messages.OrderBy(m => m.TimeStamp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConversationsDTO SaveNewConversation(ConversationsDTO conversation)
        {
            try
            {
                var p = new List<SqlParameter>
                {
                    new SqlParameter("@UserId1", conversation.UsersConversations[0].UserId),
                    new SqlParameter("@UserId2", conversation.UsersConversations[1].UserId)
                };
                var datatable = GetDataTable("Ins_Conversation", p);            
                var newconversation = new ConversationsDTO { ConversationId = Guid.Parse(datatable.Rows[0]["ConversationId"].ToString()) };
                return newconversation;          
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool SaveMessage(MessagesDTO message)
        {
            try
            {
                var messagetoadd = _mapper.Map<Messages>(message);
                _context.Add(messagetoadd);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                    if (computedHash[i] != storedHash[i])
                        return false;
            }

            return true;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        



        private  DataSet GetDataSet(string spName, IReadOnlyCollection<SqlParameter> sqlParams)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionstring))
                {
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParams != null && sqlParams.Count > 0)
                        {
                            foreach (var p in sqlParams)
                            {
                                cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                            }
                        }
                        conn.Open();
                        try
                        {
                            var da = new SqlDataAdapter(cmd);
                            var ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }


        private DataTable GetDataTable(string spName, IReadOnlyCollection<SqlParameter> sqlParams)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionstring))
                {
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParams != null && sqlParams.Count > 0)
                        {
                            foreach (var p in sqlParams)
                            {
                                cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                            }
                        }
                        conn.Open();
                        try
                        {
                            var adapt = new SqlDataAdapter(cmd);
                            var ds = new DataSet();
                            adapt.Fill(ds);
                            return (ds.Tables.Count > 0 ? ds.Tables[0] : null);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        private object GetValue(string spName, IReadOnlyCollection<SqlParameter> sqlParams)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionstring))
                {
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParams != null && sqlParams.Count > 0)
                        {
                            foreach (var p in sqlParams)
                            {
                                cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                            }
                        }

                        conn.Open();
                        try
                        {

                            return cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }


        private  void ExecuteNonQuery(string spName, IReadOnlyCollection<SqlParameter> sqlParams)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionstring))
                {
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParams != null && sqlParams.Count > 0)
                        {
                            foreach (var p in sqlParams)
                            {
                                cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                            }
                        }
                        conn.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
