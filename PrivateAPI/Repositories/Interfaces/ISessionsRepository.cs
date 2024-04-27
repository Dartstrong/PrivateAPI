using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PrivateAPI.Models;
using PrivateAPI.Entity;
using System.Security.Cryptography;

namespace PrivateAPI.Repositories.Interfaces
{
    public interface ISessionsRepository
    {
        Task<ActionResult<Session>> CreateNewSession(RSAPublicKey rsaPublicKey);//начало новой сессии и передача пользователю симметричного ключа для дальнейшего обмена данными
        //Task EndSession(string sessionId, EndSessionConfirm endSessionConfirm);//окончание сеанса
        byte[] Encrypt(byte[] data, RSAParameters publicKey);//зашифровка данных для дальнейшего обмена данных
        int[] ByteArrayToIntArray(byte[] data);//перевод массива байтов в массив типа int
        string IntArrayToStr(int[] data);//перевод массива типа int в строку
    }
}