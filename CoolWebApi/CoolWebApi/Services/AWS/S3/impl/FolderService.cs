using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CoolWebApi.Utils.Wrapper;
using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Services.Identity;
using CoolWebApi.Utils.Entities.Catalog;
using CoolWebApi.Utils.Entities.Misc;
using CoolWebApi.Utils.Entities.ExtendedAttributes;
using CoolWebApi.Utils.Repositories;
using CoolWebApi.Services.AWS;
using Amazon.S3;
using CoolWebApi.Models.Requests.AWS;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using CoolWebApi.Models.DTO.AWS;
using IResult = CoolWebApi.Utils.Wrapper.IResult;

namespace CoolWebApi.Services.AWS.impl
{
    public class FolderService : IFolderService
    {

        private readonly IAmazonS3 _s3Client;

        public FolderService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        
    }
}