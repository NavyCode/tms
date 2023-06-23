using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Plans;
using TestPlanService.Models.Projects;
using TestPlanService.Services.Db.Tables;
using File = TestPlanService.Services.Db.Tables.File;

namespace TestPlanService.Services.Db.SubSystems
{
    public class FilesSubsystem
    {
        DatabaseService _db;

        public FilesSubsystem(DatabaseService context)
        {
            _db = context;
        }

        public File Upload(Project project, string name, byte[] data, string tags)
        {
            using (var compressedFileStream = new MemoryStream())
            { 
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    var zipEntry = zipArchive.CreateEntry(name);
                    using (var originalFileStream = new MemoryStream(data))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                }
                var file = new File()
                {
                    Data = compressedFileStream.ToArray(),
                    Compressed = true,
                    CreateDate = DateTime.Now,
                    Name = name,
                    Tags = tags,
                    Project = project,
                };
                _db.Context.Files.Add(file);
                _db.Context.SaveChanges();
                return file;
            }
        }

        public byte[] Download(int id, out string name)
        {
            name = null;
            var file = _db.Context.Files.FirstOrDefault(p => p.Id == id);
            if (file == null)
                return null;

            name = file.Name;
            using (var decompressedStream = new MemoryStream())
            using (var compressedFileStream = new MemoryStream(file.Data))
            {
                using (ZipArchive archive = new ZipArchive(compressedFileStream))
                {
                    if(archive.Entries.Count == 1)
                    {
                        using (var zipEntryStream = archive.Entries.First().Open())
                        {
                            zipEntryStream.CopyTo(decompressedStream);
                            return decompressedStream.ToArray();
                        }
                    }
                }
            }
            return null;
        }
    }
}
