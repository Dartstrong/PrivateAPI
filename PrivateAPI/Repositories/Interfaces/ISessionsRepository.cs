using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PrivateAPI.Models;
using PrivateAPI.Entity;
namespace PrivateAPI.Repositories.Interfaces
{
    public interface ISessionsRepository
    {
        Task<ActionResult<Session>> CreateNewSession(RSAPublicKey rsaPublicKey);//начало новой сессии и передача пользователю симметричного ключа для дальнейшего обмена данными
    }
}