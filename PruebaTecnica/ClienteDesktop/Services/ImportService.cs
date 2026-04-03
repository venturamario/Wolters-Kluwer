using System;
using System.Collections.Generic;
using System.IO;
using ClienteDesktop.Models;
using Newtonsoft.Json;

namespace ClienteDesktop.Services
{
    public class ImportService
    {
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
            return JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(path));
        }
    }
}