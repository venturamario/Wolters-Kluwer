using System;
using System.Collections.Generic;
using System.IO;
using ClienteDesktop.Models;
using Newtonsoft.Json;
using ClienteDesktop.Helpers;

namespace ClienteDesktop.Services
{
    public class ImportService
    {
        public List<Client> Import(string filePath, Action<int> progressCallback)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            List<Client> rawData;
            
            if (extension == ".csv")
            {
                rawData = ImportFromCsv(filePath, progressCallback);
            }
            else if (extension == ".json")
            {
                rawData = ImportFromJson(filePath);
            }                
            else
            {
                throw new Exception("Formato de archivo no soportado.");                
            }

            if (rawData == null)
            {
                return new List<Client>();
            }

            // 2. SOLID: El servicio se encarga de entregar solo datos que CUMPLEN las reglas
            return rawData.Where(c => 
                Validator.IsValidDni(c.DNI) && 
                (string.IsNullOrEmpty(c.Email) || Validator.IsValidEmail(c.Email))
            ).ToList();
        }

        public List<Client> ImportFromCsv(string path, Action<int> reportProgress)
        {
            var list = new List<Client>();
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var cols = lines[i].Split(',');
                if (cols.Length >= 6)
                {
                    list.Add(new Client {
                        DNI = cols[0], FirstName = cols[1], LastName = cols[2],
                        BirthDate = DateTime.Parse(cols[3]), Phone = cols[4], Email = cols[5]
                    });
                }
                reportProgress((i * 100) / (lines.Length - 1));
            }
            return list;
        }
        public List<Client> ImportFromJson(string path)
        {
            var result = JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(path));
            return result ?? new List<Client>();
        }
    }
}