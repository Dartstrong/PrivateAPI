using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Entities;
namespace PrivateAPI.Repositories.Interfaces
{
    public interface IDialoguesRepository
    {
        Task<StatusCodeResult> CreateRequestStartDialogue(RequestStartDialogue requestStartDialogue, int sessionId);
    }
}
