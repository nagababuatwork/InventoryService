using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryService.Models;
using Microsoft.AspNetCore.Authorization;

namespace InventoryService.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoesController : ControllerBase
    {
        private readonly InventoryContext _context;
        public UserInfoesController(InventoryContext context)
        {
            _context = context;
        }
       
        [HttpGet("GetUserDetails")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserInfo()
        {
            return await _context.UserInfo.ToListAsync();
        }

        [HttpGet("GetUserDetailsById/{Id}")]
        public async Task<ActionResult<UserInfo>> GetUserInfo(int id)
        {
            var userInfo = await _context.UserInfo.FindAsync(id);

            if (userInfo == null)
            {
                return NotFound();
            }
            return userInfo;
        }
        
        [HttpPut("UpdateUserDetails/{Id}")]
        public async Task<IActionResult> PutUserInfo(UserInfo userInfo, int id)
        {
            userInfo.CreatedDate = DateTime.Now;
            if (id != userInfo.UserId)
            {
                return BadRequest();
            }
            
            _context.Entry(userInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        
        [HttpPost("InsertUserDetails")]
        public async Task<ActionResult<UserInfo>> PostUserInfo(UserInfo userInfo)
        {
            var checkUserExists = (from UserInfo in _context.UserInfo
                                   where (UserInfo.UserName == userInfo.UserName || UserInfo.Email == userInfo.Email)
                                   select new UserInfo
                                   {
                                       UserId = UserInfo.UserId
                                   }).FirstOrDefault();
            if (checkUserExists == null)
            {
                userInfo.CreatedDate = DateTime.Now;
                _context.UserInfo.Add(userInfo);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUserInfo", new { id = userInfo.UserId }, userInfo);
            }
            else
            {
                return BadRequest("user name or email already exists");
            }
        }

        [HttpDelete("DeleteUserDetails/{id}")]
        public async Task<ActionResult<UserInfo>> DeleteUserInfo(int id)
        {
            var userInfo = await _context.UserInfo.FindAsync(id);
            if (userInfo == null)
            {
                return NotFound();
            }

            _context.UserInfo.Remove(userInfo);
            await _context.SaveChangesAsync();

            return userInfo;
        }
        private bool UserInfoExists(int id)
        {
            return _context.UserInfo.Any(e => e.UserId == id);
        }
    }
}
